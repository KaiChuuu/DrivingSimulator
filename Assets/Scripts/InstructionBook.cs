using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstructionBook : MonoBehaviour
{
    [SerializeField] GameObject instructionBook;

    [SerializeField] AudioSource effectPlayer;
    [SerializeField] AudioClip songData;

    bool inAction = false;

    private float cooldown = 0.5f;
    private float timer = 0f;

    private void Start()
    {
        effectPlayer = GetComponent<AudioSource>();
        effectPlayer.clip = songData;
    }

    private void Update()
    {
        if (inAction)
        {
            timer -= 0.01f;
            if (timer <= 0f)
            {
                inAction = false;
            }
        }
    }

    public void ReadInstructions()
    {
        if (!inAction)
        {
            inAction = true;
            timer = cooldown;

            instructionBook.SetActive(true);
            PlayPageFlipAudio();
        }
    }

    public void CloseBook()
    {
        if (!inAction)
        {
            inAction = true;
            timer = cooldown;

            instructionBook.SetActive(false);
            PlayPageFlipAudio();
        }
    }

    void PlayPageFlipAudio()
    {
        effectPlayer.PlayOneShot(songData);
    }
}
