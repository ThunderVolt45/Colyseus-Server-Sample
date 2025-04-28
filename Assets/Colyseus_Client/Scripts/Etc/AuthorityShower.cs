using UnityEngine;
using Colyseus_Client;

[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(NetworkObject))]
public class AuthorityShower : MonoBehaviour
{
    [SerializeField] private Material materialMine;
    [SerializeField] private Material materialOther;

    private NetworkObject _networkObject;
    private MeshRenderer _meshRenderer;

    void Start()
    {
        _networkObject = GetComponent<NetworkObject>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (_networkObject.IsMine)
        {
            _meshRenderer.material = materialMine;
        }
        else
        {
            _meshRenderer.material = materialOther;
        }
    }
}
