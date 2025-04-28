import { Room, Client } from "@colyseus/core";
import { GameRoomState } from "./schema/GameRoomState";
import { Player } from "./schema/Player";
import { NetworkObj } from "./schema/NetworkObj";
import { v4 } from 'uuid';

const ServerResponseLog = "Server-Response-Log";
const ServerResponseWarning = "Server-Response-Warning";
const ServerResponseError = "Server-Response-Error";

//#region Room
export class GameRoom extends Room<GameRoomState> {
    state = new GameRoomState();
    maxClients = 4;
    patchRate = 25;

    // 방 생성
    onCreate(options: any) {
        // 기본 설정
        this.patchRate = options.patchRate;
        this.maxClients = options.maxClients;

        // 방 공개 여부 설정
        // this.setPrivate(options.private);
        
        // 메타데이터 설정
        this.setMetadata({
            ["roomTitle"]: options.roomTitle,
            ["password"]: options.password,
            ["maxClients"]: options.maxClients
        });
        
        SetRoomControlListener(this);
        SetRPCListener(this);
        SetChatListener(this);
        SetNetworkObjectListener(this);
        SetAuthorityListener(this);

        console.log(`room [${this.roomId}] has been created!`);
    }

    // 클라이언트 방 참가
    onJoin(client: Client, options: any) {
        // player 추가
        let playerData = JSON.parse(options.player);
        let player = new Player();
        
        player.nickname = playerData.nickname;
        player.metadata = playerData.metadata;

        this.state.Players.set(client.sessionId, player);
        
        // 호스트 설정
        if (this.state.Players.size === 1) {
            player.isHost = true;
            this.state.hostId = client.sessionId;
        }
        
        // 접속 알림
        client.send("player", client.sessionId);

        console.log(`${client.sessionId} join the room [${this.roomId}]!`);
    }

    // 클라이언트 방 이탈
    async onLeave(client: Client, consented: boolean) {
        // flag client as inactive for other users
        this.state.Players.get(client.sessionId).connected = false;

        try {
            if (consented) {
                throw new Error("consented leave");
            }
    
            // allow disconnected client to reconnect into this room until 20 seconds
            await this.allowReconnection(client, 20);
    
            // client returned! let's re-activate it.
            this.state.Players.get(client.sessionId).connected = true;
    
        } catch (e) {
            // 대기 시간이 끝났거나 클라이언트가 의도적으로 종료한 상태이므로 클라이언트를 룸에서 제거한다.
            // 먼저 소유 중인 오브젝트를 찾아서 모두 제거
            let ownedObjects = this.state.Players.get(client.sessionId).ownedObjects;
            ownedObjects.forEach(objectId => {
                let obj = this.state.Objects.get(objectId);
                if (obj.destroyOnOwnerLeave) {
                    this.state.Objects.delete(objectId);
                    this.state.Transforms.delete(objectId);
                    this.state.Animations.delete(objectId);
                }
                else {
                    obj.owner = "";
                }
            });
            
            // 마지막으로 플레이어 제거
            this.state.Players.delete(client.sessionId);
        }
        
        // host migration
        if (this.state.hostId === client.sessionId) {
            let sessionIds = Array.from(this.state.Players.keys());
            
            this.state.hostId = sessionIds[Math.floor(Math.random() * sessionIds.length)];
            this.clients.forEach(c => {
                if (c.sessionId === this.state.hostId) {
                    this.state.Players.get(c.sessionId).isHost = true;
                    c.send("host", true);
                }
            })
        }

        console.log(`${client.sessionId} leave the room [${this.roomId}]!`);
    }

    // 방 제거
    onDispose() {
        console.log(`room [${this.roomId}] disposing...`);
    }
}
//#endregion

