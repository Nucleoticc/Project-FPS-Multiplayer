using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class ExitGame : MonoBehaviourPunCallbacks
{
    public void LeaveRoom()
    {
        RoomManager.instance.LeaveRoom();
    }
}
