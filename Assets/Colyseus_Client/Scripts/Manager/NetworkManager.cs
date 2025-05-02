using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colyseus;
using Colyseus.Schema;
using UnityEngine;
using UnityEngine.Events;

namespace Colyseus_Client
{
	[Serializable]
	public class RoomData
	{
		public string roomTitle;
		public string password;
		public int maxClients;
		public string roomTag;
	}

	public class AvailableRoom : ColyseusRoomAvailable
	{
		public RoomData metadata;
	}
	
	public class NetworkManager : MonoBehaviour
	{
		const string ServerResponseLog = "Server-Response-Log";
		const string ServerResponseWarning = "Server-Response-Warning";
		const string ServerResponseError = "Server-Response-Error";

		public enum ServerState { Waiting, Connecting, Connected, Disconnected }

		public static NetworkManager Instance;

		[Header("Server")]
		public string ServerName = "gameroom";
		public string URL = "ws://localhost:2567";
		public string AuthKey = "c21eaa06f11eca81f1663a1315bd6928cf46833e";
		public string SessionId;

		[Header("Host")]
		public bool IsHost = false;

		[Header("Options")]
		public int patchRate = 30;
		public int maxClient = 10;

		// [Header("Room Metadata")]
		// public string roomTitle;
		// public string password;
		// public string roomTag;

		[Header("Connection State")]
		public ServerState serverState = ServerState.Waiting;

		public ColyseusRoom<GameRoomState> room;
		private StateCallbackStrategy<GameRoomState> callback;
		public Dictionary<string, NetworkObject> NetworkObjects;

		private ColyseusClient client;
		private Dictionary<string, object> options = new Dictionary<string, object>();

		[Space(10f)]
		public UnityEvent<StateCallbackStrategy<GameRoomState>> stateCallbackEvent;
		public UnityEvent<ColyseusRoom<GameRoomState>> roomInitializeEvent;

		#region Initialize
		private void Awake()
		{
			if (Instance != null)
			{
				Debug.LogWarning("Multiple instance of ServerView is not allowed. Instance destroyed.");
				Destroy(this);
				return;
			}

			Instance = this;

			stateCallbackEvent = new();
			NetworkObjects = new Dictionary<string, NetworkObject>();
		}

		private async void Start()
		{
			client = new ColyseusClient(URL);
			SetOptions();

			await JoinOrCreateRoom();
		}
		#endregion

		#region Room - Join/Create
		private void SetOptions()
		{
			// server
			options["authKey"] = AuthKey;

			// options
			options["patchRate"] = patchRate;
			options["maxClients"] = maxClient;

			// meta data
			// options["roomTitle"] = roomTitle;
			// options["password"] = password;
			// options["roomTag"] = roomTag;

			// player
			options["player"] = JsonUtil.Serialize(NetworkPlayerManager.Instance.PlayerData);
		}
		#endregion

		public async Awaitable<bool> JoinOrCreateRoom()
		{
			serverState = ServerState.Connecting;
			Debug.Log("Try Join/Create room...");

			try
			{
				room = await client.JoinOrCreate<GameRoomState>(ServerName, options);

				IsHost = true;
				InitRoom(room);
				
				return true;
			}
			catch (Exception e)
			{
				Debug.LogWarning(e);
			}

			Debug.LogError("Fail to Join/Create Room!");
			return false;
		}

		#region Room - Initialize
		private void InitRoom(ColyseusRoom<GameRoomState> room)
		{
			SetServerResponseListener(room);
			SetRoomMessageListener(room);
			SetStateCallbackListener(room);

			roomInitializeEvent?.Invoke(room);
		}

		private void SetServerResponseListener(ColyseusRoom<GameRoomState> room)
		{
			room.OnMessage<string>(ServerResponseLog, message =>
			{
				Debug.Log(message);
			});

			room.OnMessage<string>(ServerResponseWarning, message =>
			{
				Debug.LogWarning(message);
			});

			room.OnMessage<string>(ServerResponseError, message =>
			{
				Debug.LogError(message);
			});
		}

