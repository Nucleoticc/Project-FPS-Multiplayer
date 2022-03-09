using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FragGrenade : Grenade
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

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (Collider nearbyObject in colliders)
        {
            if (IsBehindCover(transform.position, nearbyObject.transform.position))
            {
                continue;
            }

            int damage = CalculateDamage(baseDamage, Vector3.Distance(transform.position, nearbyObject.transform.position) / explosionRadius);

            nearbyObject.GetComponent<IDamageable>()?.TakeDamage(damage, ownerNickname, "Frag 1", true);

            Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(1000f, transform.position, 5f);
            }
        }

        // Destroy(gameObject);
        // Destroy(instantiatedVFX, 2f);
    }

    int CalculateDamage(int damage, float distanceFromGrenade)
    {
        if (distanceFromGrenade <= 0.4 * explosionRadius)
        {
            return damage;
        }
        float distanceFactor = 1 - Mathf.Pow(distanceFromGrenade, 2);
        return Mathf.RoundToInt(damage * distanceFactor);
    }

    bool IsBehindCover(Vector3 grenadePosition, Vector3 enemyPosition)
    {
        RaycastHit hit;
        if (Physics.Raycast(enemyPosition, grenadePosition - enemyPosition, out hit, Vector3.Distance(grenadePosition, enemyPosition)))
        {
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Environment"))
            {
                return true;
            }
        }
        return false;
    }
}
