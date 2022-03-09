using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAudioHandler : MonoBehaviour
{
    [SerializeField] AudioSource audioSource;

    Dictionary<string, AudioClip> audioClips = new Dictionary<string, AudioClip>();

    PhotonView pV;

    public bool isFlashed = false;

    void Awake()
    {
        pV = GetComponent<PhotonView>();
    }

    public void AddSoundToDictionary(string key, AudioClip audioClip)
    {
        audioClips.Add(key, audioClip);
    }

    public void PlayAudio(string audioClip)
    {
        if (isFlashed)
        {
            return;
        }
        pV.RPC(nameof(RPC_PlayAudio), RpcTarget.All, audioClip);
    }

    [PunRPC]
    public void RPC_PlayAudio(string audioClip)
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        if (pV.IsMine)
        {
            audioSource.PlayOneShot(audioClips[audioClip]);
        }
    }

    public void PlayFlashBangAudio(AudioClip audioClip, float duration)
    {
        isFlashed = true;
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
        float newTime  = audioClip.length - duration;
        audioSource.PlayOneShot(audioClip);
        audioSource.time = newTime;
        Invoke(nameof(StopAudio), duration);
    }

    void StopAudio()
    {
        audioSource.Stop();
        isFlashed = false;
    }
}
