using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;
using DataMesh.AR;
using DataMesh.AR.UI;
using DataMesh.AR.Interactive;
using DataMesh.AR.Network;
using MEHoloClient.Entities;
using MEHoloClient.Proto;
using System.Linq;

public struct Player
{
    public string id;
    public string player;
    public bool isTurn;
}

public enum CType
{
    blue = 0,
    green = 1,
    white = 2
}
public class MainApp : MonoBehaviour, IMessageHandler
{
    //modules
    private MultiInputManager inputManager;
    private CollaborationManager collaborationManager;
    private CursorController cursor;

    //variables for message entry
    private ShowObject showObject;
    private SceneObject roomData;
    //private variables
    private GameObject focused;
    private Player playerA;
    private Player playerB;
    private string devId;
    private CType CurrentColor;
    //public variables
    public List<string> idList;
    public GameObject startBtn;
    public GameObject player1Btn;
    public GameObject restartBtn;
    public GameObject grid;
    public GameObject text;
    public GameObject message;
    public GameObject playingA;
    public GameObject playingB;
    public GameObject winBlue;
    public GameObject winGreen;
    public List<GameObject> cells;
    private GameObject selected;
    public static Combinations combinations = new Combinations();   
    private List<string> blueSelection = new List<string>();
    private List<string> greenSelection = new List<string>();
    private float counter;
    private float blueCounter;
    private float greenCounter;
    private string waiting;
    private float cellcounter;
    private bool noWinner;


    // Use this for initialization
    void Start()
    {
        StartCoroutine(WaitForInit());
        idList = new List<string>();
        devId = SystemInfo.deviceUniqueIdentifier;
    }

    /// <summary>
    /// initialization of modules and variables
    /// </summary>
    /// <returns></returns>
    private IEnumerator WaitForInit()
    {
        MEHoloEntrance entrance = MEHoloEntrance.Instance;
        while (!entrance.HasInit)
        {
            yield return null;
        }

        //instantiate elements
        inputManager = MultiInputManager.Instance;
        inputManager.layerMask = LayerMask.GetMask("Default") | LayerMask.GetMask("UI");
        inputManager.cbTap += OnTap;
        //collaboration module
        collaborationManager = CollaborationManager.Instance;
        collaborationManager.AddMessageHandler(this);
        collaborationManager.cbEnterRoom = cbEnterRoom;
        //cursor module
        cursor = UIManager.Instance.cursorController;
        
        //it is possible to use more than message.
        //bear in mind that messages must have different id so 
        string showId = "showId001";
        MsgEntry msg = new MsgEntry();
        msg.ShowId = showId;
        showObject = new ShowObject(msg);
        roomData = new SceneObject();
        roomData.ShowObjectDic.Add(showObject.ShowId, showObject);

        collaborationManager.roomInitData = roomData;
        collaborationManager.TurnOn();
    }

    /// <summary>
    /// manages the the AirTap event
    /// </summary>
    /// <param name="count"></param>
    private void OnTap(int count)
    {
        try
        {
            if (inputManager.FocusedObject != null)
                focused = inputManager.FocusedObject;
        }
        catch (Exception e)
        {
            Debug.Log(e);
        }

        if (focused.name == player1Btn.name)
        {
            PlayAction();
            player1Btn.SetActive(false);
            playerA.player = "A";
            startBtn.SetActive(true);
        }

        if (focused.name == startBtn.name)
        {

            //create players
            playerA.id = idList[0];
            playerB.player = "B";
            playerA.isTurn = false;

            //comment this block if you want to try the code
            try
            {
                playerB.id = idList[1];
                playerB.isTurn = false;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }

            VerifyPlayer();
            startBtn.SetActive(false);
            ShowCubeandText();
        }

        if (focused.name == restartBtn.name)
        {
            Restart();
        }

        try
        {
            if (focused.tag == "Cell")
            {
                cellcounter += 1;
                message.SetActive(false);
                //Debug.Log("Name is :"+focused.name);
                if (waiting == playerA.id)
                    WaitingPlayerMessage(playerB.id);

                if (waiting == playerB.id)
                    WaitingPlayerMessage(playerA.id);

                ClickCube(collaborationManager.clientId, focused.name, cellcounter);
            }

            
        }
        catch (Exception e)
        { Debug.Log(e); }
    }