		private void SetRoomMessageListener(ColyseusRoom<GameRoomState> room)
		{
			room.OnMessage<List<string>>("__playground_message_types", message => {
				string log = "This room will listen the following messages: ";
				foreach (var item in message)
				{
					log += item + ", ";
				}
				Debug.Log(log);
			});

			room.OnMessage<string>("player", sessionId =>
			{
				SessionId = sessionId;
				serverState = ServerState.Connected;
			});

			room.OnMessage<bool>("host", ishost =>
			{
				IsHost = ishost;
			});

			room.OnMessage<RoomData>("metadata", metadata =>
			{
				Debug.Log(metadata);
			});

			// 네트워크 오브젝트 생성 실패시 재시도
			room.OnMessage<string>("Create-Fail", async objectId =>
			{
				string prefabName = NetworkObjects[objectId].PrefabName;

				// 생성 실패한 네트워크 오브젝트 제거
				Destroy(NetworkObjects[objectId].gameObject);
				NetworkObjects.Remove(objectId);

				// 재생성 시도
				await NetworkInstantiate(prefabName);
			});
		}

		private void SetStateCallbackListener(ColyseusRoom<GameRoomState> room)
		{
			callback = Callbacks.Get(room);

			callback.OnAdd(state => state.Objects, async (sessionId, Obj) => {
				if (!NetworkObjects.ContainsKey(Obj.objectId))
				{
					// 리소스 로드
					var prefab = await InstantiateNetworkObject(Obj.prefabName);

					// 네트워크 오브젝트 초기화
					NetworkObject networkObject = prefab.GetComponent<NetworkObject>();
					networkObject.SetObjectId(Obj.objectId);
					networkObject.Initialize(Obj.prefabName, Obj.owner, SessionId == Obj.owner);

					// 네트워크 오브젝트 등록
					NetworkObjects.Add(Obj.objectId, networkObject);
				}

				// 변경점 추적
				callback.OnChange(Obj, () => {
					NetworkObjects[Obj.objectId].Initialize(Obj.prefabName, Obj.owner, SessionId == Obj.owner);
				});
			});

			callback.OnRemove(state => state.Objects, (sessionId, Obj) => {
				if (NetworkObjects.ContainsKey(Obj.objectId))
				{
					// 오브젝트 제거
					Destroy(NetworkObjects[Obj.objectId].gameObject);
					NetworkObjects.Remove(Obj.objectId);
				}
			});

			stateCallbackEvent?.Invoke(callback);
		}
		#endregion

		#region Network - Instantiate
		public async Task<NetworkObject> NetworkInstantiate(string prefabName, bool destroyOnOwnerLeave = true)
		{
			NetworkObj networkObj = new NetworkObj();

			networkObj.objectId = Guid.NewGuid().ToString();
			networkObj.prefabName = prefabName;
			networkObj.destroyOnOwnerLeave = destroyOnOwnerLeave;

            _ = room.Send("Create", networkObj);

			var prefab = await InstantiateNetworkObject(prefabName);

			// 네트워크 오브젝트 초기화
			NetworkObject networkObject = prefab.GetComponent<NetworkObject>();
			networkObject.SetObjectId(networkObj.objectId);
			networkObject.Initialize(networkObj.prefabName, networkObj.owner, true);

			// 네트워크 오브젝트 등록
			NetworkObjects.Add(networkObj.objectId, networkObject);

			return networkObject;
		}

        private async Task<GameObject> InstantiateNetworkObject(string prefabName)
        {
			// 리소스가 로드될 때까지 대기
			var request = Resources.LoadAsync(prefabName);
			while (request.isDone)
			{
				await Awaitable.NextFrameAsync();
			}

			// 프리팹 생성
			return (GameObject)Instantiate(request.asset);
		}

        public async void NetworkDestroy(NetworkObject networkObject)
		{
			NetworkObj networkObj = new NetworkObj();

			networkObj.objectId = networkObject.ObjectId;

			await room.Send("Destroy", networkObj);

			Destroy(networkObject.gameObject);
			NetworkObjects.Remove(networkObject.ObjectId);
		}
		#endregion

		#region Room - Leave
		public async void LeaveRoom()
		{
			if (room == null)
				return;
			
			await room.Leave();
		}

		private void OnDestroy()
		{
			if (room != null)
				LeaveRoom();
		}
		#endregion
	}
}
