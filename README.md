# colyseus-server-sample

- [colyseus-server-sample](#colyseus-server-sample)
  - [Introduction](#introduction)
  - [How to Install](#how-to-install)
  - [Quick Start](#quick-start)
  - [Manual](#manual)
  - [License](#license)

## Introduction

[Colyseus](https://github.com/colyseus) 기반으로 작성된 Unity용 클라이언트-서버 코드 샘플로 다음과 같은 기능을 제공합니다:

1. Transform 동기화
2. Animation 동기화
3. Rigidbody 동기화 (실험적 기능, **매우 비쌈**)
4. 오브젝트 생성, 파괴
5. RPC
6. 채팅

## How to Install

별도 UnityPackage는 **제공되지 않으며** Repository를 직접 복사해서 사용하면 됩니다.

## Quick Start

1. Repository를 다운로드 받습니다.
2. **Unity 6000.0.46f1 이상 버전**으로 다운로드 받은 Repository 폴더를 엽니다.
3. Unity 에디터 내에서 Colyseus_Client/Scenes/NetworkSample.unity Scene을 엽니다.
4. 터미널 상에서 ../Server 폴더로 이동한 후 ``npm start`` 명령어를 실행해 로컬 서버를 실행합니다.
5. Unity 에디터 상에서 Multiplayer Play Mode 기능을 활성화한 뒤 원하는 만큼 Virtual Player를 추가합니다.
6. Unity 에디터 상에서 Play mode에 진입한 뒤 각각의 Player에서 ``Start Server`` 버튼을 클릭합니다.
7. Enjoy :)

## Manual

[Manual](MANUAL.md) 문서를 참고해주세요.

## License

이 샘플 프로젝트를 제작하는데에 [Colyseus](https://github.com/colyseus/colyseus)가 사용되었습니다. 자세한 내용은 [해당 에셋의 라이선스](Server\LICENCE)를 참고해주세요.

이 샘플 프로젝트를 제작하는데에 [Colyseus Unity SDK](https://github.com/colyseus/colyseus-unity-sdk?tab=readme-ov-file)가 사용되었습니다. 자세한 내용은 [해당 에셋의 라이선스](Assets\Colyseus_Server\LICENSE)를 참고해주세요.

이 샘플 프로젝트를 제작하는데에 Unity에서 제공하는 [Starter Assets](https://assetstore.unity.com/packages/essentials/starter-assets-character-controllers-urp-267961) 에셋이 사용되었습니다. 자세한 내용은 [해당 에셋의 라이선스](https://unity.com/kr/legal/licenses/unity-companion-license)를 참고해주세요.

나머지 제가 작성한 코드는 MIT License를 따릅니다.