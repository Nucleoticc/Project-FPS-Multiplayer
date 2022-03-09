using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class ScoreboardItem : MonoBehaviour
{
    [SerializeField] Text playerNameText;
    [SerializeField] Text killsText;
    [SerializeField] Text deathsText;

    public void SetUp(string player)
    {
        playerNameText.text = player;
    }

    public void IncreaseKills()
    {
        int kills = int.Parse(killsText.text);
        killsText.text = (kills + 1).ToString();
    }

    public void IncreaseDeaths()
    {
        int deaths = int.Parse(deathsText.text);
        deathsText.text = (deaths + 1).ToString();
    }
}
