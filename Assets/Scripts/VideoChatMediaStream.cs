using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Unity.VisualScripting;
using Unity.WebRTC;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;

using UnityEngine.Events;
using UnityEditor.Presets;

public class VideoChatMediaStream : MonoBehaviour
{

    [SerializeField] private string clientId;
    [SerializeField] private Camera cameraStream;
    // [SerializeField] private RawImage sourceImage;
    [SerializeField] private GameObject displayQuad;
    [SerializeField] private List<GameObject> receiveImages = new List<GameObject>();

    private Dictionary<string, RTCPeerConnection> pcs = new Dictionary<string, RTCPeerConnection>();
    private VideoStreamTrack videoStreamTrack ;

    private bool hasRecievedOffer = false;
    private SeesionDescription recivedOfferSessionDescTemp;
    private string receivedOfferChannelId;

    private WebSocket ws;
    private int receiveImageCounter = 0;
    private float connectionTimeout = 5.0f;  // Timeout in seconds

    public AudioSource audioSource;
    private AudioStreamTrack audioStreamTrack;
    LogManager logManager;

    public GameObject button;
    public UnityEvent onPress;
    GameObject presser;
    bool isBtnPressed;
    private void Awake()
    {
        WebRTC.Initialize();
        // Check if AudioSource component already exists
        
    }
        // Start is called before the first frame update
    void Start()
    {
        if (audioSource == null)
        {
            // Find an AudioSource component in the scene
            audioSource = FindObjectOfType<AudioSource>();

            // If no AudioSource component is found, create a new GameObject and add AudioSource to it
            if (audioSource == null)
            {
                GameObject audioObject = new GameObject("MicrophoneAudioSource");
                audioSource = audioObject.AddComponent<AudioSource>();
            }
        }

        logManager = FindObjectOfType<LogManager>();
       
    }

    // Update is called once per frame
    void Update()
    {
        if (hasRecievedOffer)
        {
            hasRecievedOffer = !hasRecievedOffer;
            StartCoroutine(CreateAnswer(pcs[receivedOfferChannelId], receivedOfferChannelId));
        }
    }

    private void OnDestroy()
    {
      

        videoStreamTrack?.Stop();
        audioStreamTrack?.Stop();
        Microphone.End(null); // Stop microphone capture when the client is destroyed.
        foreach (var connection in pcs)
        {
            connection.Value.Close();
        }
        if (ws != null)
            ws.Close();

        WebRTC.Dispose();
    }

