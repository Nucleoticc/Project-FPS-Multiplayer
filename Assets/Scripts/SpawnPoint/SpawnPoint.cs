using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [SerializeField] GameObject graphics;

    public bool IsSpawnable { get; private set; } = true;

    void Awake()
    {
        graphics.SetActive(false);
    }

    void Update()
    {
        //Detect if a player is in range with a spherecast
        if (Physics.CheckSphere(transform.position, 10f, 1 << LayerMask.NameToLayer("Player")))
        {
            IsSpawnable = false;
        }
        else
        {
            IsSpawnable = true;
        }
    }
}