    /// <summary>
    /// called when restart button is pressed
    /// </summary>
    void Restart()
    {
        counter = 0;
        blueCounter = 0;
        greenCounter = 0;
        blueSelection.Clear();
        greenSelection.Clear();
        focused = null;
        player1Btn.SetActive(true);
        grid.SetActive(false);
        winBlue.SetActive(false);
        winGreen.SetActive(false);
        idList.Clear();
        text.SetActive(false);

        foreach (GameObject go in cells)
        {
            go.GetComponent<Renderer>().material.color = Color.white;
            go.GetComponent<Collider>().enabled = true;
        }
        restartBtn.SetActive(false);

    }

    /// <summary>
    /// manages the color of the cube according to the player that clicks the cube
    /// </summary>
    /// <param name="ip"></param>
    private void ClickCube(string ip, string cellname, float cellcount)
    {

        if (ip == playerA.id)
        {
            CurrentColor = 0;
            blueCounter += 1;
            if (blueCounter > 3)
            {
                blueSelection.Clear();
                blueCounter = 1;
            }

            counter = blueCounter;
        }

        if (ip == playerB.id)
        {
            CurrentColor = CType.green;
            greenCounter += 1;
            if (greenCounter > 3)
            {
                greenSelection.Clear();
                greenCounter = 1;
            }

            counter = greenCounter;
        }

        MsgEntry entry = new MsgEntry();
        entry.OpType = MsgEntry.Types.OP_TYPE.Upd;
        entry.ShowId = ip;
        ObjectInfo info = new ObjectInfo();
        info.ObjType = cellname;
        entry.Info = info;
        entry.Pr.Add(counter);
        entry.Pr.Add(cellcount);
        entry.Vec.Add((long)CurrentColor);

        SyncMsg msg = new SyncMsg();
        msg.MsgEntry.Add(entry);

        collaborationManager.SendMessage(msg);
    }

    /// <summary>
    /// verify which device is Player A
    /// and whice device is Player B
    /// </summary>
    private void VerifyPlayer()
    {
        foreach (string s in idList)
        {
            Debug.Log("ID LIST CONTENT: " + s);
        }
        if (collaborationManager.clientId == playerA.id)
        {
            text.SetActive(true);
            text.GetComponent<Text>().text = "You are Player A";
            playerA.isTurn = true;
        }

        if (collaborationManager.clientId == playerB.id)
        {
            text.SetActive(true);
            text.GetComponent<Text>().text = "You are Player B";
            playerB.isTurn = false;
        }

        WaitingPlayerMessage(playerB.id);
    }

    /// <summary>
    /// creates a message that contains
    /// the waiting player id
    /// </summary>
    void WaitingPlayerMessage(string currPlayer)
    {
        MsgEntry msgEntry = new MsgEntry();
        msgEntry.OpType = MsgEntry.Types.OP_TYPE.Upd;
        msgEntry.ShowId = "001";
        ObjectInfo info = new ObjectInfo();
        info.ObjType = currPlayer;
        msgEntry.Info = info;

        SyncMsg msg = new SyncMsg();
        msg.MsgEntry.Add(msgEntry);
        collaborationManager.SendMessage(msg);
    }

    /// <summary>
    /// shows the game cube and
    /// a message: Player A starts
    /// </summary>
    private void ShowCubeandText()
    {
        grid.SetActive(true);
        message.SetActive(true);
    }

