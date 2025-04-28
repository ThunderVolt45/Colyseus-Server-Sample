using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Colyseus_Client
{
    public class RPCSample : MonoBehaviour
    {
        [SerializeField] private Button btnRpcAll;
        [SerializeField] private Button btnRpcOther;
        [SerializeField] private Button btnRpcTarget;
        [SerializeField] private TMP_InputField inputTarget;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private void Start()
        {
            SetButtonListener();
            InitializeRPC();
        }

        private void SetButtonListener()
        {
            btnRpcAll.onClick.AddListener(() =>
            {
                RPCParameter parameter = new RPCParameter
                    (NetworkManager.Instance.SessionId, "Is Send RPC to everyone.");
                
                NetworkRPCManager.Instance.SendRPC("RPC", parameter, RPCTarget.All);
            });

            btnRpcOther.onClick.AddListener(() =>
            {
                RPCParameter parameter = new RPCParameter
                    (NetworkManager.Instance.SessionId, "Is Send RPC", "to other.");
                
                NetworkRPCManager.Instance.SendRPC("RPC", parameter, RPCTarget.Other);
            });

            btnRpcTarget.onClick.AddListener(() =>
            {
                RPCParameter parameter = new RPCParameter
                    (new object[]{NetworkManager.Instance.SessionId, "Is Send RPC to YOU."});
                
                NetworkRPCManager.Instance.SendRPC("RPC", parameter, inputTarget.text);
            });
        }

        private void InitializeRPC()
        {
            NetworkRPCManager.Instance.AddRPCFunction(RPC);
        }

        private void RPC(RPCParameter parameter)
        {
            string log = "";

            for (int i = 0; i < parameter.parameters.Length; i++)
            {
                if (parameter.parameters[i] != null)
                    log += parameter.parameters[i] + " ";
            }

            Debug.Log(log);
        }
    }
}
