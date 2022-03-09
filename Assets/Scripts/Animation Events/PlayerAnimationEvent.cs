using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAnimationEvent : MonoBehaviour
{
    PlayerMovementHandler playerMovementHandler;
    PlayerAudioHandler audioHandler;
    PhotonView pV;

    [SerializeField] AudioSource audioSource;

    void Awake()
    {
        playerMovementHandler = GetComponent<PlayerMovementHandler>();
        audioHandler = GetComponent<PlayerAudioHandler>();
        pV = GetComponent<PhotonView>();
    }

    public void PlayFootStepsRunning()
    {
        if (playerMovementHandler.isGrounded)
        {
            pV.RPC(nameof(RPC_PlayFootSteps), RpcTarget.All, 0.7f);
        }
    }

    public void PlayFootStepsWalking()
    {
        if (playerMovementHandler.isGrounded)
        {
            pV.RPC(nameof(RPC_PlayFootSteps), RpcTarget.All, 0.3f);
        }
    }

    [PunRPC]
    public void RPC_PlayFootSteps(float volume)
    {
        if (pV.IsMine)
        {
            if (audioHandler.isFlashed)
            {
                return;
            }
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
            audioSource.volume = volume;
            audioSource.Play();
        }
    }
}
