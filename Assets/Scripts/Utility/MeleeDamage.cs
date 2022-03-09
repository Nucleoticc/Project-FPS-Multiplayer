using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class MeleeDamage : MonoBehaviour
{
    Collider meleeDamageCollider;

    int damageOnAttack;

    PhotonView pV;

    void Start()
    {
        meleeDamageCollider = GetComponentInChildren<Collider>();
        pV = GetComponentInParent<PhotonView>();
    }

    void OnTriggerEnter(Collider other)
    {
        other.GetComponent<IDamageable>()?.TakeDamage(damageOnAttack, pV.Owner.NickName, "Axe");
    }

    public void ActivateCollider(int damage)
    {
        damageOnAttack = damage;
        meleeDamageCollider.enabled = true;
    }

    public void DeactivateCollider()
    {
        meleeDamageCollider.enabled = false;
    }
}
