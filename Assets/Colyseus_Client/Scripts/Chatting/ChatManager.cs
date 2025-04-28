using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Colyseus_Client
{
    public class ChatManager : MonoBehaviour
    {
        [Header("Prefabs")]
        [SerializeField] private ChatMessageObject messageObject;

        [Header("Scroll View")]
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private Transform scrollviewContent;

        [Header("InputField")]
        [SerializeField] private TMP_InputField inputField;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (inputField.gameObject.activeSelf)
                {
                    SendMessage();
                    inputField.gameObject.SetActive(false);
                }
                else
                {
                    inputField.gameObject.SetActive(true);
                    inputField.ActivateInputField();
                }
            }
        }

        public void CreateChatObject(ChatMessage message)
        {
            var chat = Instantiate(messageObject, scrollviewContent);
            chat.SetText(message);

            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }

        public void SendMessage()
        {
            inputField.text.Trim();

            if (string.IsNullOrWhiteSpace(inputField.text))
            {
                Debug.Log("Inputfield is null or whitespace. SendMessage Halted.");
                return;
            }

            NetworkChatManager.Instance.Send(inputField.text);
            inputField.text = "";

            Canvas.ForceUpdateCanvases();
            scrollRect.verticalNormalizedPosition = 0f;
        }
    }
}
