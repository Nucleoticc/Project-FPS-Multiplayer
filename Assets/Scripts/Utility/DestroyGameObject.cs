using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyGameObject : MonoBehaviour
{
    [SerializeField] float delay;

    void Start()
    {
        Invoke("DestroyObject", delay);
    }

    void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}
