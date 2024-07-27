using System.Collections;
using UnityEngine;
using TMPro; // Use this namespace if using TextMeshPro

public class LogManager : MonoBehaviour
{
    public TextMeshProUGUI logText; // Assign this in the inspector
    private float timeout = 5.0f; // Time in seconds before the log disappears

    private void Start()
    {
        ClearLog(); // Start with an empty log
    }

    public void AddMessage(string message)
    {
        logText.text += message + "\n"; // Append new message
       // StopCoroutine("HideLog"); // Stop previous timer
       // StartCoroutine("HideLog"); // Start a new timeout timer
        logText.gameObject.SetActive(true); // Ensure the text box is visible
    }

    private IEnumerator HideLog()
    {
        yield return new WaitForSeconds(timeout); // Wait for the timeout
        //ClearLog(); // Clear the log text
        logText.gameObject.SetActive(false); // Hide the text box
    }

    private void ClearLog()
    {
        logText.text = ""; // Clear the text
    }
}
