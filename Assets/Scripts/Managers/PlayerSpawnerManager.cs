using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerSpawnerManager : MonoBehaviourPunCallbacks
{
    PhotonView pV;

    GameObject controller;

    void Awake()
    {
        pV = GetComponent<PhotonView>();
    }

    void Start()
    {
        if (pV.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        Transform spawnPoint = SpawnPointManager.instance.GetSpawnPoint();
        controller = PhotonNetwork.Instantiate(Path.Combine("Photon Prefabs", "PlayerController"), spawnPoint.position, spawnPoint.rotation, 0, new object[] { pV.ViewID });
    }

    public void Die()
    {
        Invoke("Respawn", 10f);
    }

    void Respawn()
    {
        PhotonNetwork.Destroy(controller);
        CreateController();
    }
}
