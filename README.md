# Implementation Description

### DownloadManager.cs
: UnityPlayerActivity를 통해 Android native plugin과 통신합니다. 
Foreground Service를 실행하고, Service로부터 Message를 전달받아 처리합니다.

### PluginTest.cs
: 유니티 앱이 백그라운드로 진입했을 경우, DownloadManager API 함수를 호출합니다.
파일이 다운로드 될 때 마다 다운로드 상태 텍스트를 업데이트합니다.

### FileViewer.cs
: private External Storage로 다운로드 된 파일의 Preview UI를 업데이트합니다. 


### MainActivity.java
: UnityPlayerActivity를 상속받은 Activity입니다.
Firebase 초기화 및 익명 로그인 후, DownloadServive를 실행합니다.

### DownloadService.java
: ServiceThread를 관리하고, 다운로드가 끝났을 경우 Notification을 전송하는 Foreground Service입니다.
Service의 onDestroy가 호출될 때 Unity 앱에 Message를 보냅니다.

### ServiceThread.java
: Firebase Storage의 특정 folder 위치에 있는 모든 파일을 찾고, 비동기 다운로드를 진행합니다.
각 파일의 다운로드가 완료되었을 경우, Unity 앱에 Message를 보냅니다.

### LocalNotification.java
: 디바이스에 안드로이드 Notification을 만들고 보냅니다.