using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [SerializeField]
    private AudioMixer mixer;
    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private AudioClip[] _bgClips;
    // Start is called before the first frame update
    void Awake()
    {
        Init();
    }

    void Init()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        _audioSource = this.gameObject.GetComponent<AudioSource>();
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        for(int i = 0; i < _bgClips.Length; i++)
        {
            if(arg0.name == _bgClips[i].name)
            {
                BgSoundPlay(_bgClips[i]);
            }
        }
    }

    public void BGSoundVolume(float val)
    {
        mixer.SetFloat("BG", Mathf.Log10(val) * 20);
    }

    public void SFXSoundVolume(float val)
    {
        mixer.SetFloat("SFX", Mathf.Log10(val) * 20);
    }

    public void PlaySFX(string sfxName)
    {
        GameObject soundObject = new GameObject(sfxName + "Sound");
        AudioSource soundObjectAudioSource = soundObject.AddComponent<AudioSource>();
        AudioClip playClip = Resources.Load<AudioClip>("Sound/" + sfxName);
        soundObjectAudioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("SFX")[0];
        soundObjectAudioSource.clip = playClip;
        soundObjectAudioSource.Play();

        Destroy(soundObject, playClip.length);
    }

    public void BgSoundPlay(AudioClip clip)
    {
        _audioSource.outputAudioMixerGroup = mixer.FindMatchingGroups("BG")[0];
        _audioSource.clip = clip;
        _audioSource.loop = true;
        _audioSource.volume = 0.1f;
        _audioSource.Play();
    }
}
