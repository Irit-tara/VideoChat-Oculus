using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class SignalingMessage 
{
    public readonly SignalingMessageType Type;
    public readonly string ChannelID;
    public readonly string Message;

    public SignalingMessage(string messageString)
    {
        var messageArray = messageString.Split("!");
        if (messageArray.Length < 3)
        {
            Type = SignalingMessageType.OTHER;
            ChannelID = "";
            Message = messageString;
        } else if (Enum.TryParse(messageArray[0], out SignalingMessageType resultType))
        {
            switch(resultType)
            {
                case SignalingMessageType.OFFER:
                case SignalingMessageType.ANSWER:
                case SignalingMessageType.CANDIDATE:
                {
                    Type = resultType;
                    ChannelID = messageArray[1];
                    Message = messageArray[2];
                    break;
                 }
                case SignalingMessageType.OTHER:
                default:
                    break;
            }

        }

    }
   /* public static SignalingMessage CreateFromMessage(string messageString)
    {
        return new SignalingMessage(messageString);
    }*/
}

