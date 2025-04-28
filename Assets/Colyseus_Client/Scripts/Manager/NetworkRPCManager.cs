using System;
using System.Collections.Generic;
using Colyseus;
using UnityEngine;

namespace Colyseus_Client
{
    [Serializable]
    public class RPC
    {
        public string functionName;
        public string target;
        public RPCParameter parameter;

        public RPC(){}

        public RPC(string functionName, RPCParameter parameter)
        {
            this.functionName = functionName;
            this.parameter = parameter;
        }

        public RPC(string functionName, RPCParameter parameter, string targetSessionId)
        {
            this.functionName = functionName;
            this.parameter = parameter;
            target = targetSessionId;
        }
    }

    [Serializable]
    public class RPCParameter
    {
        public object[] parameters;

        public RPCParameter(){}

        public RPCParameter(object parameter1)
        {
            parameters = new object[]{parameter1};
        }

        public RPCParameter(object parameter1, object parameter2)
        {
            parameters = new object[]{parameter1, parameter2};
        }

        public RPCParameter(object parameter1, object parameter2, object parameter3)
        {
            parameters = new object[]{parameter1, parameter2, parameter3};
        }

        public RPCParameter(object parameter1, object parameter2, object parameter3, object parameter4)
        {
            parameters = new object[]{parameter1, parameter2, parameter3, parameter4};
        }

        public RPCParameter(object[] parameters)
        {
            this.parameters = parameters;
        }
    }

    public enum RPCTarget
    {
        Other,
        All
    }

    public class NetworkRPCManager : MonoBehaviour
    {
        public static NetworkRPCManager Instance;

        public delegate void RPCFunction(RPCParameter RPCparam);
        public Dictionary<string, RPCFunction> RPCFunctions;

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogWarning("Multiple instance of NetworkChatManager is not allowed. Instance destroyed.");
                Destroy(this);
                return;
            }

            Instance = this;
            RPCFunctions = new Dictionary<string, RPCFunction>();
        }

        private void Start()
        {
            NetworkManager.Instance.roomInitializeEvent.AddListener(RPCListener);
        }

        public void RPCListener(ColyseusRoom<GameRoomState> room)
        {
            room.OnMessage<RPC>("RPC", (rpc) =>
            {
                RPCFunctions[rpc.functionName](rpc.parameter);
            });
        }

        public void AddRPCFunction(RPCFunction RPCfunction)
        {
            RPCFunctions[RPCfunction.Method.Name] = RPCfunction;
        }

        public void AddRPCFunction(string RPCName, RPCFunction RPCfunction)
        {
            RPCFunctions[RPCName] = RPCfunction;
        }

        public async void SendRPC(string functionName, RPCParameter parameter, RPCTarget target = RPCTarget.Other)
        {
            RPC rpc = new(functionName, parameter, target.ToString());
            await NetworkManager.Instance.room.Send("RPC", rpc);
        }

        public async void SendRPC(string functionName, RPCParameter parameter, string targetSessionId)
        {
            RPC rpc = new(functionName, parameter, targetSessionId);
            await NetworkManager.Instance.room.Send("RPC", rpc);
        }
    }
}
