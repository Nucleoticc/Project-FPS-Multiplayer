using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Transform cam;

    void Start()
    {
        cam = transform.root.GetComponentInChildren<Camera>().transform;
    }

    void LateUpdate()
    {
        if (cam == null)
        {
            cam = FindObjectOfType<Camera>().transform;
        }

        if (cam == null)
        {
            return;
        }

        transform.LookAt(transform.position + cam.forward);
    }
}
