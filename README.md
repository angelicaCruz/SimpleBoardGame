# SimpleBoardGame
This project is an example use of METool kit. 

**SCENARIO**
- Module used: Collaboration and Live. 
- Two devices share the same scene and interact with a common object: the **Grid**.
- Two devices stand for two players: Player A colors the blocks of the grid with blue while Player B uses color green to color the blocks. 
- At every turn, only one player can interact with the scene. This is where Collaboration module plays a big role. Thanks to its actions, devices and workstation are able to exchanges messages and are synced with each other.
**Note:**
- Before getting started, download [**METookit**](https://github.com/DataMesh-OpenSource/METoolkit "METoolkit Source"). 
- This project is created with the older version on METool kit, so make sure you're not using the METool kit 2017 version. 

If you want to know more, visit [**DataMesh**](https://www.datamesh.com/ "DataMesh website").


### Get Started
Get started by integrating **METool kit** with your Unity project. This can be done in two different ways. 
1. Unzip the folder. Go to Unity, click **Open** and select the unzipped file. 
2. You can create a new project then drag and drop the METool kit folder in the **Project** section in Unity. 

To configure your project correctly, follow [**Configuration manual**](http://docs.datamesh.com/projects/me-live/en/latest/toolkit/toolkit-man-configure-your-project/ "Project Config"). 

Find **_scene_** and have as you main scene. In the project panel, find **MEHoloEntrance** then drag and drop it in the Heirarchy. It is important that you have this in your scene as this manages all the modules of the METool kit: Cursors, Inputs and Collaboration in this scenario. Follow [**Integration manual**](http://docs.datamesh.com/projects/me-live/en/latest/toolkit/toolkit-man-integrated-METoolkit/ "Integration Manual") for a correct integration of the tool kit.  

### Build
- [**For HoloLens**](https://github.com/DataMesh-OpenSource/SolarSystemExplorer/blob/master/Docs/DiveDeeper/build-hololens-app.md "HoloLens build")

- [**For PC**](https://github.com/DataMesh-OpenSource/SolarSystemExplorer/blob/master/Docs/DiveDeeper/build-pc-app.md#build-pc-app "PC build"). Use the setting in this build and run the app in Unity, connect your Spectator view and run the application.

- [**For Surface**](https://github.com/DataMesh-OpenSource/SolarSystemExplorer/blob/master/Docs/DiveDeeper/build-surface-app.md "Surface build")


### NOTES
1. Make sure **MEConfigNetwork.ini** contains your own server ip. If you have MeshExpert Center, you can find it under **Service IP** in the Dash board. If not, you can use you **ipconfig** in you command line.
2. Check **Console** in Unity and see the value of **Delay**. If it is equals to zero even after executing the note above, try turning off your Firewall. 
3. Two players are needed to run the project project correctly.  If you don't have two HoloLens, use the Pc as one of the device players. Also, try to win as the project does not implement "no-winner" situation.(we'll try to have it)
4. You can use this project with Spectator view. 

### Final Result
<p align="center">
<img src="https://user-images.githubusercontent.com/26377727/34098368-054fdab4-e417-11e7-91da-83f548ae8b6b.png">
<p align="center"><em>Spectator and HoloLens view</em></p>
</p>
