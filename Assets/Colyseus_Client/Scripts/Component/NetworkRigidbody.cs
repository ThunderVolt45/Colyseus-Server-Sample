using UnityEngine;

namespace Colyseus_Client
{
    [RequireComponent(typeof(Rigidbody))]
    public class NetworkRigidbody : NetworkComponent
    {
        public ColyseusRigidbody networkRigidbody;

        [Header("Syncronization Settings")]
        [Range(0, 100)]
        [Tooltip("보간 속도")]
        [SerializeField] private float lerpSpeed = 20f;

        [Header("Precision Settings")]
        [Range(0, 6)]
        [Tooltip("네트워크로 전송하는 Rigidbody 속도 정밀도 (10 ^ n)")]
        [SerializeField] private int VelocityPrecision = 3;

        [Range(0, 6)]
        [Tooltip("네트워크로 전송하는 Rigidbody 각속도 정밀도 (10 ^ n)")]
        [SerializeField] private int AngularVelocityPrecision = 3;

        private Rigidbody _rigidbody;
        private NetworkRigidbodyManager _rigidbodyManager;

        #region Initialize
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbodyManager = NetworkRigidbodyManager.Instance;

            networkRigidbody = new ColyseusRigidbody();

            UpdateNetworkRigidbody();
        }
        #endregion

        #region Update
        protected virtual void Update()
        {
            if (!IsMine)
            {
                UpdateRigidbody();
                return;
            }

            BroadcastRigidbody();
        }

        private void UpdateRigidbody()
        {
            _rigidbody.mass = networkRigidbody.mass;
            _rigidbody.linearDamping = networkRigidbody.drag;
            _rigidbody.angularDamping = networkRigidbody.angularDrag;
            _rigidbody.useGravity = networkRigidbody.gravity;
            _rigidbody.isKinematic = networkRigidbody.kinematic;

            if (lerpSpeed > 0)
            {
                float lerp = Mathf.Clamp(Time.deltaTime * lerpSpeed, 0f, 0.999f);

                Vector3 lerpVelocity = Vector3.Lerp(_rigidbody.linearVelocity, networkRigidbody.velocity, lerp);
                Vector3 lerpAngularVelocity = Vector3.Lerp(_rigidbody.angularVelocity, networkRigidbody.angularVelocity, lerp);

                _rigidbody.linearVelocity = lerpVelocity;
                _rigidbody.angularVelocity = lerpAngularVelocity;
            }
            else
            {
                _rigidbody.linearVelocity = networkRigidbody.velocity;
                _rigidbody.angularVelocity = networkRigidbody.angularVelocity;
            }
        }

        private void UpdateNetworkRigidbody()
        {
            networkRigidbody.mass = _rigidbody.mass;
            networkRigidbody.drag = _rigidbody.linearDamping;
            networkRigidbody.angularDrag = _rigidbody.angularDamping;
            networkRigidbody.gravity = _rigidbody.useGravity;
            networkRigidbody.kinematic = _rigidbody.isKinematic;

            networkRigidbody.velocity = new ColyseusVector3(_rigidbody.linearVelocity, VelocityPrecision);
            networkRigidbody.angularVelocity = new ColyseusVector3(_rigidbody.angularVelocity, AngularVelocityPrecision);
        }
        #endregion

        #region Networking
        private void BroadcastRigidbody()
        {
            if (string.IsNullOrEmpty(SessionId))
            {
                return;
            }

            UpdateNetworkRigidbody();
            _rigidbodyManager.BroadcastRidgebody(ObjectId, networkRigidbody);
        }

        public async void OnAddRigidbody(ColyseusRigidbody rb)
        {
            if (IsMine) return;

            while (_rigidbody == null)
            {
                await Awaitable.NextFrameAsync();
            }

            networkRigidbody = rb;
            
            _rigidbody.mass = networkRigidbody.mass;
            _rigidbody.linearDamping = networkRigidbody.drag;
            _rigidbody.angularDamping = networkRigidbody.angularDrag;
            _rigidbody.useGravity = networkRigidbody.gravity;
            _rigidbody.isKinematic = networkRigidbody.kinematic;

            _rigidbody.linearVelocity = networkRigidbody.velocity;
            _rigidbody.angularVelocity = networkRigidbody.angularVelocity;
        }

        public void OnChangeRigidbody(ColyseusRigidbody rb)
        {
            if (IsMine) return;

            networkRigidbody = rb;
        }
        #endregion
    }
}
