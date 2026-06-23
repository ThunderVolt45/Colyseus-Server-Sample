# 전체 동작 요약 (Overview)

서버와 클라이언트의 역할 분담 및 핵심 설계 패턴 요약입니다.

> [문서 목록으로 돌아가기](00-Index.md)

---

## 서버 · 클라이언트 역할 비교

| 영역 | 서버(권위) | 클라이언트 |
|------|-----------|-----------|
| **상태 보관** | `GameRoomState` (Players / Objects / Transforms / Animations / Rigidbodies 맵) | 자동 생성된 동일 스키마를 디코딩 |
| **객체 생성/소유권** | `Create` / `Destroy` / `Authority-Get` / `Authority-Release` 메시지 검증 | `NetworkManager.NetworkInstantiate()` |
| **동기화** | 소유자 검증 후 `Transforms` / `Animations` / `Rigidbodies` 맵 갱신 | `NetworkTransform/Animation/Rigidbody` 컴포넌트가 `IsMine`이면 Broadcast, 아니면 보간 적용 |
| **호스트 마이그레이션** | `onLeave`에서 호스트 이탈 시 랜덤 재지정 | `host` 메시지 수신 |

---

## 핵심 설계 패턴

- **싱글톤 매니저**
  7개 매니저가 각각 한 가지 책임(연결 / 플레이어 / 트랜스폼 / 애니메이션 / 리지드바디 / 채팅 / RPC)을 담당합니다.

- **컴포넌트 합성(Composition)**
  `NetworkObject`(정체성) + `NetworkComponent` 파생들(동기화 동작) 구조이며, `[RequireComponent]`로 의존을 강제합니다.

- **이벤트 구독(느슨한 결합)**
  각 매니저가 `NetworkManager`의 `stateCallbackEvent` / `roomInitializeEvent`를 구독합니다.

- **권위 모델(Server-authoritative)**
  서버가 소유권(`owner`)을 검증하고, 클라이언트는 `IsMine`으로 송신/수신을 분기합니다.

---

## 관련 문서

- [01-Server.md](01-Server.md) — 서버 측 클래스 다이어그램
- [02-Client-Components.md](02-Client-Components.md) — 네트워크 컴포넌트 계층
- [03-Client-Managers.md](03-Client-Managers.md) — 매니저 계층
- [04-Client-Data.md](04-Client-Data.md) — 데이터 / 메시지 / 보조 클래스
