using System.Collections;
using TMPro;
using UnityEngine;

namespace Colyseus_Client
{
    public class ChatMessageObject : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [Range(0, 120)] [SerializeField] private float lifeTime = 0f;

        private IEnumerator Start()
        {
            if (lifeTime > 0f)
            {
                yield return new WaitForSeconds(lifeTime);
                Destroy(gameObject);
            }
        }

        public void SetText(ChatMessage message)
        {
            string name = string.IsNullOrEmpty(message.nickname) ? message.sessionId : message.nickname;

            text.text = $"{name} : {message.message}";
            text.color = message.IsMine() ? Color.green : Color.white;
        }
    }    
}
