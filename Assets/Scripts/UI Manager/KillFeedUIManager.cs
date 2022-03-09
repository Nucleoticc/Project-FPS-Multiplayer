using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class KillFeedUIManager : MonoBehaviour
{
    [Header("Killfeed UI")]
    [SerializeField] Transform killfeedContent;
    [SerializeField] GameObject killfeedItemPrefab;

    public static KillFeedUIManager instance;

    PhotonView pV;

    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }

        pV = GetComponent<PhotonView>();
    }

    public void AddKillfeedItem(string attacker, string victim, string weapon)
    {
        pV.RPC("RPC_AddKillfeedItem", RpcTarget.All, attacker, victim, weapon);
    }

    [PunRPC]
    public void RPC_AddKillfeedItem(string attacker, string victim, string weapon)
    {
        GameObject killfeedItem = Instantiate(killfeedItemPrefab, killfeedContent);
        killfeedItem.GetComponent<KillFeedItem>().SetUp(attacker, victim, weapon);
    }
}
