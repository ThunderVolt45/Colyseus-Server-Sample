using System;
using UnityEngine;

namespace Colyseus_Client
{
    [RequireComponent(typeof(Animator))]
    public class NetworkAnimation : NetworkComponent
    {
        private ColyseusAnimation parameter;
        private Tuple<int, AnimatorControllerParameterType>[] animationHashes;

        private Animator animator;
        private NetworkAnimationManager animationManager;

        #region Initialize
        private void Awake()
        {
            animator = GetComponent<Animator>();
            animationManager = NetworkAnimationManager.Instance;

            InitializeAnimation();
        }

        private void InitializeAnimation()
        {
            parameter = new ColyseusAnimation();
            parameter.param = new string[animator.parameterCount];

            animationHashes = new Tuple<int, AnimatorControllerParameterType>[animator.parameterCount];

            for (int i = 0; i < animator.parameterCount; i++)
            {
                var animParam = animator.GetParameter(i);
                animationHashes[i] = new Tuple<int, AnimatorControllerParameterType>(animParam.nameHash, animParam.type);
            }
        }
        #endregion

        #region Update
        public void Update()
        {
            if (IsMine)
            {
                BroadcastAnimation();
            }
        }
        #endregion

        #region Networking
        private void BroadcastAnimation()
        {
            for (int i = 0; i < animator.parameterCount; i++)
            {
                switch (animator.GetParameter(i).type)
                {
                    case AnimatorControllerParameterType.Float:
                        parameter.param[i] = animator.GetFloat(animationHashes[i].Item1).ToString();
                        break;
                    case AnimatorControllerParameterType.Int:
                        parameter.param[i] = animator.GetInteger(animationHashes[i].Item1).ToString();
                        break;
                    case AnimatorControllerParameterType.Bool:
                    case AnimatorControllerParameterType.Trigger:
                        parameter.param[i] = animator.GetBool(animationHashes[i].Item1).ToString();
                        break;
                }
            }

            animationManager.BroadcastAnimation(ObjectId, parameter);
        }

        public void OnChangeAnimation(ColyseusAnimation parameter)
        {
            this.parameter = parameter;

            for (int i = 0; i < parameter.param.Length; i++)
            {
                int hash = animationHashes[i].Item1;
                string param = parameter.param[i];

                switch (animationHashes[i].Item2)
                {
                    case AnimatorControllerParameterType.Float:
                        animator.SetFloat(hash, float.Parse(param));
                        break;
                    case AnimatorControllerParameterType.Int:
                        animator.SetInteger(hash, int.Parse(param));
                        break;
                    case AnimatorControllerParameterType.Bool:
                    case AnimatorControllerParameterType.Trigger:
                        animator.SetBool(hash, bool.Parse(param));
                        break;
                }
            }
        }
        #endregion
    }
}
