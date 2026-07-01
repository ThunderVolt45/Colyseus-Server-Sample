"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.GameRoomState = void 0;
const schema_1 = require("@colyseus/schema");
const Player_1 = require("./Player");
const NetworkObj_1 = require("./NetworkObj");
class GameRoomState extends schema_1.Schema {
    constructor() {
        super(...arguments);
        this.hostId = "";
        this.Players = new schema_1.MapSchema();
        this.Objects = new schema_1.MapSchema();
        this.Transforms = new schema_1.MapSchema();
        this.Animations = new schema_1.MapSchema();
        this.Rigidbodies = new schema_1.MapSchema();
    }
}
exports.GameRoomState = GameRoomState;
__decorate([
    (0, schema_1.type)("string")
], GameRoomState.prototype, "hostId", void 0);
__decorate([
    (0, schema_1.type)({ map: Player_1.Player })
], GameRoomState.prototype, "Players", void 0);
__decorate([
    (0, schema_1.type)({ map: NetworkObj_1.NetworkObj })
], GameRoomState.prototype, "Objects", void 0);
__decorate([
    (0, schema_1.type)({ map: "string" })
], GameRoomState.prototype, "Transforms", void 0);
__decorate([
    (0, schema_1.type)({ map: "string" })
], GameRoomState.prototype, "Animations", void 0);
__decorate([
    (0, schema_1.type)({ map: "string" })
], GameRoomState.prototype, "Rigidbodies", void 0);
