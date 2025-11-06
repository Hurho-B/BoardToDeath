using NUnit.Framework;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    public AudioSource MusicPlayer;
    public AudioClip[] Song = new AudioClip[10];

    public bool shouldPlayMusic = false;

    public void play(AudioClip[] s)
    {
        int songIndex = 0;

        shouldPlayMusic = true;

        shuffle(s);

        //Play music starting with first song in the array
        while (shouldPlayMusic)
        {
            if (!MusicPlayer.isPlaying)
            {
                if ((songIndex > s.Length) || (songIndex < 0))
                {
                    shuffle(s);
                    songIndex = 0;
                }

                MusicPlayer.PlayOneShot(s[songIndex]);
            }
        }
    }

    public void shuffle(AudioClip[] s)
    {
        //Shuffle the array using Fisher-Yates Shuffle
        for (int i = s.Length - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i);

            AudioClip temp = s[i];

            s[i] = s[rnd];
            s[rnd] = temp;
        }

        //Set the contents of the original song array to that of the newly-shuffled array 
        for (int i = 0; i < s.Length; i++)
        {
            Song[i] = s[i];
        }
    }
}