    public void InitClient(string serverIp, int serverPort = 8080)
    {
        ws = new WebSocket($"ws://{serverIp}:{serverPort}/{nameof(VideoChatMediaStreamService)}");
        ws.OnMessage += (sender, e) =>
        {
            var singlingMessage = new SignalingMessage(e.Data);
            //Debug.Log($"GotMessageClient Type {singlingMessage.Type.ToString()} Data: {e.Data.ToString()}");
            switch(singlingMessage.Type)
            {
                case SignalingMessageType.OFFER:
                    //only set offerand send answer on reciving connections on this client
                    //clients that are bound to receive from me
                    if (clientId == singlingMessage.ChannelID.Substring(1,1))
                    {
                        Debug.Log(clientId + " - Got OFFER with channel ID " + singlingMessage.ChannelID + "message :" + singlingMessage.Message);

                        receivedOfferChannelId = singlingMessage.ChannelID;
                        recivedOfferSessionDescTemp = SeesionDescription.FromJSON(singlingMessage.Message);
                        hasRecievedOffer = true;

                    }
                    break;
                case SignalingMessageType.ANSWER:
                    //for sending connections on this client
                    if (clientId == singlingMessage.ChannelID.Substring(0,1))
                    {
                        Debug.Log(clientId + " - Got ANSWER with channel ID " + singlingMessage.ChannelID + "message :" + singlingMessage.Message);

                        var recievedAnswerSessionDescTemp = SeesionDescription.FromJSON(singlingMessage.Message);
                        RTCSessionDescription answerSessionDesc = new RTCSessionDescription();
                        answerSessionDesc.type = RTCSdpType.Answer;
                        answerSessionDesc.sdp = recievedAnswerSessionDescTemp.Sdp;

                        pcs[singlingMessage.ChannelID].SetRemoteDescription(ref answerSessionDesc);
                    }
                    break;
               case SignalingMessageType.CANDIDATE:
                    {
                        if (clientId == singlingMessage.ChannelID.Substring(1,1))
                        {
                            Debug.Log(clientId + " - Got CANDIDATE with channel ID " + singlingMessage.ChannelID + "message :" + singlingMessage.Message);

                            var candidateInit = CandidateInit.FromJSON(singlingMessage.Message);
                            RTCIceCandidateInit init = new RTCIceCandidateInit();
                            init.sdpMid = candidateInit.SdpMid;
                            init.sdpMLineIndex = candidateInit.SdpMLineIndex;
                            init.candidate = candidateInit.Candidate;
                            RTCIceCandidate candidate = new RTCIceCandidate(init);

                            // add candidate to list of connections
                            pcs[singlingMessage.ChannelID].AddIceCandidate(candidate);
                        }
                    }
                    break;
                default:
                    if (e.Data.Contains("|"))
                    {
                        Debug.Log(clientId + " message :" + singlingMessage.Message);


                        //first message on OnInit of the web soucket has '!" sent to all participantns 
                        var connectionIds = e.Data.Split('|');
                        foreach(var connectionId in connectionIds)
                        {
                            //if client = client 1 , add only relevant connections
                            //can send 10,12 and recieve only 01,21
                            if (connectionId.Contains(clientId))
                            {
                                pcs.Add(connectionId, CreatePeerConnection(connectionId));
                            }
                        }
                    }
                    else
                    {
                        Debug.Log($"My Client ID = {e.Data}");
                        //set clientId, based on the connection order
                        clientId = e.Data;
                    }
                    break;
            }
        };
        ws.Connect();
        ws.OnOpen += (sender, e) =>
        {
            Debug.Log("*****   Connection opened ******");
            StopCoroutine("CheckConnectionTimeout");  // Stop the timeout check when connection is successful
        };

        ws.OnError += (sender, e) =>
        {
            Debug.Log("Error encountered: " + e.Message);
        };

        ws.OnClose += (sender, e) =>
        {
            Debug.Log("Connection closed: " + e.Reason);
        };
        videoStreamTrack = cameraStream.CaptureStreamTrack(1280, 720);
        //sourceImage.texture = cameraStream.targetTexture;
        displayQuad.GetComponent<Renderer>().material.mainTexture = cameraStream.targetTexture;

        // Start microphone and assign it to AudioSource
        if (Microphone.devices.Length > 0)
         {
             audioSource.clip = Microphone.Start(null, true, 10, 44100);
          
            /* while (!(Microphone.GetPosition(null) > 0)) { }
             audioSource.Play();*/
         }

         // Create AudioStreamTrack from the AudioSource
         audioStreamTrack = new AudioStreamTrack(audioSource);


        //ws.ConnectAsync();  // Use ConnectAsync to not block the main thread
        // StartCoroutine("CheckConnectionTimeout");

        if (!ws.IsAlive)  // Check if the connection is not established
        {
            Debug.Log("Connection attempt timed out.");
            ShowErrorToUser("Failed to connect to the server.Connection attempt timed out.");
            ws.Close();  // Ensure the connection is closed properly
        }else
        {
            ShowErrorToUser("Connection Established");
            StartCoroutine(WebRTC.Update());
        }

       

    }

    private IEnumerator CheckConnectionTimeout()
    {
        yield return new WaitForSeconds(connectionTimeout);
        if (!ws.IsAlive)  // Check if the connection is not established
        {
            Debug.Log("Connection attempt timed out.");
            ShowErrorToUser("Failed to connect to the server.");
            ws.Close();  // Ensure the connection is closed properly
        }
    }

    private void ShowErrorToUser(string errorMessage)
    {
        // Implement user notification, e.g., using a UI Text element or a popup
        Debug.Log(errorMessage);  // Placeholder for actual user notification
        logManager.AddMessage(errorMessage);
    }
    private RTCPeerConnection CreatePeerConnection(string id) // id id websoucket 01,10,...
    {
        var pc = new RTCPeerConnection();
        pc.OnIceCandidate = candidate =>
        {
            var candidateInit = new CandidateInit()
            {
                SdpMid = candidate.SdpMid,
                SdpMLineIndex = candidate.SdpMLineIndex ?? 0,
                Candidate = candidate.Candidate,
            };
            // defined candidate message - edit and add the client id
            ws.Send("CANDIDATE!" + id + "!" + candidateInit.ConvertToJSON());


        };
        pc.OnIceConnectionChange = state => { Debug.Log(state); };
        pc.OnNegotiationNeeded = () => { StartCoroutine(CreateOffer(pc, id)); };

        pc.OnTrack = e =>
        {
            if (e.Track is VideoStreamTrack track)
            {
                track.OnVideoReceived += tex =>
                {
                    logManager.AddMessage("recieveing Image");
                   // displayQuad.GetComponent<Renderer>().material.mainTexture = tex;
                    //receiveImages[receiveImageCounter].texture = tex;
                    receiveImages[receiveImageCounter].GetComponent<Renderer>().material.mainTexture = tex;
                    receiveImageCounter++;
                };
            }
            if (e.Track is AudioStreamTrack audioTrack)
            {
                if (!gameObject.GetComponent<AudioSource>())
                {
                    logManager.AddMessage("recieveing Audio");
                    var audioSource = gameObject.AddComponent<AudioSource>();
                    audioSource.SetTrack(audioTrack);
                    audioSource.Play();
                }
                else
                {
                    Debug.LogWarning("AudioSource already exists on this GameObject. Skipping adding a new one.");
                }

            }
        };

        /*pc.AddTrack(videoStreamTrack);*/
      
        return pc;
    }

