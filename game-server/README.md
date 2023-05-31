# ctc-game-server

GameLift 프로토타이핑 서버 프로젝트 입니다. 

### Game Project Simple Git Flow Rule

1. develop 브랜치에서 신규 브랜치 생성 feature/[new-branch-name]
2. feature/[new-branch-name] 에서 작업 후 merge request 하기 전 develop 에서 feature/[new-branch-name] 으로 merge
3. 충돌이 있다면 충돌 부분 작업자와 논의 후 충돌 해결
4. feature/[new-branch-name] 에서 develop으로 merge request
5. merge request 시 feature/[new-branch-name] 브랜치가 더 이상 필요 없다면 merge 후 삭제되게 체크
6. 승인자(파트원)들이 code review 후 approve

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
    - GameServer : 게임 서버, API 서버, AWS GameLift 클라이언트 SDK 적용
    - BattleServer : 배틀을 위한 TCP 서버, GameLift 서버 SDK 적용
- 클래스 라이브러리
    - BattleProtocol : BattleServer와 디바이스 간 주고 받는 패킷 정의
    - CommonProtocol : GameServer와 디바이스 간 주고 받는 패킷 적의
    - BattleTimer : 게임 로직에서 사용할 타이머, System.Timers.Timer 기반으로 Pause 기능을 개발
    - CommonConfig : 서버 환경에 따라 접근 정보를 다르게 쎄팅하기 위한 프로젝트
    - CommonType : enum 정의
    - DBConnector : MySqlConnector 라이브러리
    - ForGameLift : GameLift Build Upload 할 때 서버 바이너리 외에 함께 올라 가야 할 파일
    - Publish : 클라이언트와 공유할 Protocol이나 Enum을 클라이언트에 공유하고 mpc를 사용하여 클라이언트에 MessagePackGenerated.cs 생성

---

### 개별 프로젝트 설명

- GameServer

    API 추가 방법

    1. CommonProtocol.MessageType 추가
    2. ReqXXX, ResXXX 패킷 정의 (ex. ReqAccountJoin.cs, ResAccountJoin.cs)
    3. ResXXX 패킷에서 클라이언트에 전달할 결과가 있다면 ResponseType에 정의
    4. CommonProtocol.ProtocolFactory 클래스의 함수에 추가한 MessageType과 Res, Req 함수 switch case 추가
    5. GameServer프로젝트의 Controllers.User 폴더안에 WebXXXX 컨트롤러 정의, 실제 패킷 받았을때 행위 코드 작성. 
    6. GameServer.ControllerFactory에 MessageType에 따른 WebXXX 분기 switch case 추가


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



- Publish

    * 환경 변수 설정 필요 
    * 고급 시스템 설정 → 환경 변수 → 사용자 변수 새로 만들기
    * 변수 이름 : KCLIENT_TARGET_DIR
    * 변수 값 : 자신의 클라이언트 위치 Path

    ex)

    ![/images/ctc-game-server/ctc-game-server-01.png](/images/ctc-game-server/ctc-game-server-01.png)

    공유 스크립트 복사 및 MessagePack 프로토콜 mpc를 통한 MessagePackGenerated 생성

    Publish 프로젝트 빌드 시 빌드 이벤트 발생 : 프로젝트 속성 → 빌드 이벤트 → 빌드 이벤트 명령줄 대화 상자

    mpc.exe 버전에 따라 상당히 다른 액션을 취하는데 현재 솔루션에서 사용중인 MessagePack 버전에 맞는 mpc.exe "Publish\MessagePackGenerator" 위치

    Publish 빌드에서 mpc 수행하여 생성된 MessagePackGenerated가 가끔 반영이 안될 때가 있음. 빌드 2~3번 수행하면 반연됨

- ForGameLift

    * 빌드 시 Dependency 디렉토리에 있는 파일 복사 ( GameLift Build Upload 시 필요)


### 환경 변수 설정

- GAME_DB_IP
- GAME_DB_PORT
- GAME_DB_USER
- GAME_DB_PASSWORD
- GAME_DB_CHARSET
- GAME_DB_DATABASE