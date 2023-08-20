using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Sirenix.OdinInspector;

public enum SFX_tag {pigipop, upgrade, buildComplete, levelUp, newPigiFound, correct, yeah, }

public class AudioCtrl : SerializedMonoBehaviour
{
    public static AudioCtrl Instance;

    [SerializeField] AudioSource sfx_source, bgm_source;

    [TableList(ShowIndexLabels = true)]
    [SerializeField] AudioData[] audioDatas;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("settings_sfx_voulume"))
        {
            PlayerPrefs.SetFloat("settings_sfx_voulume", 0.8f);
            PlayerPrefs.SetFloat("settings_bgm_voulume", 0.8f);
        }

        SetVolume();
    }

    public void PlaySFXbyTag(SFX_tag tag)
    {
        foreach(AudioData data in audioDatas)
        {
            if(data.tag == tag)
            {
                sfx_source.PlayOneShot(data.src);
            }
        }
        //sfx_source.PlayOneShot(audioClips[(int)tag]);
    }

    public void SetVolume()
    {
        sfx_source.volume = PlayerPrefs.GetFloat("settings_sfx_voulume");
        bgm_source.volume = PlayerPrefs.GetFloat("settings_bgm_voulume");
    }

    [Serializable]
    public class AudioData
    {
        public SFX_tag tag;
        public AudioClip src;
    }

}
