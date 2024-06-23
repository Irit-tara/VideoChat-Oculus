using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.WebRTC;
using UnityEngine;
using WebSocketSharp.Server;

public class VideoChatWebSocketServer : MonoBehaviour
{
    private WebSocketServer wssv;
    private string serverIpv4Address;
    private int serverPort = 80;

    private void Awake()
    {
        WebRTC.Initialize();
        //get server ip in the network
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach( var ip in host.AddressList )
        {
            
            if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
            {

                serverIpv4Address = ip.ToString();
                break;
            }
            
            

        }
        wssv = new WebSocketServer($"ws://{serverIpv4Address}:{serverPort}");
        wssv.AddWebSocketService<VideoChatMediaStreamService>($"/{nameof(VideoChatMediaStreamService)}");
        wssv.Start();
    }

    private void OnDestroy()
    {
        wssv.Stop();
        WebRTC.Dispose();

    }
}
