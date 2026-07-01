"use strict";
var __decorate = (this && this.__decorate) || function (decorators, target, key, desc) {
    var c = arguments.length, r = c < 3 ? target : desc === null ? desc = Object.getOwnPropertyDescriptor(target, key) : desc, d;
    if (typeof Reflect === "object" && typeof Reflect.decorate === "function") r = Reflect.decorate(decorators, target, key, desc);
    else for (var i = decorators.length - 1; i >= 0; i--) if (d = decorators[i]) r = (c < 3 ? d(r) : c > 3 ? d(target, key, r) : d(target, key)) || r;
    return c > 3 && r && Object.defineProperty(target, key, r), r;
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.Player = void 0;
const schema_1 = require("@colyseus/schema");
class Player extends schema_1.Schema {
    constructor() {
        super(...arguments);
        this.nickname = "";
        this.metadata = "";
        this.isHost = false;
        this.connected = true;
        this.ownedObjects = new schema_1.ArraySchema();
    }
}
exports.Player = Player;
__decorate([
    (0, schema_1.type)("string")
], Player.prototype, "nickname", void 0);
__decorate([
    (0, schema_1.type)("string")
], Player.prototype, "metadata", void 0);
__decorate([
    (0, schema_1.type)("boolean")
], Player.prototype, "isHost", void 0);
__decorate([
    (0, schema_1.type)("boolean")
], Player.prototype, "connected", void 0);
__decorate([
    (0, schema_1.type)(["string"])
], Player.prototype, "ownedObjects", void 0);
