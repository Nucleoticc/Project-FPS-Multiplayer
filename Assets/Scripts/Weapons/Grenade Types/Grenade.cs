using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Photon.Pun;

public class Grenade : MonoBehaviour
{
    public float delay { get; set; }
    public string explosionVFX { get; set; }
    public float explosionRadius { get; set; }
    public int baseDamage { get; set; }
    public string ownerNickname { get; set; }
    
    public bool isStillInHand = true;

    protected GameObject instantiatedVFX;

    protected Camera cam;

    protected PhotonView pV;

    protected virtual void Awake()
    {
        pV = GetComponent<PhotonView>();
    }

    protected virtual void Start()
    {
        cam = Camera.main;
    }

    protected virtual void Explode()
    {
        Vector3 instantiatedVFXPosition = transform.position;

        if (isStillInHand)
        {
            instantiatedVFXPosition = cam.transform.position;
        }

        // instantiatedVFX = Instantiate(explosionVFX, instantiatedVFXPosition + new Vector3(0, 0.5f, 0), Quaternion.identity);
        instantiatedVFX = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", explosionVFX), instantiatedVFXPosition + new Vector3(0, 0.5f, 0), Quaternion.identity);
    }

    public virtual void Throw(float throwForce) { }
}
