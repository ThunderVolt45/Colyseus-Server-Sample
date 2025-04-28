using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class EventCaller : MonoBehaviour
{
    public UnityEvent eventToCall;

    private void Start()
    {
        var btn = GetComponent<Button>();
        btn.onClick.AddListener(OnClickButton);
    }

    public void OnClickButton()
    {
        eventToCall?.Invoke();
        gameObject.SetActive(false);
    }
}
