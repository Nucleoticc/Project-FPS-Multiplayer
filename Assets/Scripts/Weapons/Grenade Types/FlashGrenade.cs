using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FlashGrenade : Grenade
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Start()
    {
        if (!pV.IsMine)
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
        CheckVisibilityAndApplyFlash();
    }

    void CheckVisibilityAndApplyFlash()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, 25f);
        foreach (Collider collider in colliders)
        {
            float distance = Vector3.Distance(transform.position, collider.transform.position) / explosionRadius;
            FlashBlindness flashBlindness = collider.GetComponentInChildren<FlashBlindness>();
            if (flashBlindness != null)
            {
                PhotonView colliderPV = collider.gameObject.GetPhotonView();
                pV.RPC("RPC_ApplyFlash", colliderPV.Owner, distance, colliderPV.ViewID);
            }
        }
    }

    [PunRPC]
    void RPC_ApplyFlash(float distance, int targetView)
    {
        PhotonView target = PhotonView.Find(targetView);

        if (target != null)
        {
            FlashBlindness flashBlindness = target.GetComponentInChildren<FlashBlindness>();
            flashBlindness.GoBlind(distance);
        }
    }
}