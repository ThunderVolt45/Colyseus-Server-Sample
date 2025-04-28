using UnityEngine;
using UnityEngine.UI;
using Colyseus_Client;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mono.Cecil.Cil;

public class ObjectCreater : MonoBehaviour
{
    [SerializeField] private GameObject targetPrefab;

    [Space(10f)]
    public List<NetworkObject> createdObjects = new List<NetworkObject>();

    private void Start()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickStart);
    }

    public async void OnClickStart()
    {
        var createdObject = await NetworkManager.Instance.NetworkInstantiate(targetPrefab.name);
        
        createdObjects.Add(createdObject);
    }
}
