using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimatedHandOnInput : MonoBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;
     public Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        if (animator == null)
        {
            Debug.LogError("Animator not assigned in " + gameObject.name);
        }
    }

    // Update is called once per frame
    void Update()
    {
        float pinch = pinchAnimationAction.action.ReadValue<float>();
        animator.SetFloat("Trigger", pinch);
        float grip = gripAnimationAction.action.ReadValue<float>();
        animator.SetFloat("Grip", grip);
    }
}
