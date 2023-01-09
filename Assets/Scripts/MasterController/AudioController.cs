using UnityEngine;
using System.Collections;

public class AudioController : MonoBehaviour
{

    // Use this for initialization
    AudioSource audioSource_Background;
    AudioSource audioSource_OneShot;
    public float volumne = 1f;

    public bool isSoundOn;
    public bool isBackgroundMusicOn;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        if (Master.Audio == null)
        {
            Master.Audio = this;
        }
        
        if (audioSource_Background == null || audioSource_OneShot == null)
        {
            audioSource_Background = gameObject.GetComponents<AudioSource>()[0];
            audioSource_OneShot = gameObject.GetComponents<AudioSource>()[1];
        }
        ReadSetting();
    }

    void Start()
    {

    }

    void Update()
    {
        audioSource_Background.mute = !isBackgroundMusicOn;
        audioSource_OneShot.mute = !isSoundOn;
    }

    void ReadSetting()
    {
        isSoundOn = PlayerPrefs.GetInt("isSoundOn", 1) == 1 ? true : false;
        isBackgroundMusicOn = PlayerPrefs.GetInt("isBackgroundMusicOn", 1) == 1 ? true : false;
    }

    public void TurnOffSound()
    {
        isSoundOn = false;
        PlayerPrefs.SetInt("isSoundOn", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void TurnOnSound()
    {
        isSoundOn = true;
        PlayerPrefs.SetInt("isSoundOn", isSoundOn ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void ToggleSound()
    {
        if (isSoundOn)
        {
            TurnOffSound();
            return;
        }
        else
        {
            TurnOnSound();
            return;
        }
    }

    public void ToggleBackgroundMusic()
    {
        if (isBackgroundMusicOn)
        {
            TurnOffBackgroundMusic();
            return;
        }
        else
        {
            TurnOnBackgroundMusic();
            return;
        }
    }

    public void PlayBackgroundMusic(string musicName, float thisVolumne = -1, bool startAgainIfPlaying = true)
    {

        if (!startAgainIfPlaying)
        {
            if (audioSource_Background.isPlaying && audioSource_Background.clip.name == musicName)
            {
                return;
            }
        }

        AudioClip clip = Resources.Load<AudioClip>("Audios/Musics/" + musicName);
        audioSource_Background.clip = clip;
        audioSource_Background.loop = true;
        if (thisVolumne == -1)
        {
            audioSource_Background.volume = volumne;
        }
        else
        {
            audioSource_Background.volume = thisVolumne;
        }

        audioSource_Background.Play();
    }

    public void StopBackgroundMusic()
    {
        audioSource_Background.Stop();
    }

    public void TurnOnBackgroundMusic()
    {
        isBackgroundMusicOn = true;
        PlayerPrefs.SetInt("isBackgroundMusicOn", 1);
        PlayerPrefs.Save();
    }

    public void TurnOffBackgroundMusic()
    {
        isBackgroundMusicOn = false;
        PlayerPrefs.SetInt("isBackgroundMusicOn", 0);
        PlayerPrefs.Save();
    }

    public void PlaySound(string soundName, float thisVolumne = -1)
    {
        AudioClip clip = Resources.Load<AudioClip>("Audios/Sounds/" + soundName);
        float vol = 0;

        if (thisVolumne == -1)
        {
            vol = volumne;
        }
        else
        {
            vol = thisVolumne;
        }

        if (clip != null) audioSource_OneShot.PlayOneShot(clip, vol);
    }

    public void StopSound()
    {
        audioSource_OneShot.enabled = false;
        audioSource_OneShot.enabled = true;
    }



}