//#region Room Control
function SetRoomControlListener(room: GameRoom) {
    // 강퇴
    room.onMessage("Kick", (client, message: string) => {
        if (client.sessionId !== room.state.hostId) {
            client.send(ServerResponseWarning, `Kick FAILED: Only the host can kick other clients.`);
            return;
        }

        for (client of room.clients) {
            if (client.sessionId === message) {
                client.leave();
                break;
            }
        }
    });

    // 방 잠금
    room.onMessage("Lock", (client, message: string) => {
        if (client.sessionId !== room.state.hostId) {
            client.send(ServerResponseWarning, `Lock FAILED: Only the host can lock/unlock room.`);
            return;
        }

        room.lock();
    });

    // 방 잠금 해제
    room.onMessage("Unlock", (client, message: string) => {
        if (client.sessionId !== room.state.hostId) {
            client.send(ServerResponseWarning, `Unlock FAILED: Only the host can lock/unlock room.`);
            return;
        }

        room.unlock();
    });

    // 방 공개
    room.onMessage("Show", (client, _) => {
        if (client.sessionId !== room.state.hostId) {
            client.send(ServerResponseWarning, `Open FAILED: Only the host can open/hide room.`);
            return;
        }

        room.setPrivate(false);
    });

    // 방 숨김
    room.onMessage("Hide", (client, _) => {
        if (client.sessionId !== room.state.hostId) {
            client.send(ServerResponseWarning, `Hide FAILED: Only the host can open/hide room.`);
            return;
        }

        room.setPrivate(true);
    });

    // 방 메타데이터 설정
    room.onMessage("Metadata-Set", (client, message: any) => {
        if (client.sessionId !== room.state.hostId) {
            client.send(ServerResponseWarning, `Metadata-Set FAILED: Only the host can set metadata of room.`);
            return;
        }

        room.setMetadata({
            ["roomTitle"]: message.roomTitle,
            ["password"]: message.password,
            ["maxClients"]: message.maxClients
        });

        room.maxClients = message.maxClients;
        room.broadcast("metadata", room.metadata);
    })

    // 방 메타데이터 가져오기
    room.onMessage("Metadata-Get", (client, message: any) => {
        client.send("metadata", room.metadata);
    })
}
//#endregion

//#region RPC
function SetRPCListener(room: GameRoom) {
    room.onMessage("RPC", (client, message) => {
        switch (message.target) {
            case "All":
                room.broadcast("RPC", message);
                break;
            case "Other":
                room.broadcast("RPC", message, { except: client });
                break;
            default:
                // 목표 세션 ID를 가진 클라이언트에게 메시지 전송
                for (let client of room.clients) {
                    if (client.sessionId === message.target) {
                        client.send("RPC", message);
                        return;
                    }
                }
                
                // 해당 세션 ID를 가진 클라이언트가 없다면
                client.send(ServerResponseWarning, `RPC FAILED: There is no client with that session ID.`);
                break;
        }
    });
}
//#endregion

//#region Chat
function SetChatListener(room: GameRoom) {
    room.onMessage("Chat", (client, message) => {
        room.broadcast("Chat", message, { except: client });
    });
}
//#endregion

