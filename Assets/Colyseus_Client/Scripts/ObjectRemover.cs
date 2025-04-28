using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Colyseus_Client;

public class ObjectRemover : MonoBehaviour
{
    private void Start()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickStart);
    }

    public void OnClickStart()
    {
        var manager = NetworkManager.Instance;
        var creater = FindAnyObjectByType<ObjectCreater>();

        foreach (var createdObject in creater.createdObjects)
        {
            manager.NetworkDestroy(createdObject);
        }

        creater.createdObjects.Clear();
    }
}
