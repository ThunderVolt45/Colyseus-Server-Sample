import { Schema, type } from "@colyseus/schema";

export class NetworkObj extends Schema {
    @type("string") objectId: string = "";
    @type("string") prefabName: string = "";
    @type("string") owner: string = "";
    @type("boolean") destroyOnOwnerLeave: boolean = true;
}
