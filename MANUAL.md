# Manual

- [Manual](#manual)
- [Type](#type)
  - [ColyseusQuaternion](#colyseusquaternion)
    - [Constructor](#constructor)
    - [Properties](#properties)
    - [Methods](#methods)
  - [ColyseusVector3](#colyseusvector3)
    - [Constructor](#constructor-1)
    - [Properties](#properties-1)
    - [Methods](#methods-1)
- [Schema](#schema)
  - [ColyseusAnimation](#colyseusanimation)
    - [Properties](#properties-2)
  - [ColyseusRigidbody](#colyseusrigidbody)
    - [Properties](#properties-3)
  - [ColyseusTransform](#colyseustransform)
    - [Properties](#properties-4)
- [Component](#component)
  - [NetworkObject](#networkobject)
    - [Properties](#properties-5)
    - [Methods](#methods-2)
  - [NetworkComponent](#networkcomponent)
    - [Properties](#properties-6)
    - [Methods](#methods-3)
  - [NetworkAnimation](#networkanimation)
  - [NetworkRigidbody](#networkrigidbody)
    - [Properties](#properties-7)
  - [NetworkTransform](#networktransform)
    - [Properties](#properties-8)
- [Manager](#manager)
  - [NetworkManager](#networkmanager)
    - [Properties](#properties-9)
    - [Events](#events)
    - [Methods](#methods-4)
  - [NetworkTransformManager](#networktransformmanager)
  - [NetworkRigidbodyManager](#networkrigidbodymanager)
  - [NetworkAnimationManager](#networkanimationmanager)
  - [NetworkChatManager](#networkchatmanager)
    - [Events](#events-1)
    - [Methods](#methods-5)
  - [NetworkRPCManager](#networkrpcmanager)
    - [Methods](#methods-6)

# Type

## ColyseusQuaternion

Colyseus를 통해 동기화할 수 있도록 만든 Quaternion 클래스입니다.

네트워크 동기화 비용을 줄이기 위해 내부적으로 값을 정수 형태로 변환하여 저장합니다.

### Constructor

|Parameter|Description|
|-|-|
|(Quaternion)quaternion, (int)precision = 3|Quaternion 값과 정밀도 값(기본 3)을 받아 ``ColyseusQuaternion``를 생성합니다.|

### Properties

|Property|Description|
|-|-|
|x|정수형으로 변환된 x값.|
|y|정수형으로 변환된 y값.|
|z|정수형으로 변환된 z값.|
|z|정수형으로 변환된 w값.|
|prec|정수형과 실수형을 변환하는데 사용할 정밀도 값|

### Methods

|Method|Parameter|Description|
|-|-|-|
|ToQuaternion|-|``ColyseusQuaternion``를 ``UnityEngine.Quaternion``로 변환합니다.|

## ColyseusVector3

Colyseus를 통해 동기화할 수 있도록 만든 Vector3 클래스입니다.

네트워크 동기화 비용을 줄이기 위해 내부적으로 값을 정수 형태로 변환하여 저장합니다.

### Constructor

|Parameter|Description|
|-|-|
|(Vector3)vector3, (int)precision = 3|Vector3 값과 정밀도 값(기본 3)을 받아 ``ColyseusVector3``를 생성합니다.|

### Properties

|Property|Description|
|-|-|
|x|정수형으로 변환된 x값.|
|y|정수형으로 변환된 y값.|
|z|정수형으로 변환된 z값.|
|prec|정수형과 실수형을 변환하는데 사용할 정밀도 값|

### Methods

|Method|Parameter|Description|
|-|-|-|
|ToVector3|-|``ColyseusVector3``를 ``UnityEngine.Vector3``로 변환합니다.|

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
|angularvelocity|Rigidbody의 각속도 값|

## ColyseusTransform

Server와 Client 사이에서 Transform 정보를 전달하는데 사용되는 Schema입니다.

### Properties

|Property|Description|
|-|-|
|position|Transform의 위치 값|
|rotation|Transform의 회전 값|
|scale|Transform의 크기 값|

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
|GetNetworkComponent|where T : NetworkComponent|``NetworkObject``에 부착된 ``NetworkComponent``를 가져옵니다.|

## NetworkComponent

``NetworkComponent``는 Colyseus를 통해 동기화되는 Component의 부모 클래스입니다.

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
|SetEnable|(bool)active|``NetworkComponent``를 활성화 합니다.|
|GetNetworkComponent|where T : NetworkComponent|``NetworkObject``에 부착된 ``NetworkComponent``를 가져옵니다.|

## NetworkAnimation

``NetworkAnimation``은 Colyseus를 통해 Animation을 동기화하고자 하는 GameObject에 부착해 사용하는 Component입니다.

GameObject 내의 ``animator``를 찾아 ``animator`` 내의 각 파라미터를 직렬화해 Server에 전송하거나 Server에서 가져온 값을 역직렬화해 ``animator``를 갱신하는 방식으로 Animation를 동기화하므로 Animation를 설계할 때 이 점을 고려해야 정상적으로 동기화될 수 있습니다.

## NetworkRigidbody

``NetworkRigidbody``은 Colyseus를 통해 Rigidbody을 동기화하고자 하는 GameObject에 부착해 사용하는 Component입니다.

**동기화가 매우 비싼 편이므로 필요한 부분에만 사용하는 것을 권장합니다.**

**서로 다른 Client가 소유하는 Rigidbody 간의 상호작용은 테스트되지 않았습니다.**

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
|PositionPrecision|Transfrom의 위치 정밀도 (권장값 3)|
|RotationPrecision|Transfrom의 회전 정밀도 (권장값 2)|
|ScalePrecision|Transfrom의 크기 정밀도 (권장값 2)|

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
|serverState|현재 Server의 연결 상태|
|room|Colyseus Room|
|NetworkObjects|네트워크 상에서 추적되고 있는 ``NetworkObject``들의 Dictionary|

### Events

|Event|Parameter|Description|
|-|-|-|
|stateCallbackEvent|``StateCallbackStrategy<GameRoomState>``|Room 내의 State가 추가/변경/삭제되었을 때 호출되는 Event|
|roomInitializeEvent|``ColyseusRoom<GameRoomState>``|Room이 초기화 되었을 때 호출되는 Event|

### Methods

|Method|Parameter|Description|
|-|-|-|
|JoinOrCreateRoom|-|이미 존재하는 Room에 접속하거나 새 Room을 생성함|
|LeaveRoom|-|Room과의 연결을 중단함|
|NetworkInstantiate|(string)prefabName, (bool)destroyOnOwnerLeave|``Resources`` 폴더 내에 존재하는 ``NetworkObject`` Prefab을 네트워크를 통해 동기화되는 오브젝트로 생성함|
|NetworkDestroy|``NetworkObject``|네트워크를 통해 동기화되고 있는 ``NetworkObject``를 네트워크 상에서 파괴함|

## NetworkTransformManager

Room 내 State 추가/변경/삭제를 추적해 ``NetworkTransform``이 부착된 ``NetworkObject``의 Transform를 동기화하는 Manager입니다.

## NetworkRigidbodyManager

Room 내 State 추가/변경/삭제를 추적해 ``NetworkRigidbody``이 부착된 ``NetworkObject``의 Rigidbody를 동기화하는 Manager입니다.

## NetworkAnimationManager

Room 내 State 추가/변경/삭제를 추적해 ``NetworkAnimation``이 부착된 ``NetworkObject``의 Animation를 동기화하는 Manager입니다.

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
|AddRPCFunction|``RPCFunction``|Room 내에서 사용할 RPC를 등록하는 메소드|
|AddRPCFunction|(string)RPCName, ``RPCFunction``|Room 내에서 사용할 RPC를 등록하는 메소드|
|SendRPC|(string)functionName, ``RPCParameter``, ``RPCTarget``|``RPCTarget``의 대상 Client의 RPC를 호출하는 메소드|
|SendRPC|(string)functionName, ``RPCParameter``, (string)targetSessionId|targetSessionId를 갖는 Client의 RPC를 호출하는 메소드|