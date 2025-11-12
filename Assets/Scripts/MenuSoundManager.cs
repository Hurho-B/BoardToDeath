using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuSoundManager : MonoBehaviour
{
    public AudioSource MenuSoundPlayer;
    public AudioClip hover;
    public AudioClip select;
    public AudioClip back;
    public AudioClip pause;
    public AudioClip unpause;
    public AudioClip master;
    public AudioClip sfx;
    public AudioClip music;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MenuSoundPlayer.loop = false;
    }

    public void PlayHoverSound()
    {
        MenuSoundPlayer.PlayOneShot(hover);
    }

    public void PlayPauseSound()
    {
        MenuSoundPlayer.PlayOneShot(pause);
    }

    public void PlayUnpauseSound()
    {
        MenuSoundPlayer.PlayOneShot(unpause);
    }

    public void PlaySelectSound()
    {
        if (MenuSoundPlayer.isPlaying)
        {
            MenuSoundPlayer.Stop();
            MenuSoundPlayer.PlayOneShot(select);
        }
        else
        {
            MenuSoundPlayer.PlayOneShot(select);
        }
    }

    public void PlayBackSound()
    {
        MenuSoundPlayer.PlayOneShot(back);
    }

    public void PlayMasterSound()
    {
        MenuSoundPlayer.PlayOneShot(master);
    }

    public void PlaySFXSound()
    {
        if (MenuSoundPlayer.isPlaying)
        {
            MenuSoundPlayer.Stop();
            MenuSoundPlayer.PlayOneShot(sfx);
        }
        else
        {
            MenuSoundPlayer.PlayOneShot(sfx);
        }
    }

    public void PlayMusicSound()
    {
        MenuSoundPlayer.PlayOneShot(music);
    }
}
