using System;
using Colyseus;
using Colyseus.Schema;
using UnityEngine;

namespace Colyseus_Client
{
    public class NetworkAnimationManager : MonoBehaviour
    {
        public static NetworkAnimationManager Instance;

        private NetworkManager networkManager;
        
        #region Initialize
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Multiple instance of NetworkAnimationManager is not allowed. Instance destroyed.");
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            networkManager = NetworkManager.Instance;
            networkManager.stateCallbackEvent.AddListener(AnimationListener);
        }

        public void AnimationListener(StateCallbackStrategy<GameRoomState> callback)
        {
            callback.OnAdd(state => state.Animations, (sessionId, entity) => {
                OnAddAnimation(sessionId, entity);
                callback.OnChange(state => state.Animations, (sessionId, entity) => OnChangeAnimation(sessionId, entity));
            });

            callback.OnRemove(state => state.Animations, (sessionId, _) => {
                OnRemoveAnimation(sessionId);
            });
        }
        #endregion

        #region Networking
        private async void OnAddAnimation(string key, string anim)
        {
            if (string.IsNullOrEmpty(anim))
            {
                return;
            }

            while (!networkManager.NetworkObjects.ContainsKey(key))
            {
                await Awaitable.NextFrameAsync();
            }
            
            var parameter = JsonUtil.QuickDeserialize<ColyseusAnimation>(anim);
            networkManager.NetworkObjects[key].GetNetworkComponent<NetworkAnimation>().OnChangeAnimation(parameter);
        }

        private void OnChangeAnimation(string key, string anim)
        {
            if (string.IsNullOrEmpty(anim))
            {
                return;
            }

            if (networkManager.NetworkObjects.TryGetValue(key, out var obj))
            {
                var parameter = JsonUtil.QuickDeserialize<ColyseusAnimation>(anim);

                if (obj != null)
                    obj.GetNetworkComponent<NetworkAnimation>().OnChangeAnimation(parameter);
            }
        }

        private void OnRemoveAnimation(string key)
        {
            if (networkManager.NetworkObjects.TryGetValue(key, out var obj))
            {
                obj.GetNetworkComponent<NetworkAnimation>().SetEnable(false);
            }
        }

        public async void BroadcastAnimation(string objectId, ColyseusAnimation parameter)
        {
            AnimationMessage message = new AnimationMessage(objectId, JsonUtil.QuickSerialize(parameter));
            await networkManager.room.Send("Animation", message);
        }
        #endregion
    }
}
