using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class PlayerListItem : MonoBehaviourPunCallbacks
{
    [SerializeField] Text playerNameText;
    Player player;

    public void SetUp(Player player)
    {
        this.player = player;
        playerNameText.text = player.NickName;
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer == player)
        {
            Destroy(gameObject);
        }
    }

    public override void OnLeftRoom()
    {
        Destroy(gameObject);
    }
}
