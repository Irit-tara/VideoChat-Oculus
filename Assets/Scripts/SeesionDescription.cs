using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SeesionDescription : IJsonObject<SeesionDescription>
{
    public string SessionType;
    public string Sdp;

    public string ConvertToJSON()
    {
        return JsonUtility.ToJson(this);
    }

    public static SeesionDescription FromJSON(string jsonString)
    {
        return JsonUtility.FromJson<SeesionDescription>(jsonString);
    }
}
