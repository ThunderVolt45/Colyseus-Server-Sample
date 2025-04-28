using Colyseus.Schema;
using UnityEngine;

namespace Colyseus_Client
{
    public class NetworkRigidbodyManager : MonoBehaviour
    {
        public static NetworkRigidbodyManager Instance;

        private NetworkManager networkManager;

        #region Initialize
        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Multiple instance of NetworkRigidbodyManager is not allowed. Instance destroyed.");
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            networkManager = NetworkManager.Instance;
            networkManager.stateCallbackEvent.AddListener(RigidbodyListener);
        }

        public void RigidbodyListener(StateCallbackStrategy<GameRoomState> callback)
        {
            callback.OnAdd(state => state.Rigidbodies, (sessionId, rb) => {
                OnAddRigidbody(sessionId, rb);
                callback.OnChange(state => state.Rigidbodies, (sessionId, rb) => OnChangeRigidbody(sessionId, rb));
            });

            callback.OnRemove(state => state.Rigidbodies, (sessionId, _) => {
                OnRemoveRidgebody(sessionId);
            });
        }
        #endregion

        #region Networking
        private async void OnAddRigidbody(string key, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            while (!networkManager.NetworkObjects.ContainsKey(key))
            {
                await Awaitable.NextFrameAsync();
            }

            var rb = JsonUtil.QuickDeserialize<ColyseusRigidbody>(message);
            networkManager.NetworkObjects[key].GetNetworkComponent<NetworkRigidbody>().OnAddRigidbody(rb);
        }

        private void OnChangeRigidbody(string key, string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                return;
            }

            if (networkManager.NetworkObjects.TryGetValue(key, out var obj))
            {
                var rb = JsonUtil.QuickDeserialize<ColyseusRigidbody>(message);
                obj.GetNetworkComponent<NetworkRigidbody>().OnChangeRigidbody(rb);
            }
        }

        private void OnRemoveRidgebody(string key)
        {
            if (networkManager.NetworkObjects.TryGetValue(key, out var obj))
            {
                obj.GetNetworkComponent<NetworkRigidbody>().SetEnable(false);
            }
        }

        public async void BroadcastRidgebody(string objectId, ColyseusRigidbody rb)
        {
            RigidbodyMessage message = new RigidbodyMessage(objectId, JsonUtil.QuickSerialize(rb));
            await networkManager.room.Send("Rigidbody", message);
        }
        #endregion
    }
}
