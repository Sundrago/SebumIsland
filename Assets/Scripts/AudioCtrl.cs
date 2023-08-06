using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCtrl : MonoBehaviour
{
    public enum SFX_tag {pigipop, upgrade, buildComplete, levelUp, newPigiFound, correct, yeah, }

    [SerializeField] AudioSource source;
    [SerializeField] AudioClip[] audioClips;

    public void PlaySFX(int idx) {
        source.PlayOneShot(audioClips[idx]);
    }

    public void PlaySFXbyTag(SFX_tag tag)
    {
        source.PlayOneShot(audioClips[(int)tag]);
    }
}
