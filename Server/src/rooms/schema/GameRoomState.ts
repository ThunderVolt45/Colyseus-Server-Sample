import { Schema, type, MapSchema } from "@colyseus/schema";
import { Player } from "./Player";
import { NetworkObj } from "./NetworkObj";

export class GameRoomState extends Schema {
    @type("string") hostId: string = "";
    @type({ map: Player }) Players: MapSchema<Player> = new MapSchema<Player>();
    @type({ map: NetworkObj }) Objects: MapSchema<NetworkObj> = new MapSchema<NetworkObj>();
    @type({ map: "string" }) Transforms: MapSchema<string> = new MapSchema<string>();
    @type({ map: "string" }) Animations: MapSchema<string> = new MapSchema<string>();
    @type({ map: "string" }) Rigidbodies: MapSchema<string> = new MapSchema<string>();
}
