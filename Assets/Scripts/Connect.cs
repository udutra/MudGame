using UnityEngine;
using System.Collections;

public class Connect : MonoBehaviour {

    string connectToIP = "127.0.0.1";
    int connectPort = 25001;
    public bool showConnect = false;
    
    
    void OnGUI()
    {
        if (!showConnect)
        {
            return;
        }

        if (Network.peerType == NetworkPeerType.Disconnected)
        {

            GUILayout.Label("Connection status: Disconnected");

            connectToIP = GUILayout.TextField(connectToIP, GUILayout.MinWidth(100));
            connectPort = int.Parse(GUILayout.TextField(connectPort.ToString()));

            GUILayout.BeginVertical();
            if (GUILayout.Button("Connect as client"))
            {
                //Connect to the "connectToIP" and "connectPort" as entered via the GUI
                //Ignore the NAT for now
                
                Network.Connect(connectToIP, connectPort);
                


            }

            if (GUILayout.Button("Start Server"))
            {
                //Start a server for 32 clients using the "connectPort" given via the GUI
                //Ignore the nat for now	
                //Network.useNat = false;                
                Network.InitializeServer(4, connectPort, false);
                
            }
            GUILayout.EndVertical();


        }
        else
        {

            if (Network.peerType == NetworkPeerType.Connecting)
            {

                GUILayout.Label("Connection status: Connecting");

            }
            else if (Network.peerType == NetworkPeerType.Client)
            {

                GUILayout.Label("Connection status: Client!");
                GUILayout.Label("Ping to server: " + Network.GetAveragePing(Network.connections[0]));

            }
            else if (Network.peerType == NetworkPeerType.Server)
            {

                GUILayout.Label("Connection status: Server!");
                GUILayout.Label("Connections: " + Network.connections.Length);
                if (Network.connections.Length >= 1)
                {
                    GUILayout.Label("Ping to first player: " + Network.GetAveragePing(Network.connections[0]));
                }
            }

            if (GUILayout.Button("Disconnect"))
            {   
                 Network.Disconnect(200);
            }
        }
    }
}
