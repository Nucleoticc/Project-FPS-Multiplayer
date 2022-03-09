using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Realtime;

public class RoomListItem : MonoBehaviour
{
    [SerializeField] Text roomNameText;
    
    public RoomInfo info;

    public void SetUp(RoomInfo info)
    {
        roomNameText.text = info.Name;

        this.info = info;
    }

    public void OnClick()
    {
        Launcher.instance.JoinRoom(info);
    }
}
