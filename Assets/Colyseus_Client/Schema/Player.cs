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

public partial class Player : Schema {
#if UNITY_5_3_OR_NEWER
[Preserve]
#endif
public Player() { }
	[Type(0, "string")]
	public string nickname = default(string);

	[Type(1, "string")]
	public string metadata = default(string);

	[Type(2, "boolean")]
	public bool isHost = default(bool);

	[Type(3, "boolean")]
	public bool connected = default(bool);

	[Type(4, "array", typeof(ArraySchema<string>), "string")]
	public ArraySchema<string> ownedObjects = null;
}

