using NUnit.Framework;
using UnityEngine;

public class MusicHandler : MonoBehaviour
{
    public AudioSource MusicPlayer;
    public AudioClip[] Songs = new AudioClip[10];

    public int songIndex = 0;

    public bool shouldPlayMusic = false;

    void Start()
    {
        shuffle(Songs);
    }

    void Update()
    {
        if (!MusicPlayer.isPlaying)
            play(Songs);
    }

    public void play(AudioClip[] s)
    {
        //Play music starting with first song in the array
        if (shouldPlayMusic)
        {
            //Only play a song if the music player is not already playing a song
            if (!MusicPlayer.isPlaying)
            {
                //Reshuffle and start over if the current playlist has already been played
                if ((songIndex >= s.Length) || (songIndex < 0))
                {
                    songIndex = 0;
                    shuffle(Songs);
                }

                MusicPlayer.PlayOneShot(s[songIndex]);
            }

            //Increment song index so the next song will be played
            songIndex++;
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
            Songs[i] = s[i];
        }
    }
}
