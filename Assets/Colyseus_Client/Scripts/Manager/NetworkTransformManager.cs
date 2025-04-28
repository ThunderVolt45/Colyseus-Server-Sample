using System;
using Colyseus;
using Colyseus.Schema;
using UnityEngine;

namespace Colyseus_Client
{
    public class NetworkTransformManager : MonoBehaviour
    {
        public static NetworkTransformManager Instance;

        private NetworkManager networkManager;

        #region Initialize
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Multiple instance of NetworkTransformManager is not allowed. Instance destroyed.");
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            networkManager = NetworkManager.Instance;
            networkManager.stateCallbackEvent.AddListener(TransformListener);
        }

        public void TransformListener(StateCallbackStrategy<GameRoomState> callback)
        {
            callback.OnAdd(state => state.Transforms, (sessionId, tf) => {
                OnAddTransform(sessionId, tf);
                callback.OnChange(state => state.Transforms, (sessionId, tf) => OnChangeTransform(sessionId, tf));
            });

            callback.OnRemove(state => state.Transforms, (sessionId, _) => {
                OnRemoveTransform(sessionId);
            });
        }
        #endregion

        #region Networking
        private async void OnAddTransform(string key, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            while (!networkManager.NetworkObjects.ContainsKey(key))
            {
                await Awaitable.NextFrameAsync();
            }

            var tf = JsonUtil.QuickDeserialize<ColyseusTransform>(message);
            networkManager.NetworkObjects[key].GetNetworkComponent<NetworkTransform>().OnAddTransform(tf);
        }

        private void OnChangeTransform(string key, string message)
        {
            if (networkManager.NetworkObjects.TryGetValue(key, out var obj))
            {
                var tf = JsonUtil.QuickDeserialize<ColyseusTransform>(message);
                obj.GetNetworkComponent<NetworkTransform>().OnChangeTransform(tf);
            }
        }

        private void OnRemoveTransform(string key)
        {
            if (networkManager.NetworkObjects.TryGetValue(key, out var obj))
            {
                obj.GetNetworkComponent<NetworkTransform>().SetEnable(false);
            }
        }

        public async void BroadcastTransform(string objectId, ColyseusTransform tf)
        {
            TransformMessage message = new TransformMessage(objectId, JsonUtil.QuickSerialize(tf));
            await networkManager.room.Send("Transform", message);
        }
        #endregion
    }
}
