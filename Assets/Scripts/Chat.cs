using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Chat : MonoBehaviour {

    public bool usingChat = false;  //Can be used to determine if we need to stop player movement since we're chatting
    public GUISkin skin;                       //Skin
    public bool showChat = false;          //Show/Hide the chat

    //Private vars used by the script
    private string inputField = "";

    private Vector2 scrollPosition;
    private int width = 500;
    private int height = 180;
    public string playerName;
    private float lastUnfocusTime = 0;
    private Rect window;
    NetworkView network;
    public bool newName;
    public NetworkPlayer networkPlayer;
    public Places playerAtThisPlace;
   

    public Itens myItens;
    public bool hasMap = false;

    Server serverScript;


    void Start()
    {
        window = new Rect(Screen.width / 2 - width / 2, Screen.height - height + 5, width, height);
        serverScript = GetComponent<Server>();
    }

    private List<ChatEntry> chatEntries;
    public class ChatEntry
    {
        public string name = "";
        public string text = "";
    }

    void OnDisconnectedFromServer()
    {
        CloseChatWindow();
    }
	
	public void CloseChatWindow()
    {
        showChat = false;
        inputField = "";
        newName = false;
        chatEntries = new List<ChatEntry>();
    }
	
    public void ShowChatWindow()
    {
        showChat = true;
        inputField = "";
        chatEntries = new List<ChatEntry>();
    }

    void OnGUI()
    {
        if (!showChat)
        {
            return;
        }

        GUI.skin = skin;

        if (Event.current.type == EventType.keyDown && inputField.Length <= 0 && Event.current.character == '\n')
        {
            if (lastUnfocusTime + 0.25f < Time.time)
            {
                usingChat = true;
                GUI.FocusWindow(5);
                GUI.FocusControl("Chat input field");
            }
        }

        window = GUI.Window(5, window, GlobalChatWindow, "");
    }


    void GlobalChatWindow(int id)
    {

        GUILayout.BeginVertical();
        GUILayout.Space(10);
        GUILayout.EndVertical();
        

        // Begin a scroll view. All rects are calculated automatically - 
        // it will use up any available screen space and make sure contents flow correctly.
        // This is kept small with the last two parameters to force scrollbars to appear.
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        foreach (ChatEntry entry in chatEntries)
        {
            GUILayout.BeginHorizontal();
            if (entry.name == "")
            {//Game message
                
                GUILayout.Label(entry.text);
            }
            else
            {
                GUILayout.Label(entry.name + ": " + entry.text);
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(3);

        }
        // End the scrollview we began above.
        GUILayout.EndScrollView();

        if (Event.current.type == EventType.keyDown && inputField.Length > 0 && Event.current.character == '\n')
        {
            if (newName)
            {
                HitEnter(inputField);
            }
            else
            {
                NewPlayer(inputField);
                inputField = ""; //Clear line
                GUI.UnfocusWindow();//Deselect chat
                lastUnfocusTime = Time.time;
                usingChat = false;
            }
        }
        GUI.SetNextControlName("Chat input field");
        inputField = GUILayout.TextField(inputField);


        if (Input.GetKeyDown("mouse 0"))
        {
            if (usingChat)
            {
                usingChat = false;
                GUI.UnfocusWindow();//Deselect chat
                lastUnfocusTime = Time.time;
            }
        }
    } 

    void HitEnter(string msg)
    {
        msg = msg.Replace("\n", "");
        //GetComponent<NetworkView>().RPC("Interpret", RPCMode.Server, msg, this);        
        GetComponent<NetworkView>().RPC("Interpret", RPCMode.Server, msg, playerName);
        inputField = ""; //Clear line
        GUI.UnfocusWindow();//Deselect chat
        lastUnfocusTime = Time.time;
        usingChat = false;
    }

    public void NewPlayer(string inputField)
    {              
        this.playerName = inputField.ToLower();
        AddChatMessage("Player", inputField);
        GetComponent<NetworkView>().RPC("NewPlayerList", RPCMode.Server, this.playerName);
        inputField = ""; //Clear line
        GUI.UnfocusWindow();//Deselect chat
        lastUnfocusTime = Time.time;
        usingChat = false;          
       
    }

    [RPC]
    public void AddChatMessage(string name, string msg)
    {
        ChatEntry entry = new ChatEntry();
        entry.name = name;
        entry.text = msg;

        if (msg.CompareTo("Nome alterado com sucesso! ") == 0){
            this.newName = true;
        }
        chatEntries.Add(entry);

        //Remove old entries
        if (chatEntries.Count > 10)
        {
            chatEntries.RemoveAt(0);
        }

        scrollPosition.y = 1000000;
    }  
}
