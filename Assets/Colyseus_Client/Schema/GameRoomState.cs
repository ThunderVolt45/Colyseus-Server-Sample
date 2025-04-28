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

public partial class GameRoomState : Schema {
#if UNITY_5_3_OR_NEWER
[Preserve]
#endif
public GameRoomState() { }
	[Type(0, "string")]
	public string hostId = default(string);

	[Type(1, "map", typeof(MapSchema<Player>))]
	public MapSchema<Player> Players = null;

	[Type(2, "map", typeof(MapSchema<NetworkObj>))]
	public MapSchema<NetworkObj> Objects = null;

	[Type(3, "map", typeof(MapSchema<string>), "string")]
	public MapSchema<string> Transforms = null;

	[Type(4, "map", typeof(MapSchema<string>), "string")]
	public MapSchema<string> Animations = null;

	[Type(5, "map", typeof(MapSchema<string>), "string")]
	public MapSchema<string> Rigidbodies = null;
}

