using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCtrl : MonoBehaviour
{
    public enum Sfxs {pigipop, upgrade, buildComplete, levelUp, newPigiFound, correct, yeah, }
    public Sfxs sfx;

    [SerializeField] AudioSource source;
    [SerializeField] AudioClip[] audioClips;

    public void PlaySFX(int idx) {
        source.PlayOneShot(audioClips[idx]);
    }
}
