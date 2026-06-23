# Manual

이 문서는 **Colyseus-Server-Sample**의 API 레퍼런스이자 사용 가이드입니다.
클래스 구조를 시각적으로 보고 싶다면 [`Docs/` 클래스 다이어그램](Docs/00-Index.md)을 함께 참고하세요.

- [Manual](#manual)
- [Overview](#overview)
- [Usage](#usage)
  - [네트워크 오브젝트 생성 / 파괴](#네트워크-오브젝트-생성--파괴)
  - [Transform / Animation / Rigidbody 동기화](#transform--animation--rigidbody-동기화)
  - [채팅 전송 / 수신](#채팅-전송--수신)
  - [RPC 등록 / 호출](#rpc-등록--호출)
  - [소유권(Authority) 이전](#소유권authority-이전)
  - [커스텀 NetworkComponent 작성](#커스텀-networkcomponent-작성)
- [Type](#type)
  - [ColyseusQuaternion](#colyseusquaternion)
  - [ColyseusVector3](#colyseusvector3)
- [Schema](#schema)
  - [ColyseusAnimation](#colyseusanimation)
  - [ColyseusRigidbody](#colyseusrigidbody)
  - [ColyseusTransform](#colyseustransform)
- [Message](#message)
  - [TransformMessage / AnimationMessage / RigidbodyMessage](#transformmessage--animationmessage--rigidbodymessage)
- [Component](#component)
  - [NetworkObject](#networkobject)
  - [NetworkComponent](#networkcomponent)
  - [NetworkPlayer](#networkplayer)
  - [NetworkAnimation](#networkanimation)
  - [NetworkRigidbody](#networkrigidbody)
  - [NetworkTransform](#networktransform)
  - [AuthorityShower](#authorityshower)
- [Manager](#manager)
  - [NetworkManager](#networkmanager)
  - [NetworkPlayerManager](#networkplayermanager)
  - [NetworkTransformManager](#networktransformmanager)
  - [NetworkRigidbodyManager](#networkrigidbodymanager)
  - [NetworkAnimationManager](#networkanimationmanager)
  - [NetworkChatManager](#networkchatmanager)
  - [NetworkRPCManager](#networkrpcmanager)
- [Data Types](#data-types)
  - [ServerState](#serverstate)
  - [RoomData / AvailableRoom](#roomdata--availableroom)
  - [ChatMessage](#chatmessage)
  - [RPC / RPCParameter / RPCTarget](#rpc--rpcparameter--rpctarget)
- [Server](#server)
  - [Authoritative Model](#authoritative-model)
  - [GameRoom Messages](#gameroom-messages)
  - [Server Schema](#server-schema)

# Overview

이 프로젝트는 **서버가 권위(authoritative) 상태를 보유**하고, Unity 클라이언트가 그 상태를 구독·동기화하는 구조입니다.

기본 동작 흐름은 다음과 같습니다.

1. `NetworkManager`가 Colyseus 서버에 접속해 Room을 생성/참가합니다. (`JoinOrCreateRoom`)
2. Room 초기화 시 각 Manager가 `stateCallbackEvent` / `roomInitializeEvent`를 구독해 동기화 Listener를 등록합니다.
3. `NetworkInstantiate`로 `NetworkObject`를 생성하면 서버 State(`Objects`)에 등록되고, 모든 클라이언트에 복제됩니다.
4. 오브젝트에 부착된 `NetworkTransform` / `NetworkAnimation` / `NetworkRigidbody`가 **소유자(`IsMine`)이면 값을 Broadcast**하고, 아니면 서버 State를 받아 보간(lerp)하여 반영합니다.

> 전체 동작 요약과 설계 패턴은 [Docs/05-Overview.md](Docs/05-Overview.md)를, 클래스 관계는 [Docs/00-Index.md](Docs/00-Index.md)를 참고하세요.

# Usage

> 아래 예제는 `Colyseus_Client` 네임스페이스를 `using` 한 상태를 가정합니다.

## 네트워크 오브젝트 생성 / 파괴

`NetworkInstantiate`에 넘기는 이름은 **`Resources` 폴더 내 Prefab 이름**이어야 하며, 해당 Prefab에는 `NetworkObject`가 부착되어 있어야 합니다.

```csharp
// 생성 (소유자가 떠날 때 자동 파괴: destroyOnOwnerLeave 기본 true)
NetworkObject obj = await NetworkManager.Instance.NetworkInstantiate("Cube");

// 소유자가 떠나도 유지되는 오브젝트
NetworkObject persistent = await NetworkManager.Instance.NetworkInstantiate("Cube", destroyOnOwnerLeave: false);

// 파괴 (소유자만 가능)
NetworkManager.Instance.NetworkDestroy(obj);
```

## Transform / Animation / Rigidbody 동기화

동기화하려는 GameObject(Prefab)에 `NetworkObject`와 함께 원하는 동기화 컴포넌트를 부착하기만 하면 됩니다.
별도의 코드 없이 자동으로 송수신됩니다.

| 컴포넌트 | 부착 시 동기화되는 것 | 필요 컴포넌트 |
|----------|----------------------|---------------|
| `NetworkTransform` | 위치 / 회전 / 크기 | - |
| `NetworkAnimation` | Animator 파라미터 | `Animator` |
| `NetworkRigidbody` | 물리 상태(속도/질량 등) | `Rigidbody` |

> 소유자(`IsMine == true`)의 값이 서버로 전송되어 나머지 클라이언트에 반영됩니다.

## 채팅 전송 / 수신

```csharp
// 전송
NetworkChatManager.Instance.Send("Hello, world!");

// 수신 (UnityEvent 구독)
NetworkChatManager.Instance.ChatEvent.AddListener(msg =>
{
    Debug.Log($"[{msg.nickname}] {msg.message} (mine: {msg.IsMine()})");
});
```

## RPC 등록 / 호출

```csharp
// 1) 호출될 함수를 등록 (메서드 이름이 키로 사용됨)
NetworkRPCManager.Instance.AddRPCFunction(MyRpc);

void MyRpc(RPCParameter param)
{
    Debug.Log(param.parameters[0]);
}

// 2) 호출 — 대상 지정
NetworkRPCManager.Instance.SendRPC("MyRpc", new RPCParameter("payload"), RPCTarget.All);    // 모두에게
NetworkRPCManager.Instance.SendRPC("MyRpc", new RPCParameter("payload"), RPCTarget.Other);  // 나를 제외한 모두
NetworkRPCManager.Instance.SendRPC("MyRpc", new RPCParameter("payload"), targetSessionId);  // 특정 세션
```

## 소유권(Authority) 이전

`NetworkObj.owner`가 빈 문자열이거나 자신이 Host이면 소유권을 가져올 수 있습니다.
클라이언트에는 별도 래퍼가 없으므로 Room 메시지로 직접 요청합니다.

```csharp
// 소유권 획득 / 반환
NetworkManager.Instance.room.Send("Authority-Get", new { objectId = obj.ObjectId });
NetworkManager.Instance.room.Send("Authority-Release", new { objectId = obj.ObjectId });
```

> 서버 측 검증 규칙은 [Server > GameRoom Messages](#gameroom-messages)를 참고하세요.

## 커스텀 NetworkComponent 작성

`NetworkComponent`를 상속하면 `IsMine` / `ObjectId` / `SessionId` 등에 바로 접근할 수 있습니다.

```csharp
public class MyComponent : NetworkComponent
{
    private void Update()
    {
        if (!IsMine) return;   // 소유자만 로직 실행

        // ... 동기화하고 싶은 값 처리 ...
    }
}
```

# Type

## ColyseusQuaternion

Colyseus를 통해 동기화할 수 있도록 만든 Quaternion 클래스입니다.

네트워크 동기화 비용을 줄이기 위해 내부적으로 값을 **정수(`int`)** 형태로 변환하여 저장합니다.
(`prec` 자릿수만큼 10의 거듭제곱을 곱한 뒤 정수로 양자화)

### Constructor

|Parameter|Description|
|-|-|
|(Quaternion)quaternion, (int)precision = 3|Quaternion 값과 정밀도 값(기본 3)을 받아 ``ColyseusQuaternion``를 생성합니다.|

### Properties

|Property|Type|Description|
|-|-|-|
|x|int|정수형으로 변환된 x값.|
|y|int|정수형으로 변환된 y값.|
|z|int|정수형으로 변환된 z값.|
|w|int|정수형으로 변환된 w값.|
|prec|int|정수형과 실수형을 변환하는데 사용할 정밀도 값.|

### Methods

|Method|Parameter|Description|
|-|-|-|
|ToQuaternion|-|``ColyseusQuaternion``를 ``UnityEngine.Quaternion``로 변환합니다.|

> `ColyseusQuaternion`은 `UnityEngine.Quaternion`으로의 암시적(implicit) 변환 연산자를 제공합니다.

## ColyseusVector3

Colyseus를 통해 동기화할 수 있도록 만든 Vector3 클래스입니다.

네트워크 동기화 비용을 줄이기 위해 내부적으로 값을 **정수(`long`)** 형태로 변환하여 저장합니다.

### Constructor

|Parameter|Description|
|-|-|
|(Vector3)vector3, (int)precision = 3|Vector3 값과 정밀도 값(기본 3)을 받아 ``ColyseusVector3``를 생성합니다.|

### Properties

|Property|Type|Description|
|-|-|-|
|x|long|정수형으로 변환된 x값.|
|y|long|정수형으로 변환된 y값.|
|z|long|정수형으로 변환된 z값.|
|prec|int|정수형과 실수형을 변환하는데 사용할 정밀도 값.|

### Methods

|Method|Parameter|Description|
|-|-|-|
|ToVector3|-|``ColyseusVector3``를 ``UnityEngine.Vector3``로 변환합니다.|

> `ColyseusVector3`는 `UnityEngine.Vector3`로의 암시적(implicit) 변환 연산자를 제공합니다.

# Schema

## ColyseusAnimation

Server와 Client 사이에서 Animation 정보를 전달하는데 사용되는 Schema입니다.

직접 byte 값을 다루는 형태로 데이터를 전달할 수 있다면 좋겠으나 colyseus에서 지원하는 자료형에 byte[]는 없는 관계로 약간의 오버헤드를 감수하고 그냥 string 값에 json 형태로 값을 담아 전송합니다.

### Properties

|Property|Description|
|-|-|
|param|각 애니메이션 파라미터의 json 직렬화 값이 담긴 배열|

## ColyseusRigidbody

Server와 Client 사이에서 Rigidbody 정보를 전달하는데 사용되는 Schema입니다.

### Properties

|Property|Description|
|-|-|
|mass|Rigidbody의 질량 값|
|drag|Rigidbody가 힘에 의해 움직일 때의 공기 저항 값|
|angularDrag|Rigidbody가 토크에 의해 회전할 때의 공기 저항 값|
|gravity|Rigidbody가 중력의 영향을 받을지 여부|
|kinematic|Rigidbody가 물리엔진의 제어를 받을지 여부|
|velocity|Rigidbody의 선속도 값|
|angularVelocity|Rigidbody의 각속도 값|

## ColyseusTransform

Server와 Client 사이에서 Transform 정보를 전달하는데 사용되는 Schema입니다.

### Properties

|Property|Description|
|-|-|
|position|Transform의 위치 값 (``ColyseusVector3``)|
|rotation|Transform의 회전 값 (``ColyseusQuaternion``)|
|scale|Transform의 크기 값 (``ColyseusVector3``)|

# Message

## TransformMessage / AnimationMessage / RigidbodyMessage

각 Manager가 동기화 값을 서버로 전송할 때 사용하는 메시지 래퍼입니다.
`objectId`와 직렬화된 상태 문자열을 함께 담아 전송합니다.

|Class|Properties|Description|
|-|-|-|
|TransformMessage|objectId, transform|Transform 동기화 값을 전송 ("Transform" 메시지)|
|AnimationMessage|objectId, animation|Animation 동기화 값을 전송 ("Animation" 메시지)|
|RigidbodyMessage|objectId, rigidbody|Rigidbody 동기화 값을 전송 ("Rigidbody" 메시지)|

# Component

## NetworkObject

``NetworkObject``는 네트워크를 통해 동기화되는 Object의 기본 단위로 Colyseus를 통해 동기화할 오브젝트에 반드시 부착되어있어야 합니다.

### Properties

|Property|Description|
|-|-|
|ObjectId|해당 오브젝트가 네트워크 상에서 갖는 ID 값|
|SessionId|해당 오브젝트를 소유하는 Client의 세션 ID 값|
|PrefabName|해당 오브젝트의 Prefab 이름|
|IsMine|해당 오브젝트를 Local Client가 소유 중인지 여부|

### Methods

|Method|Parameter|Description|
|-|-|-|
|SetObjectId|(string)objectId|``NetworkObject``의 ID를 설정합니다.|
|Initialize|(string)prefabName, (string)sessionId, (bool)isMine|``NetworkObject``를 초기화합니다.|
|GetNetworkComponent|where T : NetworkComponent|``NetworkObject``에 부착된 ``NetworkComponent``를 가져옵니다. (타입별로 캐싱됨)|

## NetworkComponent

``NetworkComponent``는 Colyseus를 통해 동기화되는 Component의 부모 클래스입니다.
``[RequireComponent(typeof(NetworkObject))]``로 ``NetworkObject`` 부착을 강제합니다.

### Properties

|Property|Description|
|-|-|
|NetworkObject|``NetworkComponent``가 부착된 GameObject의 ``NetworkObject``|
|IsMine|``NetworkObject.IsMine``|
|SessionId|``NetworkObject.SessionId``|
|ObjectId|``NetworkObject.ObjectId``|

### Methods

|Method|Parameter|Description|
|-|-|-|
|SetEnable|(bool)active|``NetworkComponent``를 활성화/비활성화 합니다.|
|GetNetworkComponent|where T : NetworkComponent|``NetworkObject``에 부착된 ``NetworkComponent``를 가져옵니다.|

## NetworkPlayer

``NetworkPlayer``는 플레이어 오브젝트를 나타내는 ``NetworkComponent``입니다.
플레이어의 닉네임을 보유하며, `NetworkPlayerManager`가 로컬 플레이어를 생성할 때 사용합니다.

### Properties

|Property|Description|
|-|-|
|Nickname|플레이어의 닉네임|

### Methods

|Method|Parameter|Description|
|-|-|-|
|Initialize|(string)nickname|닉네임을 설정하며 초기화합니다.|
|SetNickname|(string)nickname|닉네임을 변경합니다.|

## NetworkAnimation

``NetworkAnimation``은 Colyseus를 통해 Animation을 동기화하고자 하는 GameObject에 부착해 사용하는 Component입니다.

GameObject 내의 ``animator``를 찾아 ``animator`` 내의 각 파라미터를 직렬화해 Server에 전송하거나 Server에서 가져온 값을 역직렬화해 ``animator``를 갱신하는 방식으로 Animation를 동기화하므로 Animation를 설계할 때 이 점을 고려해야 정상적으로 동기화될 수 있습니다.

> ``[RequireComponent(typeof(Animator))]`` — ``Animator``가 반드시 필요합니다.

## NetworkRigidbody

``NetworkRigidbody``은 Colyseus를 통해 Rigidbody을 동기화하고자 하는 GameObject에 부착해 사용하는 Component입니다.

**동기화가 매우 비싼 편이므로 필요한 부분에만 사용하는 것을 권장합니다.**

**서로 다른 Client가 소유하는 Rigidbody 간의 상호작용은 테스트되지 않았습니다.**

> ``[RequireComponent(typeof(Rigidbody))]`` — ``Rigidbody``가 반드시 필요합니다.

### Properties

|Property|Description|
|-|-|
|networkRigidbody|네트워크 내에서 저장된 Rigidbody 값|
|lerpSpeed|Server의 State 값과 Client의 State 값을 보간할 때 사용하는 값|
|VelocityPrecision|Rigidbody의 선속도 정밀도 (권장값 3)|
|AngularVelocityPrecision|Rigidbody의 각속도 정밀도 (권장값 3)|

## NetworkTransform

``NetworkTransform``은 Colyseus를 통해 Transform을 동기화하고자 하는 GameObject에 부착해 사용하는 Component입니다.

### Properties

|Property|Description|
|-|-|
|networkTransform|네트워크 내에서 저장된 Transform 값|
|lerpSpeed|Server의 State 값과 Client의 State 값을 보간할 때 사용하는 값|
|PositionPrecision|Transform의 위치 정밀도 (권장값 3)|
|RotationPrecision|Transform의 회전 정밀도 (권장값 2)|
|ScalePrecision|Transform의 크기 정밀도 (권장값 2)|

## AuthorityShower

``AuthorityShower``는 오브젝트의 소유권 여부를 시각화하는 보조 Component입니다.
``NetworkObject.IsMine`` 값에 따라 ``MeshRenderer``의 Material을 교체합니다.

> ``[RequireComponent(typeof(MeshRenderer))]``, ``[RequireComponent(typeof(NetworkObject))]``

### Properties

|Property|Description|
|-|-|
|materialMine|로컬 Client가 소유 중일 때 표시할 Material|
|materialOther|다른 Client가 소유 중일 때 표시할 Material|

# Manager

## NetworkManager

Colyseus Server에 접속해 Room을 생성/참가한 뒤 Server와의 동기화에 필요한 모든 Listener의 초기화를 수행하는 Manager입니다.

### Properties

|Property|Description|
|-|-|
|Instance|``NetworkManager`` 싱글톤 인스턴스|
|ServerName|Server에 연결할 때 사용할 Room 종류|
|URL|접속할 Server의 URL|
|AuthKey|Server에 연결할 때 사용할 인증 키|
|SessionId|현재 Client의 Session ID|
|IsHost|현재 Client가 Room의 Host인지 여부|
|patchRate|Room을 생성할 때 사용할 네트워크 갱신 주기|
|maxClient|Room을 생성할 때 Room의 인원 수 제한|
|serverState|현재 Server의 연결 상태 (``ServerState``)|
|room|Colyseus Room (``ColyseusRoom<GameRoomState>``)|
|NetworkObjects|네트워크 상에서 추적되고 있는 ``NetworkObject``들의 Dictionary|

### Events

|Event|Parameter|Description|
|-|-|-|
|stateCallbackEvent|``StateCallbackStrategy<GameRoomState>``|Room 내의 State가 추가/변경/삭제되었을 때 호출되는 Event|
|roomInitializeEvent|``ColyseusRoom<GameRoomState>``|Room이 초기화 되었을 때 호출되는 Event|

### Methods

|Method|Parameter|Description|
|-|-|-|
|JoinOrCreateRoom|-|이미 존재하는 Room에 접속하거나 새 Room을 생성함. ``Awaitable<bool>`` 반환|
|LeaveRoom|-|Room과의 연결을 중단함|
|NetworkInstantiate|(string)prefabName, (bool)destroyOnOwnerLeave = true|``Resources`` 폴더 내에 존재하는 ``NetworkObject`` Prefab을 네트워크를 통해 동기화되는 오브젝트로 생성함. ``Task<NetworkObject>`` 반환|
|NetworkDestroy|``NetworkObject``|네트워크를 통해 동기화되고 있는 ``NetworkObject``를 네트워크 상에서 파괴함|

## NetworkPlayerManager

Room 내 ``Players`` State를 추적해 플레이어의 추가/변경/제거를 처리하고, 로컬 Client의 플레이어 오브젝트를 생성하는 Manager입니다.

### Properties

|Property|Description|
|-|-|
|Instance|``NetworkPlayerManager`` 싱글톤 인스턴스|
|PlayerData|Room 참가 시 서버로 전송할 로컬 플레이어 데이터 (``Player``)|
|players|Room 내 플레이어 목록|
|LocalPlayer|로컬 Client가 소유한 ``NetworkPlayer``|
|defaultPlayerPrefab|로컬 플레이어 생성 시 사용할 기본 Prefab|
|generatePosition|플레이어 생성 위치 후보|

### Methods

|Method|Parameter|Description|
|-|-|-|
|PlayerListener|``StateCallbackStrategy<GameRoomState>``|``Players`` State의 추가/변경/제거 콜백을 등록함|
|OnAddPlayer|(string)key, ``Player``|플레이어 추가 시 호출. 본인이면 로컬 플레이어를 생성함|
|OnChangePlayer|(string)key, ``Player``|플레이어 변경 시 호출 (닉네임 등 갱신)|
|OnRemovePlayer|(string)key, ``Player``|플레이어 제거 시 호출|

## NetworkTransformManager

Room 내 State 추가/변경/삭제를 추적해 ``NetworkTransform``이 부착된 ``NetworkObject``의 Transform를 동기화하는 Manager입니다.

|Method|Parameter|Description|
|-|-|-|
|BroadcastTransform|(string)objectId, ``ColyseusTransform``|Transform 값을 서버로 전송함|

## NetworkRigidbodyManager

Room 내 State 추가/변경/삭제를 추적해 ``NetworkRigidbody``이 부착된 ``NetworkObject``의 Rigidbody를 동기화하는 Manager입니다.

|Method|Parameter|Description|
|-|-|-|
|BroadcastRidgebody|(string)objectId, ``ColyseusRigidbody``|Rigidbody 값을 서버로 전송함|

## NetworkAnimationManager

Room 내 State 추가/변경/삭제를 추적해 ``NetworkAnimation``이 부착된 ``NetworkObject``의 Animation를 동기화하는 Manager입니다.

|Method|Parameter|Description|
|-|-|-|
|BroadcastAnimation|(string)objectId, ``ColyseusAnimation``|Animation 값을 서버로 전송함|

## NetworkChatManager

Room에 "Chat" Message를 송/수신하는 Manager입니다.

### Events

|Event|Parameter|Description|
|-|-|-|
|ChatEvent|``ChatMessage``|Room에서 "Chat" Message를 수신받았을 때 호출되는 이벤트|

### Methods

|Method|Parameter|Description|
|-|-|-|
|Send|(string)message|parameter로 받은 문자열로 ``ChatMessage``를 생성해 Room에 전송하는 메소드|

## NetworkRPCManager

Room의 Message를 통해 다른 Client에 RPC를 호출할 수 있는 기능을 제공하는 Manager 입니다.

### Methods

|Method|Parameter|Description|
|-|-|-|
|AddRPCFunction|``RPCFunction``|Room 내에서 사용할 RPC를 등록하는 메소드 (메서드 이름이 키)|
|AddRPCFunction|(string)RPCName, ``RPCFunction``|이름을 직접 지정해 RPC를 등록하는 메소드|
|SendRPC|(string)functionName, ``RPCParameter``, ``RPCTarget`` = Other|``RPCTarget``의 대상 Client의 RPC를 호출하는 메소드|
|SendRPC|(string)functionName, ``RPCParameter``, (string)targetSessionId|targetSessionId를 갖는 Client의 RPC를 호출하는 메소드|

# Data Types

## ServerState

`NetworkManager.serverState`가 갖는 서버 연결 상태 enum입니다.

|Value|Description|
|-|-|
|Waiting|초기 대기 상태|
|Connecting|서버 접속 시도 중|
|Connected|접속 완료 (SessionId 수신됨)|
|Disconnected|연결 종료됨|

## RoomData / AvailableRoom

방의 메타데이터를 표현하는 클래스입니다.

`RoomData` Properties:

|Property|Description|
|-|-|
|roomTitle|방 제목|
|password|방 비밀번호|
|maxClients|최대 인원 수|
|roomTag|방 태그|

`AvailableRoom`은 `ColyseusRoomAvailable`을 상속하며, `metadata`(`RoomData`)를 포함해 룸 목록 조회 시 사용합니다.

## ChatMessage

채팅 메시지를 표현하는 클래스입니다.

### Properties

|Property|Description|
|-|-|
|sessionId|메시지를 보낸 Client의 Session ID|
|nickname|보낸 사람의 닉네임|
|time|전송 시각|
|message|메시지 본문|

### Methods

|Method|Parameter|Description|
|-|-|-|
|IsMine|-|이 메시지를 로컬 Client가 보냈는지 여부를 반환|

> `sessionId`가 `"SYSTEM"`인 메시지는 시스템 알림(입장/퇴장/RPC 로그 등)으로 사용됩니다.

## RPC / RPCParameter / RPCTarget

RPC 호출에 사용되는 타입입니다.

`RPC` Properties:

|Property|Description|
|-|-|
|functionName|호출할 함수 이름 (등록된 RPC의 키)|
|target|호출 대상 (`"All"` / `"Other"` / 특정 sessionId)|
|parameter|호출 시 전달할 ``RPCParameter``|

`RPCParameter` Properties:

|Property|Description|
|-|-|
|parameters|호출 함수에 전달할 인자 배열 (`object[]`)|

`RPCTarget` enum:

|Value|Description|
|-|-|
|Other|자신을 제외한 모든 Client|
|All|자신을 포함한 모든 Client|

# Server

서버는 `Server/src/`에 TypeScript로 작성되어 있으며, `GameRoom`이 권위 상태(`GameRoomState`)를 보유합니다.
자세한 클래스 구조는 [Docs/01-Server.md](Docs/01-Server.md)를 참고하세요.

## Authoritative Model

- 모든 동기화 메시지(`Transform`/`Animation`/`Rigidbody`)는 **서버가 소유권(`owner`)을 검증**한 뒤에만 State를 갱신합니다.
- 소유자가 아닌 Client가 갱신을 시도하면 `Server-Response-Warning` 메시지가 반환됩니다.
- Host가 떠나면 `onLeave`에서 남은 Client 중 무작위로 새 Host가 지정되고, 해당 Client에 `host` 메시지가 전송됩니다.

## GameRoom Messages

클라이언트가 `room.Send(type, payload)`로 보낼 수 있는 메시지 목록입니다.

|Group|Type|Description|
|-|-|-|
|Room Control|Kick|(Host) 특정 sessionId의 Client를 강퇴|
|Room Control|Lock / Unlock|(Host) 방 잠금 / 해제|
|Room Control|Show / Hide|(Host) 방 공개 / 비공개|
|Room Control|Metadata-Set / Metadata-Get|(Host) 방 메타데이터 설정 / 조회|
|Network Object|Create / Destroy|네트워크 오브젝트 생성 / 파괴 (소유권 검증)|
|Network Object|Transform / Animation / Rigidbody|상태 동기화 (소유자만 가능)|
|Authority|Authority-Get / Authority-Release|오브젝트 소유권 획득 / 반환|
|RPC|RPC|`All`/`Other`/특정 세션 대상 원격 호출|
|Chat|Chat|다른 Client에게 채팅 브로드캐스트|

서버가 클라이언트로 보내는 응답 메시지:

|Type|Description|
|-|-|
|Server-Response-Log|정보 로그|
|Server-Response-Warning|경고 (권한 부족 등)|
|Server-Response-Error|오류 (오브젝트 미초기화 등)|
|player|접속 완료 시 sessionId 전달|
|host|Host로 지정되었을 때 전달|
|Create-Fail|오브젝트 생성 실패 (ID 중복) 시 전달|

## Server Schema

`GameRoomState` (권위 상태):

|Property|Type|Description|
|-|-|-|
|hostId|string|현재 Host의 sessionId|
|Players|MapSchema\<Player\>|참가자 목록|
|Objects|MapSchema\<NetworkObj\>|네트워크 오브젝트 목록|
|Transforms|MapSchema\<string\>|objectId별 직렬화된 Transform|
|Animations|MapSchema\<string\>|objectId별 직렬화된 Animation|
|Rigidbodies|MapSchema\<string\>|objectId별 직렬화된 Rigidbody|

`Player`:

|Property|Type|Description|
|-|-|-|
|nickname|string|닉네임|
|metadata|string|부가 정보|
|isHost|bool|Host 여부|
|connected|bool|연결 상태 (재접속 대기 중 false)|
|ownedObjects|ArraySchema\<string\>|소유한 오브젝트 ID 목록|

`NetworkObj`:

|Property|Type|Description|
|-|-|-|
|objectId|string|오브젝트 ID|
|prefabName|string|Prefab 이름|
|owner|string|소유자 sessionId (빈 문자열이면 무소유)|
|destroyOnOwnerLeave|bool|소유자 이탈 시 자동 파괴 여부|
