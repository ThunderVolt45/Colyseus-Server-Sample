using System.Threading.Tasks;
using UnityEngine;

namespace Colyseus_Client
{
    public class NetworkTransform : NetworkComponent
    {
        public ColyseusTransform networkTransform;

        [Header("Syncronization Settings")]
        [Range(0, 100)]
        [Tooltip("보간 속도")]
        [SerializeField] private float lerpSpeed = 20f;

        [Header("Precision Settings")]
        [Range(0, 6)]
        [Tooltip("네트워크로 전송하는 Transform 위치 정밀도 (10 ^ n)")]
        [SerializeField] private int PositionPrecision = 3;

        [Range(0, 6)]
        [Tooltip("네트워크로 전송하는 Transform 회전 정밀도 (10 ^ n)")]
        [SerializeField] private int RotationPrecision = 2;

        [Range(0, 6)]
        [Tooltip("네트워크로 전송하는 Transform 크기 정밀도 (10 ^ n)")]
        [SerializeField] private int ScalePrecision = 2;

        private Transform _transform;
        private NetworkTransformManager _transformManager;

        #region Initialize
        private void Start()
        {
            _transform = GetComponent<Transform>();
            _transformManager = NetworkTransformManager.Instance;

            networkTransform = new ColyseusTransform();

            UpdateNetworkTransform();
        }
        #endregion

        #region Update
        protected virtual void Update()
        {
            if (!IsMine)
            {
                UpdateTransform();
                return;
            }

            BroadcastTransform();
        }

        private void UpdateTransform()
        {
            if (lerpSpeed > 0)
            {
                float lerp = Mathf.Clamp(Time.deltaTime * lerpSpeed, 0f, 0.999f);

                Vector3 lerpPos = Vector3.Lerp(_transform.position, networkTransform.position, lerp);
                Quaternion lerpRot = Quaternion.Lerp(_transform.rotation, networkTransform.rotation, lerp);
                Vector3 lerpSca = Vector3.Lerp(_transform.localScale, networkTransform.scale, lerp);

                _transform.SetPositionAndRotation(lerpPos, lerpRot);
                _transform.localScale = lerpSca;
            }
            else
            {
                _transform.SetPositionAndRotation(networkTransform.position, networkTransform.rotation);
                _transform.localScale = networkTransform.scale;
            }
        }

        private void UpdateNetworkTransform()
        {
            networkTransform.position = new ColyseusVector3(_transform.position, PositionPrecision);
            networkTransform.rotation = new ColyseusQuaternion(_transform.rotation, RotationPrecision);
            networkTransform.scale = new ColyseusVector3(_transform.localScale, ScalePrecision);
        }
        #endregion

        #region Networking
        private void BroadcastTransform()
        {
            if (string.IsNullOrEmpty(SessionId))
            {
                return;
            }

            networkTransform.position = new ColyseusVector3(_transform.position, PositionPrecision);
            networkTransform.rotation = new ColyseusQuaternion(_transform.rotation, RotationPrecision);
            networkTransform.scale = new ColyseusVector3(_transform.localScale, ScalePrecision);

            _transformManager.BroadcastTransform(ObjectId, networkTransform);
        }

        public async void OnAddTransform(ColyseusTransform tf)
        {
            if (IsMine) return;

            while (_transform == null)
            {
                await Awaitable.NextFrameAsync();
            }

            this.networkTransform = tf;
            _transform.SetPositionAndRotation(tf.position, tf.rotation);
            _transform.localScale = tf.scale;
        }

        public void OnChangeTransform(ColyseusTransform tf)
        {
            if (IsMine) return;

            this.networkTransform = tf;
        }
        #endregion
    }
}
