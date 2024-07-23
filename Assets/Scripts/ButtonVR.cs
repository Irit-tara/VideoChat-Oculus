using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

public class ButtonVR : MonoBehaviour
{
    [SerializeField] private float threshold = .1f;
    [SerializeField] private float deadzone = .025f;
    public UnityEvent onPress;
    public UnityEvent onRelease;
   
    bool _ispressed;
    private Vector3 _startPosition;
    private ConfigurableJoint _joint;

    // Start is called before the first frame update
    void Start()
    {
        _ispressed = false;
        _startPosition = transform.localPosition;
        _joint = GetComponent<ConfigurableJoint>();

    }

     void Update()
    {
        if (!_ispressed && GetValue() + threshold >= 1)
        {
            Pressed();
        }
        if (_ispressed && GetValue() - threshold <= 1)
        {
            Rleased();
        }
         /*   button.transform.localPosition = new Vector3(0, 0.003f, 0);
            presser = other.gameObject;
            onPress.Invoke();
            _ispressed = true;
        }*/
    }
    private float GetValue()
    {
        var value = Vector3.Distance(_startPosition, transform.localPosition) / _joint.linearLimit.limit;
        if (Math.Abs(value) < deadzone)
            value = 0;

        return Mathf.Clamp(value, deadzone - 1f, 1f);
    }
  

    public void Pressed()
    {
        _ispressed = true;
        Debug.Log("Connect To Server");

    }
    private void Rleased()
    {
        _ispressed = false;
        onRelease.Invoke();
        Debug.Log("Rleased");
    }


}
