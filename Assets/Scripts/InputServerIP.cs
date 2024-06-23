using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputServerIP : MonoBehaviour
{
    public TMP_InputField input;
    private static string _serverIP;
    public static string serverIP
    {
        get { return _serverIP; }
        set { _serverIP = value; }
    }

    public void SetServerIP(string txt)
    {
        _serverIP = txt;
    }
    public string GetIP()
    {
        return _serverIP;
    }
   
}
