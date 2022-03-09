using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillFeedItem : MonoBehaviour
{
    [SerializeField] Text killerNameText;
    [SerializeField] Text killedNameText;
    [SerializeField] Image weaponImage;

    public void SetUp(string killerName, string killedName, string weaponSprite)
    {
        killerNameText.text = killerName;
        killedNameText.text = killedName;
        weaponImage.sprite = Resources.Load<Sprite>($"WeaponIcons/{weaponSprite}");
    }
}
