using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CandidateInit : IJsonObject<CandidateInit>
{
    public string Candidate;
    public string SdpMid;
    public int SdpMLineIndex;

    public static CandidateInit FromJSON(string jsonString)
    {
        return JsonUtility.FromJson<CandidateInit>(jsonString);
    }

    public string ConvertToJSON()
    {
        return JsonUtility.ToJson(this);
    }
}