    /// <summary>
    /// creates the message to be sent to the server.
    /// Content: device Ip 
    /// </summary>
    void PlayAction()
    {
        MsgEntry msgEntry = new MsgEntry();
        msgEntry.OpType = MsgEntry.Types.OP_TYPE.Upd;
        msgEntry.ShowId = "000";
        ObjectInfo info = new ObjectInfo();
        info.ObjType = collaborationManager.clientId;
        msgEntry.Info = info;

        SyncMsg msg = new SyncMsg();
        msg.MsgEntry.Add(msgEntry);
        collaborationManager.SendMessage(msg);
    }

    /// <summary>
    /// prints message on Entrance successful
    /// </summary>
    private void cbEnterRoom()
    {
        Debug.Log("Enter Room Successfully");
    }

    /// <summary>
    /// message handler
    /// </summary>
    /// <param name="proto"></param>
    void IMessageHandler.DealMessage(SyncProto proto)
    {

        this.DealMessage(proto);
    }
    /// <summary>
    /// processes the messages
    /// </summary>
    /// <param name="proto"></param>
    void DealMessage(SyncProto proto)
    {
        Google.Protobuf.Collections.RepeatedField<MsgEntry> messages = proto.SyncMsg.MsgEntry;
        //Debug.Log("deal message");
        if (messages == null)
            return;
        for (int i = 0; i < messages.Count; i++)
        {
            MsgEntry msgEntry = messages[i];
            if (msgEntry.ShowId == showObject.ShowId)
            {
                //ChangeCubeColor((ColorType)((int)msgEntry.Vec[0]));
                Debug.Log("I am deal message");
            }
            if (msgEntry.ShowId == "000")
            {
                string s = msgEntry.Info.ToString();
                string ipAddress = s.Substring(14, collaborationManager.clientId.Length);
                //the devices' IP are extracted and inserted in idList
                //the ip is used to diversify clearly one player to the other
                idList.Add(ipAddress);
            }

            if (msgEntry.ShowId == playerA.id)
            {
                string s = msgEntry.Info.ToString();
                string cell = s.Substring(14, 5);
                //Debug.Log(cell);
                ChangeCubeColor((CType)((int)msgEntry.Vec[0]), cell, msgEntry.Pr[0]);
                NoWinner(msgEntry.Pr[1]);
            }

            if (msgEntry.ShowId == playerB.id)
            {
                string s = msgEntry.Info.ToString();
                string cell = s.Substring(14, 5);
                ChangeCubeColor((CType)((int)msgEntry.Vec[0]), cell, msgEntry.Pr[0]);
                NoWinner(msgEntry.Pr[1]);
            }

            if (msgEntry.ShowId == "001")
            {
                string s = msgEntry.Info.ToString();
                waiting = s.Substring(14, collaborationManager.clientId.Length);
                Debug.Log("Waiting is :" + waiting);
                VerifyCurrentPlayer(waiting);
            }
        }
    }
    
    /// <summary>
    /// verifies which player is waiting. 
    /// since the message is broadcasted to all devices, verify if waitingPlayer value is the same as the device's
    /// </summary>
    /// <param name="waitingPlayer"></param>
    void VerifyCurrentPlayer(string waitingPlayer)
    {
        //if device is the waiting device, then verify it device is player A or B
        //then disable colliders to enhibit interactions
        if (waitingPlayer == collaborationManager.clientId)
        {
            //Debug.Log("IF VERIFY CURRENT PLAYER");
            if (waitingPlayer == playerA.id)
            {
                Debug.Log("WAITING IS A");
                //message.SetActive(true);
                //message.GetComponent<Text>().text = "Player B is playing";
            }

            if (waitingPlayer == playerB.id)
            {
                Debug.Log("wAITING IS B");
                //message.SetActive(true);
                //message.GetComponent<Text>().text = "Player A is playing";
            }

            foreach (GameObject go in cells)
            {
                go.GetComponent<Collider>().enabled = false;
            }

        }
        else
        {
            //if not waiting device then, enable colliders
            foreach (GameObject go in cells)
            {
                go.GetComponent<Collider>().enabled = true;
            }
        }

        //message.SetActive(false);
    }