    private IEnumerator CreateOffer(RTCPeerConnection pc, string id)
    {
        var offer = pc.CreateOffer();
        yield return offer;

        var offerDesc = offer.Desc;
        var localDescOp = pc.SetLocalDescription(ref offerDesc);
        yield return localDescOp;

        //senf desc to server for reciever connection
        var offerSessionDesc = new SeesionDescription()
        {
            SessionType = offerDesc.type.ToString(),
            Sdp = offerDesc.sdp
        };
        ws.Send("OFFER!" + id + "!" + offerSessionDesc.ConvertToJSON());
    }

    private IEnumerator CreateAnswer(RTCPeerConnection pc, string id)
    {
        RTCSessionDescription offerSessionDesc = new RTCSessionDescription();
        offerSessionDesc.type = RTCSdpType.Offer;
        offerSessionDesc.sdp = recivedOfferSessionDescTemp.Sdp;

        var remoteDescOp = pc.SetRemoteDescription(ref offerSessionDesc);
        yield return remoteDescOp;

        var answer = pc.CreateAnswer();
        yield return answer;

        var answerDesc = answer.Desc;
        var localDescOp = pc.SetLocalDescription(ref answerDesc);
        yield return localDescOp;

        var answerSessionDesc = new SeesionDescription()
        {
            SessionType = answerDesc.type.ToString(),
            Sdp = answerDesc.sdp
        };
        ws.Send("ANSWER!" + id + "!" + answerSessionDesc.ConvertToJSON());
          
    }

    // for any connection - establishing the connection
    public void call()
    {

        foreach(var connection in pcs)
        {
            //only add tracks for sending connections - first number is client ID
            if (connection.Key.Substring(0,1) == clientId)
            {
                connection.Value.AddTrack(videoStreamTrack);
                if (audioStreamTrack != null)
                {
                    connection.Value.AddTrack(audioStreamTrack);
                }
            }
        }
    }

    public void ConnectToServer()
    {
        string ipNPort = InputServerIP.serverIP;
        if (ipNPort == "" || ipNPort == null)
        {
            ShowErrorToUser("Please enter IP address and port (ex:127.0.0.1:80)");
            return;
        }
            
        string[] ppp = ipNPort.Split(":");
        if (ppp.Length != 2)
        {
            ShowErrorToUser("IP and port not in valid format (ex:127.0.0.1:80)");
            return;
        }
            
        InitClient(ppp[0], Int32.Parse(ppp[1]));
    }

    public bool IsVideoEnabled()
    {
        return videoStreamTrack.Enabled;
    }
    public void ToggleVideoStream(bool enable)
    {
        if (videoStreamTrack != null)
        {
            videoStreamTrack.Enabled = enable;  // Enable or disable the video stream track
        }
    }

    public bool IsAudioEnabled()
    {
        return audioStreamTrack.Enabled;
    }

    // Method to toggle audio stream
    public void ToggleAudioStream(bool enable)
    {
        if (audioStreamTrack != null)
        {
            audioStreamTrack.Enabled = enable;  // Enable or disable the audio stream track
        }
    }

    // VR Events
    private void OnTriggerEnter(Collider other)
    {
        if (! isBtnPressed)
        {
            button.transform.localPosition = new Vector3(0, 0.003f, 0);
            presser = other.gameObject;
            isBtnPressed = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == presser)
        {
            button.transform.localPosition = new Vector3(0, 0.015f, 0);
            isBtnPressed=false;
            call();
        }
    }
}
