# rsp-server

GameLift를 활용한 가위바위보 게임 프로젝트입니다. 


### Game Project Commit Message Simple Rule

commit types : feat, fix, build, chore, ci, docs, style, refactor, test

* feat : 새로운 기능에 대한 커밋
* fix : 버그 수정에 대한 커밋
* build : 빌드 관련 파일 수정에 대한 커밋
* chore : 그 외 자잘한 수정에 대한 커밋
* ci : CI관련 설정 수정에 대한 커밋
* docs : 문서 수정에 대한 커밋
* style : 코드 스타일 혹은 포맷 등에 관한 커밋
* refactor :  코드 리팩토링에 대한 커밋
* test : 테스트 코드 수정에 대한 커밋

---


### 주요 프로젝트 구성

- 서버
    - BattleServer : 배틀을 위한 TCP 서버, GameLift 서버 SDK 적용
- 클래스 라이브러리
    - BattleProtocol : BattleServer와 디바이스 간 주고 받는 패킷 정의
    - CommonProtocol : GameServer와 디바이스 간 주고 받는 패킷 정의
    - BattleTimer : 게임 로직에서 사용할 타이머, System.Timers.Timer 기반으로 Pause 기능을 개발
    - CommonType : enum 정의

---

### 개별 프로젝트 설명

- BattleServer

    패킷 정의

    1. BattleProtocol.MessageType 추가
    2. BattleProtocol.Entities 폴더 안에 ProtoXXX 패킷 정의
    3. 클라이언트에 전달할 결과가 있다면 CommonType.ResultType에 추가
    4. BattleProtocol.ProtocolFactory 클래스에 case 추가
    5. BattleServer.Controller.Controllers 폴더에 패킷 받았을때 행위 코드 작성
    6. BattleServer.Controller.ControllerFactory 클래스에 case 추가

    게임로직

    - Player : 플레이어 정보
    - Room : Game 진행, GameLift 특성 상 단일 Room 구조, 프로세스 하나에 하나의 게임 진행
    - BattleProgress : Timer가 돌아가면서 정의된 내용대로 게임이 진행
    - BattleAction : Progress 내부에서 조건이 되었을때 DoAction
