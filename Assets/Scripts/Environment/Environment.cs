using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Environment : MonoBehaviour, IOnEventCallback
{
    [SerializeField] GameObject hitmarkerPrefab;
    [SerializeField] GameObject hitmarkerParent;

    [SerializeField] public bool isPenetrable = false;

    void OnEnable()
    {
        PhotonNetwork.AddCallbackTarget(this);
    }

    void OnDisable()
    {
        PhotonNetwork.RemoveCallbackTarget(this);
    }

    public void SpawnHitMarker(string environmentName, Vector3 position, Quaternion rotation)
    {
        object[] hitmarker = new object[] { environmentName, position, rotation };
        RaiseEventOptions options = new RaiseEventOptions()
        {
            Receivers = ReceiverGroup.All,
            CachingOption = EventCaching.DoNotCache
        };
        PhotonNetwork.RaiseEvent(99, hitmarker, options, SendOptions.SendUnreliable);
    }

    public void OnEvent(EventData photonEvent)
    {
        byte eventCode = photonEvent.Code;
        if (eventCode == 99)
        {
            object[] data = (object[])photonEvent.CustomData;
            string environmentName = (string)data[0];
            Vector3 position = (Vector3)data[1];
            Quaternion rotation = (Quaternion)data[2];
            if (environmentName == this.name)
            {
                Instantiate(hitmarkerPrefab, position, rotation, hitmarkerParent.transform);
            }
        }
    }
}
