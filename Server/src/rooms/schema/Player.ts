import { Schema, type, ArraySchema } from "@colyseus/schema";

export class Player extends Schema {
    @type("string") nickname: string = "";
    @type("string") metadata: string = "";
    @type("boolean") isHost: boolean = false;
    @type("boolean") connected: boolean = true;
    @type(["string"]) ownedObjects: ArraySchema<string> = new ArraySchema<string>();
}
