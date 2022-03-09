using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ScoreboardUIManager : MonoBehaviourPunCallbacks
{
    [Header("Scoreboard UI")]
    [SerializeField] Transform scoreboardContent;
    [SerializeField] GameObject scoreboardItemPrefab;

    public static ScoreboardUIManager instance;

    Dictionary<string, ScoreboardItem> scoreboardItems = new Dictionary<string, ScoreboardItem>();

    PhotonView pV;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        pV = GetComponent<PhotonView>();
    }

    void Start()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            AddScoreBoardItem(player.NickName);
        }
    }

    public void ShowScoreboard()
    {
        scoreboardContent.gameObject.SetActive(true);
    }

    public void HideScoreboard()
    {
        scoreboardContent.gameObject.SetActive(false);
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        AddScoreBoardItem(newPlayer.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        RemoveScoreBoardItem(otherPlayer.NickName);
    }

    void AddScoreBoardItem(string player)
    {
        ScoreboardItem item = Instantiate(scoreboardItemPrefab, scoreboardContent).GetComponent<ScoreboardItem>();
        item.SetUp(player);
        scoreboardItems.Add(player, item);
    }

    void RemoveScoreBoardItem(string player)
    {
        Destroy(scoreboardItems[player].gameObject);
        scoreboardItems.Remove(player);
    }

    public void UpdateScoreBoardItem(string killer, string victim)
    {
        pV.RPC(nameof(RPC_UpdateScoreBoardItem), RpcTarget.All, killer, victim);
    }

    [PunRPC]
    public void RPC_UpdateScoreBoardItem(string killer, string victim)
    {
        if (!killer.Equals(victim))
        {
            if (!killer.Equals("Void Zone"))
            {
                scoreboardItems[killer].IncreaseKills();
            }
        }
        scoreboardItems[victim].IncreaseDeaths();
    }
}
