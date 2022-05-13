
<p align="center">
  <img width="25%" src="https://user-images.githubusercontent.com/31209389/168243625-85776e9b-b194-4348-84b2-655ab4b9f67c.png" /> <br>
  <img width="25%" src="https://user-images.githubusercontent.com/31209389/168243713-b58b6915-5ec7-4f8e-a97b-feb8927d1d29.png" /> <br>
  <a href="https://github.com/vrchat-community/UdonSharp">
	  <img width="15%" src="https://user-images.githubusercontent.com/31209389/168245019-1f8bd2f6-1d13-48f0-af39-5dc19aaae6f7.png" /></a>
  <a href="https://github.com/SieR-VR/prism-server-example">
	  <img width="15%" src="https://user-images.githubusercontent.com/31209389/168245026-f8c12c48-448c-42a9-ace2-73c3b0c538da.png" /></a>
  <a href="https://vrchat.com/home">
	  <img width="15%" src="https://user-images.githubusercontent.com/31209389/168245367-4c88ef47-e7d3-41a0-989b-82693208d8e4.png" /></a>


</p>

<h1 align="center">
  :flashlight: PRISM NETWORK SYSTEM:flashlight:<br>
</h1>
<p align="center">
비디오 플레이어 기반 서버->클라이언트 네트워크 패킷 송수신 시스템<br>
Server to Client Networking System Based on Video Player<br>
</p><br>
<br>

## :star: Introdution / 소개
 - **Send string data to VRChat instance in real time.**
 - **(BETA) Request specific data from the server with the promised EndPoint.(BETA)**
<br>

## :package: Require / 필요
[VRChat](https://store.steampowered.com/app/438100/VRChat/)<br>
[Unity 2019.4](https://unity3d.com/kr/unity/whats-new/2019.4.31)<br>
[VRCSDK 3.0](https://vrchat.com/home/download)<br>
[UDONSharp](https://github.com/MerlinVR/UdonSharp)<br>
<br>

## 🧰 Use / 사용

### :open_file_folder: Client Install (Unity/U#)
- Please download it here -> **[Release](https://github.com/kibalab/UdonPrism/releases)**
- Import it into your Unity Project
- Place the "Toggle" prefab

### [:earth_asia: Server Install (CLI/TypeScript)](https://www.npmjs.com/package/prism-vrc)

```
npm install prism-vrc
```
#### OR 
```
yarn add prism-vrc
```
<br>

### :page_facing_up: Client Code Example (UdonSharp)
```CSharp
public class GetDataWithEvent : UdonSharpBehaviour
{
    public void Prism_Data = ""; // Variable that is received when an event occurs and updated with processed data.

    public void OnPrismReady() { return; } //Occurs when the prism system is ready.
    public void OnPrismStart() { return; } //Occurs when the Prism system receives data.
    public void OnPrismFrameChange() { return; } //Occurs when the prismatic system starts reading the next frame of the data.
    public void OnPrismFrameReadEnd() { return; } //Occurs when the Prism system has read all the current frames.
    public void OnPrismReadEnd() { return; } //Occurs when the Prism system reads all received data.


    public void OnPrismException() //Occurs when the Prism system reads all received data.
    { 
      Debug.LogError(Prism_Data); // When an exception occurs, the Prism_Data variable is updated with an exception message.
      return; 
    } 
}
```
```CSharp
using Prism;

public class GetData : UdonSharpBehaviour
{
    public DataReader DataReader;

    public override void Interact()
    {
        Debug.Log(DataReader.Data);
    }
}
```

### [:page_facing_up: Server Code Example (express)](https://github.com/SieR-VR/prism-server-example)
```TypeScript
import Prism from "prism-vrc";

app.get('/', async (req, res) => {
	res.writeHead(200, { 'Content-Type': 'video/mp4' });
	res.end(await Prism("Hello, world!"));
});
```
<br>

### :level_slider: Inspector 
<p align="center">
  <img width="50%" src="https://user-images.githubusercontent.com/31209389/168238008-87d98e7f-1fac-4d4a-8eec-b9dffef04983.png" />
</p>
<br>

### 🖧 Event Behaviour Target List
<p align="center">
  <img width="80%" src="https://user-images.githubusercontent.com/31209389/168241524-84375ab1-0730-4ac6-bd2b-7d4f9ce88e27.png" />
</p>

- Prism 이벤트를 사용할 스크립트는 모두 해당 리스트에 등록해 주세요<br>
- All scripts to use Prism Event must be registered in the appropriate list<br><br>
  <br>
  
## Developer / 개발 참여

### [KIBA_](https://github.com/kibalab)
* 🎲 Client Dev<br>
* 🏗 UI Design<br>
* 📓 Project Manage<br>
* 🖧 Networking<br>
* ✅ Q/A<br>
### [SieR](https://github.com/SieR-VR)
* 🎲 Server Dev<br>
* 🖧 Networking<br>
* ✅ Q/A<br>
<br><br>

## 라이센스 / License

**MIT License**
