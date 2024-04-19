using System;
using Sirenix.OdinInspector;
using UnityEngine;

public enum SFX_tag
{
    Pigipop,
    Upgrade,
    BuildComplete,
    LevelUp,
    NewPigiFound,
    Correct,
    Yeah,
    SmallPopPop,
    BGMMain,
    QuestArrive,
    QuestClear
}

/// <summary>
///     Manages audio playback for the game.
/// </summary>
public class AudioManager : SerializedMonoBehaviour
{
    [SerializeField] private AudioSource sfx_source, bgm_source;

    [TableList(ShowIndexLabels = true)] [SerializeField]
    private AudioData[] audioDatas;

    private AudioData bgmPlaying;
    private float bgmVolume = 0.8f;

    private float sfxVolume = 0.8f;
    public static AudioManager Instance { get; private set; }

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
        foreach (var data in audioDatas)
            if (data.tag == tag)
                sfx_source.PlayOneShot(data.src, data.volume * sfxVolume);
        //sfx_source.PlayOneShot(audioClips[(int)tag]);
    }

    public void SetVolume()
    {
        sfxVolume = PlayerPrefs.GetFloat("settings_sfx_voulume");
        bgmVolume = PlayerPrefs.GetFloat("settings_bgm_voulume");
        sfx_source.volume = PlayerPrefs.GetFloat("settings_sfx_voulume");

        if (bgmPlaying == null) bgm_source.volume = PlayerPrefs.GetFloat("settings_bgm_voulume");
        else bgm_source.volume = PlayerPrefs.GetFloat("settings_bgm_voulume") * bgmPlaying.volume;
    }

    public void PlayBGM(SFX_tag tag)
    {
        foreach (var data in audioDatas)
            if (data.tag == tag)
            {
                bgmPlaying = data;
                bgm_source.clip = data.src;
                bgm_source.volume = data.volume * bgmVolume;
                bgm_source.Play();
                return;
            }
    }

    [Serializable]
    public class AudioData
    {
        public SFX_tag tag;
        public AudioClip src;

        [Range(0f, 1f)] public float volume = 0.8f;
    }
}