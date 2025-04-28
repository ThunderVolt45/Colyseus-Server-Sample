using System;
using System.Collections.Generic;
using UnityEngine;

namespace Colyseus_Client
{
    public class NetworkObject : MonoBehaviour
    {
        private Dictionary<Type, NetworkComponent> _cachedNetworkViews = new Dictionary<Type, NetworkComponent>();

        public string ObjectId => _objectId;
	    public string SessionId => _sessionId;
        public string PrefabName => _prefabName;
	    public bool IsMine => _isMine;

        [Header("States")]
        [SerializeField] private string _objectId;
        [SerializeField] private string _prefabName;
        [SerializeField] private string _sessionId;
        [SerializeField] private bool _isMine;

        public void SetObjectId(string objectId)
        {
            _objectId = objectId;
        }

        public virtual void Initialize(string prefabName, string sessionId, bool isMine)
        {
            _prefabName = prefabName;
            _sessionId = sessionId;
            _isMine = isMine;
        }

        public T GetNetworkComponent<T>() where T : NetworkComponent
        {
            Type type = typeof(T);

            if (_cachedNetworkViews.ContainsKey(type))
            {
                return (T)_cachedNetworkViews[type];
            }

            T component = GetComponent<T>();

            if (component != null)
            {
                _cachedNetworkViews[type] = component;
            }

            return component;
        }
    }
}
