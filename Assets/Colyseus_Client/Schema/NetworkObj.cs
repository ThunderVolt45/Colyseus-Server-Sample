// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 3.0.19
// 

using Colyseus.Schema;
#if UNITY_5_3_OR_NEWER
using UnityEngine.Scripting;
#endif

public partial class NetworkObj : Schema {
#if UNITY_5_3_OR_NEWER
[Preserve]
#endif
public NetworkObj() { }
	[Type(0, "string")]
	public string objectId = default(string);

	[Type(1, "string")]
	public string prefabName = default(string);

	[Type(2, "string")]
	public string owner = default(string);

	[Type(3, "boolean")]
	public bool destroyOnOwnerLeave = default(bool);
}

