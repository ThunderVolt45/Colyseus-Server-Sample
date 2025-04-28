using UnityEngine;

namespace Colyseus_Client
{
    [RequireComponent(typeof(NetworkObject))]
    public class NetworkComponent : MonoBehaviour
    {
        public NetworkObject NetworkObject
        {
            get
            {
                if (_nob != null)
                    return _nob;
                
                return _nob = GetComponent<NetworkObject>();
            }
        }

        private NetworkObject _nob;

        public bool IsMine
        {
            get 
            {
                return NetworkObject.IsMine;
            }
        }

        public string SessionId
        {
            get
            {
                return NetworkObject.SessionId;
            }
        }

        public string ObjectId
        {
            get
            {
                return NetworkObject.ObjectId;
            }
        }

        public void SetEnable(bool active)
        {
            enabled = active;
        }

        public T GetNetworkComponent<T>() where T : NetworkComponent
        {
            return NetworkObject.GetNetworkComponent<T>();
        }
    }
}