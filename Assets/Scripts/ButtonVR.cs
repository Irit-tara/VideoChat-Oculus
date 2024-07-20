using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonVR : MonoBehaviour
{
    public GameObject button;
    public UnityEvent onPress;
    public UnityEvent onRelease;
    GameObject presser;
    bool ispressed;

    // Start is called before the first frame update
    void Start()
    {
        ispressed = false;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (!ispressed)
        {
            button.transform.localPosition = new Vector3(0, 0.003f, 0);
            presser = other.gameObject;
            onPress.Invoke();
            ispressed = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == presser)
        {
            button.transform.localPosition = new Vector3(0, 0.015f, 0);
            onRelease.Invoke();
            ispressed=false;
        }
    }

    public void connectedToServer()
    {
        Debug.Log("Connect To Server");

    }


}
