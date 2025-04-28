using UnityEngine;

namespace Colyseus_Client
{
    public class NetworkPlayer : NetworkComponent
    {
        public string Nickname => _nickname;

        [Header("States")]
        [SerializeField] private string _nickname;

        public virtual void Initialize(string nickname)
        {
            _nickname = nickname;
        }

        public void SetNickname(string nickname)
        {
            _nickname = nickname;
        }
    }
}
