using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotation : MonoBehaviour
{
    [SerializeField] private int rotationAngle = 1;

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(rotationAngle, rotationAngle, rotationAngle, Space.Self);
    }
}
