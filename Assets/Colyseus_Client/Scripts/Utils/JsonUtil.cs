using UnityEngine;
using static Newtonsoft.Json.JsonConvert;

public class JsonUtil
{
    public static T Deserialize<T>(string json)
    {
        return DeserializeObject<T>(json);
    }

    public static string Serialize<T>(T obj)
    {
        return SerializeObject(obj);
    }

    public static T QuickDeserialize<T>(string json)
    {
        return JsonUtility.FromJson<T>(json);
    }

    public static string QuickSerialize<T>(T obj)
    {
        return JsonUtility.ToJson(obj);
    }
}
