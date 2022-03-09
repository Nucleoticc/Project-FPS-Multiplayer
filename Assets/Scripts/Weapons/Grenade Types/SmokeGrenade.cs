using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeGrenade : Grenade
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        if(!pV.IsMine)
        {
            return;
        }

        base.Start();
        Invoke("Explode", delay);
    }

    public override void Throw(float throwForce)
    {
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(transform.forward * throwForce, ForceMode.Force);
    }

    protected override void Explode()
    {
        base.Explode();
    }
}