    /// <summary>
    /// changes the color of the selecrted cell
    /// and remove its collider so other user can't interact with it
    /// </summary>
    /// <param name="CurrentColor"></param>
    /// <param name="cell"></param>
    void ChangeCubeColor(CType CurrentColor, string cell,float count)
    {
        foreach (GameObject go in cells)
        {
            if (go.name == cell)
                selected = go;
        }

        Debug.Log("Selected.NAme is : " + selected.name);
        switch (CurrentColor)
        {
            case CType.blue:
                selected.GetComponent<Renderer>().material.color = Color.blue;
                selected.GetComponent<Collider>().enabled = false;
                blueSelection.Add(selected.name);
                CompareBlue();
                break;
            case CType.green:
                selected.GetComponent<Renderer>().material.color = Color.green;
                selected.GetComponent<Collider>().enabled = false;
                greenSelection.Add(selected.name);
                CompareGreen();
                break;
        }
    }

    /// <summary>
    /// compares the blue selection to all 
    /// the combinations
    /// </summary>
    private void  CompareBlue()
    {

        if (!combinations.c1.Except(blueSelection).Any())
        {
            //Debug.Log("Hooray!");
            winBlue.SetActive(true);
            restartBtn.SetActive(true);
            return;
        }
        

        if (!combinations.c2.Except(blueSelection).Any())
        {
            //Debug.Log("Hooray!");
            winBlue.SetActive(true);
            restartBtn.SetActive(true);
            return;
        }
       

        if (!combinations.c3.Except(blueSelection).Any())
        {
            //Debug.Log("Hooray!");
            winBlue.SetActive(true);
            restartBtn.SetActive(true);
            return;
        }
        

        if (!combinations.c5.Except(blueSelection).Any())
        {
            //Debug.Log("Hooray!");
            winBlue.SetActive(true);
            restartBtn.SetActive(true);
            return;
        }
        

        if (!combinations.c6.Except(blueSelection).Any())
        {
            //Debug.Log("Hooray!");
            winBlue.SetActive(true);
            restartBtn.SetActive(true);
            return;
        }
        

        if (!combinations.c7.Except(blueSelection).Any())
        {
            //Debug.Log("Hooray!");
            winBlue.SetActive(true);
            restartBtn.SetActive(true);
            return;
        }
        

        if (!combinations.c8.Except(blueSelection).Any())
        {
            //Debug.Log("Hooray!");
            winBlue.SetActive(true);
            restartBtn.SetActive(true);
            return;
        }
       
    }

    /// <summary>
    /// compares the green selection to all
    /// the combiantions
    /// </summary>
    private void CompareGreen()
    {

        if (!combinations.c1.Except(greenSelection).Any())
        {
            //Debug.Log("Hooray!");
            winGreen.SetActive(true);
            restartBtn.SetActive(true);
            return;
        }
     

        if (!combinations.c2.Except(greenSelection).Any())
        {
            //Debug.Log("Hooray!");
            winGreen.SetActive(true);
            restartBtn.SetActive(true);
            return;
        }
       

        if (!combinations.c3.Except(greenSelection).Any())
        {
            //Debug.Log("Hooray!");
            winGreen.SetActive(true);
            restartBtn.SetActive(true);
            return;
        }
        
        if (!combinations.c5.Except(greenSelection).Any())
        {
            //Debug.Log("Hooray!");
            winGreen.SetActive(true);
            restartBtn.SetActive(true);
            return;
        }
       

        if (!combinations.c6.Except(greenSelection).Any())
        {
            //Debug.Log("Hooray!");
            winGreen.SetActive(true);
            restartBtn.SetActive(true);
            return;
        }
       

        if (!combinations.c7.Except(greenSelection).Any())
        {
            //Debug.Log("Hooray!");
            winGreen.SetActive(true);
            restartBtn.SetActive(true);
            return;
        }
        

        if (!combinations.c8.Except(greenSelection).Any())
        {
            //Debug.Log("Hooray!");
            winGreen.SetActive(true);
            restartBtn.SetActive(true);
            return;
        }
        
    }
}
