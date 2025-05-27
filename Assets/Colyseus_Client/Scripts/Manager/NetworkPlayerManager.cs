using System;
using System.Collections.Generic;
using Colyseus;
using Colyseus.Schema;
using UnityEngine;

namespace Colyseus_Client
{
    public class NetworkPlayerManager : MonoBehaviour
    {
        public static NetworkPlayerManager Instance;

        public Player PlayerData = new Player();
        public List<Player> players = new List<Player>();

        public NetworkPlayer LocalPlayer;
        public GameObject defaultPlayerPrefab;
        public Transform[] generatePosition;

        private NetworkManager networkManager;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Multiple instance of NetworkPlayerManager is not allowed. Instance destroyed.");
                Destroy(this);
                return;
            }

            Instance = this;
        }

        private void Start()
        {
            networkManager = NetworkManager.Instance;
            networkManager.stateCallbackEvent.AddListener(PlayerListener);
        }

        public void PlayerListener(StateCallbackStrategy<GameRoomState> callback)
        {
            callback.OnAdd(state => state.Players, (sessionId, player) => {
                OnAddPlayer(sessionId, player);
                callback.OnChange(player, () => OnChangePlayer(sessionId, player));
            });

            callback.OnRemove(state => state.Players, (sessionId, entity) => OnRemovePlayer(sessionId, entity));
        }

        public async void OnAddPlayer(string key, Player player)
        {
            while (networkManager.serverState != NetworkManager.ServerState.Connected)
            {
                await Awaitable.EndOfFrameAsync();
            }

            if (key == networkManager.SessionId)
            {
                if (defaultPlayerPrefab != null)
                {
                    var playerPrefab = await networkManager.NetworkInstantiate(defaultPlayerPrefab.name);
                    LocalPlayer = playerPrefab.GetNetworkComponent<NetworkPlayer>();
                }
            }

            // 플레이어 접속을 알림
            ChatMessage message = new ChatMessage("SYSTEM", "SYSTEM", DateTime.Now.ToString(), $"Player {key} join the room.");
            FindAnyObjectByType<ChatManager>().CreateChatObject(message);
        }

        private void OnChangePlayer(string key, Player player)
        {
            if (networkManager.NetworkObjects.TryGetValue(key, out var networkObject))
            {
                var playerView = networkObject.GetComponent<NetworkPlayer>();
                
                // change name
                if (playerView.Nickname != player.nickname)
                {
                    playerView.SetNickname(player.nickname);
                }
            }
        }

        public void OnRemovePlayer(string key, Player player)
        {
            // 플레이어 접속 중단을 알림
            ChatMessage message = new ChatMessage("SYSTEM", "SYSTEM", DateTime.Now.ToString(), $"Player {key} left the room.");
            FindAnyObjectByType<ChatManager>().CreateChatObject(message);
        }
    }
}
