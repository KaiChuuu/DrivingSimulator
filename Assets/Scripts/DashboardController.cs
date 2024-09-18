using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class DashboardController : MonoBehaviour
{
    [SerializeField] GameObject activePanel;
    [SerializeField] AudioSource musicPlayer;
    [SerializeField] List<List<Song>> tracks = new List<List<Song>>();
    [SerializeField] List<Song> track1;
    [SerializeField] List<Song> track2;
    [SerializeField] List<Song> track3;

    [SerializeField] AudioSource effectPlayer;
    [SerializeField] AudioClip effectAudio;
    bool ignoreEffect = false;

    bool active = false;
    bool inAction = false;

    private float cooldown = 0.5f;
    private float timer = 0f;

    private int currTrack = 0;
    private int currSong = 0;

    void Start()
    {
        tracks.Add(track1);
        tracks.Add(track2);
        tracks.Add(track3);

        effectPlayer = GetComponent<AudioSource>();
        effectPlayer.clip = effectAudio;

        musicPlayer.clip = tracks[0][currSong].songData;

        activePanel.SetActive(false);
    }

    void Update()
    {
        if (inAction)
        {
            timer -= 0.01f;
            if(timer <= 0f)
            {
                inAction = false;
            }
        }

        if(active && musicPlayer.time >= musicPlayer.clip.length) 
        {
            ignoreEffect = true;
            UpdateSong("next");
        }
    }

    public void UpdatePanelState()
    {
        if (!inAction)
        {
            inAction = true;
            timer = cooldown;

            effectPlayer.PlayOneShot(effectAudio);

            if (active)
            {
                active = false;
                activePanel.SetActive(false);
                musicPlayer.Stop();
            }
            else
            {
                active = true;
                activePanel.SetActive(true);
                PlayCurrentSong();
            }
        }
    }

    public void UpdateSong(string status)
    {
        if (!inAction && active)
        {
            inAction = true;
            timer = cooldown;

            effectPlayer.PlayOneShot(effectAudio);

            if(!ignoreEffect)
            {
                effectPlayer.PlayOneShot(effectAudio);
                ignoreEffect = false;
            }

            switch (status)
            {
                case "prev":
                    if (currSong <= 0)
                    {
                        currSong = tracks[currTrack].Count - 1;
                    }
                    else
                    {
                        currSong--;
                    }
                    break;
                case "next":
                    if (currSong >= tracks[currTrack].Count - 1)
                    {
                        currSong = 0;
                    }
                    else
                    {
                        currSong++;
                    }
                    break;
            }
            PlayCurrentSong();
        }
    }

    public void UpdateTrack()
    {
        if (!inAction && active)
        {
            inAction = true;
            timer = cooldown;

            effectPlayer.PlayOneShot(effectAudio);

            if (!ignoreEffect)
            {
                effectPlayer.PlayOneShot(effectAudio);
                ignoreEffect = false;
            }

            if(currTrack == tracks.Count - 1)
            {
                currTrack = 0;
            }else
            {
                currTrack++;
            }
            currSong = 0;
            PlayCurrentSong();
        }
    }

    void PlayCurrentSong()
    {
        musicPlayer.Stop();
        musicPlayer.clip = tracks[currTrack][currSong].songData;
        activePanel.GetComponent<DashboardPanel>().UpdateSongName(tracks[currTrack][currSong].name);
        musicPlayer.Play();
    }
}
