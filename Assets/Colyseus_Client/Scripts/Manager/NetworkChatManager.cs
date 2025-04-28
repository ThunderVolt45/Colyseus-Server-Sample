using System;
using Colyseus;
using UnityEngine;
using UnityEngine.Events;

namespace Colyseus_Client
{
    [Serializable]
    public class ChatMessage
    {
        public string sessionId;
        public string nickname;
	    public string time;
	    public string message;

        public bool IsMine()
        {
            return sessionId.Equals(NetworkManager.Instance.SessionId);
        }
    }

    public class NetworkChatManager : MonoBehaviour
    {
        public static NetworkChatManager Instance;

        public UnityEvent<ChatMessage> ChatEvent;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Multiple instance of NetworkChatManager is not allowed. Instance destroyed.");
                Destroy(this);
                return;
            }

            Instance = this;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
	    {
	    	NetworkManager.Instance.roomInitializeEvent.AddListener(ChattingListener);
	    }

        public void ChattingListener(ColyseusRoom<GameRoomState> room)
        {
            room.OnMessage<ChatMessage>("Chat", (chatMsg) =>
            {
                ChatEvent?.Invoke(chatMsg);
            });
        }

        public async void Send(string message)
        {
            ChatMessage chatMsg = new ChatMessage();

            chatMsg.sessionId = NetworkManager.Instance.SessionId;
            chatMsg.nickname = NetworkPlayerManager.Instance.LocalPlayer.Nickname;
            chatMsg.time = DateTime.Now.ToString();
            chatMsg.message = message;

            await NetworkManager.Instance.room.Send("Chat", chatMsg);
            
            ChatEvent?.Invoke(chatMsg);
        }
    }
}