//#region NetworkObject
function SetNetworkObjectListener(room: GameRoom) {
    // 네트워크 오브젝트 생성
    room.onMessage("Create", (client, message) => {
        let objectId = message.objectId;

        // ID 중복 검사
        if (room.state.Objects.get(objectId) != null)
        {
            client.send("Create-Fail", objectId);
            client.send(ServerResponseError, `Create FAILED: object ID ${objectId} is duplicated!`);
            return;
        }
        
        room.state.Objects.set(objectId, new NetworkObj);
        room.state.Transforms.set(objectId, "");
        room.state.Animations.set(objectId, "");
        room.state.Rigidbodies.set(objectId, "");
        
        let object = room.state.Objects.get(objectId);
        object.objectId = objectId;
        object.owner = client.sessionId;
        object.prefabName = message.prefabName;
        object.destroyOnOwnerLeave = message.destroyOnOwnerLeave;

        room.state.Players.get(client.sessionId).ownedObjects.push(objectId);

        client.send(ServerResponseLog, `Create Success: object ID ${objectId}`);
    })

    // 네트워크 오브젝트 파괴
    room.onMessage("Destroy", (client, message) => {
        // 오브젝트 존재 여부 검사
        if (!room.state.Objects.has(message.objectId)) {
            client.send(ServerResponseWarning, `Destroy FAILED: There is no object with object ID ${message.objectId}`);
            return;
        }
        
        let object = room.state.Objects.get(message.objectId)

        // 오브젝트 소유권 검사
        if (object.owner !== client.sessionId) {
            client.send(ServerResponseWarning, `Destroy FAILED: You do not own this object ${message.objectId}`);
            return;
        }

        // 오브젝트 제거
        room.state.Objects.delete(message.objectId);
        room.state.Transforms.delete(message.objectId);
        room.state.Animations.delete(message.objectId);
        room.state.Rigidbodies.delete(message.objectId);
        
        // 소유권 정리
        let ownedObjects = room.state.Players.get(client.sessionId).ownedObjects;
        let index = ownedObjects.indexOf(message.objectId);
        ownedObjects.splice(index, 1);
    })

    // 트랜스폼 동기화
    room.onMessage("Transform", (client, message) => {
        let object = room.state.Objects.get(message.objectId);

        // 오브젝트 null 검사
        if (object == null) {
            client.send(ServerResponseError, `Transform Update FAILED: object ${message.objectId} is NOT initialized.`);
            return;
        }
        
        // 소유권 검사
        if (object.owner !== client.sessionId) {
            client.send(ServerResponseWarning, `Transform Update FAILED: You do not own this object ${message.objectId}`);
            return;
        }

        // 상태 업데이트
        if (message.transform != room.state.Transforms.get(message.objectId)) {
            room.state.Transforms.set(message.objectId, message.transform);
        }
    });

    // 애니메이션 동기화
    room.onMessage("Animation", (client, message) => {
        let object = room.state.Objects.get(message.objectId);

        // 오브젝트 null 검사
        if (object == null) {
            client.send(ServerResponseError, `Animation Update FAILED: object ${message.objectId} is NOT initialized.`);
            return;
        }
        
        // 소유권 검사
        if (object.owner !== client.sessionId) {
            client.send(ServerResponseWarning, `Animation Update FAILED: You do not own this object ${message.objectId}`);
            return;
        }

        // 상태 업데이트
        if (message.animation != room.state.Animations.get(message.objectId)) {
            room.state.Animations.set(message.objectId, message.animation);
        }
    });

    // 강체 물리 동기화
    room.onMessage("Rigidbody", (client, message) => {
        let object = room.state.Objects.get(message.objectId);

        // 오브젝트 null 검사
        if (object == null) {
            client.send(ServerResponseError, `Rigidbody Update FAILED: object ${message.objectId} is NOT initialized.`);
            return;
        }
        
        // 소유권 검사
        if (object.owner !== client.sessionId) {
            client.send(ServerResponseWarning, `Rigidbody Update FAILED: You do not own this object ${message.objectId}`);
            return;
        }

        // 상태 업데이트
        if (message.animation != room.state.Animations.get(message.objectId)) {
            room.state.Rigidbodies.set(message.objectId, message.rigidbody);
        }
    });
}
//#endregion

//#region Authority
function SetAuthorityListener(room: GameRoom) {
    room.onMessage("Authority-Get", (client, message) => {
        let object = room.state.Objects.get(message.objectId);

        // 오브젝트 null 검사
        if (object == null) {
            client.send(ServerResponseError, `Authority-Get FAILED: object ${message.objectId} is NOT initialized.`);
            return;
        }

        // 권한 검사
        if (client.sessionId !== room.state.hostId) {
            if (object.owner !== "") {
                client.send(ServerResponseWarning, `Authority-Get FAILED: this object ${message.objectId} already owned by other client`);
                return;
            }
        }

        // 소유권 할당
        object.owner = client.sessionId;
    });

    room.onMessage("Authority-Release", (client, message) => {
        let object = room.state.Objects.get(message.objectId);
        
        // 오브젝트 null 검사
        if (object == null) {
            client.send(ServerResponseError, `Authority-Release FAILED: object ${message.objectId} is NOT initialized.`);
            return;
        }

        // 권한 검사
        if (client.sessionId !== room.state.hostId) {
            if (client.sessionId !== object.owner) {
                client.send(ServerResponseWarning, `Authority-Release FAILED: You do not own this object ${message.objectId}`);
                return;
            }
        }

        // 소유권 제거
        object.owner = "";
    });
}
//#endregion