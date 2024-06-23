using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class VideoChatMediaStreamService : WebSocketBehavior
{
    //naimg conventions is first : first number is sender , second id reciver
    //Holding all possible connections between users 0, 1, 2 => a full grid
    private static List<string> connections = new List<string>()
    {
        "01", "02", "10", "12", "20", "21"
    };
    private static int connectionCounter = 0;
    protected override void OnOpen()
    {
        Debug.Log( " IritDEbug - in OnOpen " );

        //send client ID
        Sessions.SendTo(connectionCounter.ToString(), ID);
        connectionCounter++;

        // send all connected users
        Sessions.SendTo(string.Join("|", connections), ID);
    }
    protected override void OnMessage(MessageEventArgs e)
    {
        //forward message to all other clients
        foreach(var id in Sessions.ActiveIDs)
        {
            if (id != ID)
            {
                Sessions.SendTo(e.Data, id);
            }
        }
    }
}
