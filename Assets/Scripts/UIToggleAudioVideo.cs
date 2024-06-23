using UnityEngine;
using UnityEngine.UI;

public class UIToggleAudioVideo : MonoBehaviour
{
    public VideoChatMediaStream videoChat;  // Reference to your VideoChatMediaStream script
    public Button toggleAudioButton;        // Reference to your toggle audio button
    public Button toggleVideoButton;        // Reference to your toggle video button

    void Start()
    {
        // Add listeners to your buttons
        toggleAudioButton.onClick.AddListener(() => ToggleAudio());
        toggleVideoButton.onClick.AddListener(() => ToggleVideo());
    }

    private void ToggleAudio()
    {
        // Assume some mechanism to track current state or simply toggle
        bool isAudioEnabled = videoChat.IsAudioEnabled();
        videoChat.ToggleAudioStream(!isAudioEnabled);
    }

    private void ToggleVideo()
    {
        // Assume some mechanism to track current state or simply toggle
        bool isVideoEnabled = videoChat.IsVideoEnabled();
        videoChat.ToggleVideoStream(!isVideoEnabled);
    }
}
