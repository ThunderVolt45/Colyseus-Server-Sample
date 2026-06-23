# 클래스 다이어그램 문서 목록 (Class Diagrams Index)

이 문서는 **Colyseus-Server-Sample** 프로젝트의 클래스 구조를 분류별로 정리한 문서 모음의 목차입니다.
프로젝트는 크게 두 부분으로 구성됩니다.

- **서버 (TypeScript)** — `Server/src/` : 룸 로직과 권위(authoritative) 상태 스키마
- **클라이언트 (Unity C#)** — `Assets/Colyseus_Client/` : 매니저 / 네트워크 컴포넌트 / 데이터 구조

> 모든 다이어그램은 [Mermaid](https://mermaid.js.org/) 문법으로 작성되어 GitHub 및 Mermaid 지원 뷰어에서 렌더링됩니다.

---

## 문서 목록

| 문서 | 분류 | 내용 |
|------|------|------|
| [01-Server.md](01-Server.md) | 서버 (TypeScript) | `GameRoom` + 상태 스키마(`GameRoomState` / `Player` / `NetworkObj`), 메시지 핸들러 |
| [02-Client-Components.md](02-Client-Components.md) | 클라이언트 (Unity C#) | 네트워크 컴포넌트 계층 (`NetworkObject` / `NetworkComponent` 상속·합성) |
| [03-Client-Managers.md](03-Client-Managers.md) | 클라이언트 (Unity C#) | 매니저 계층 (7개 싱글톤) |
| [04-Client-Data.md](04-Client-Data.md) | 클라이언트 (Unity C#) | 데이터 / 메시지 / 보조 클래스 |
| [05-Overview.md](05-Overview.md) | 공통 | 전체 동작 요약 및 핵심 설계 패턴 |

---

## 다이어그램 범위

라이브러리 코드(`node_modules`, `Assets/Colyseus_Server/Runtime`의 Colyseus SDK)는 제외하고
**실제 프로젝트 코드만** 다이어그램에 포함했습니다.
