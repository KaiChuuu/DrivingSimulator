using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    bool inAction = false;

    [SerializeField] AudioSource effectPlayer;
    [SerializeField] AudioClip effectAudio;

    private void Start()
    {
        effectPlayer = GetComponent<AudioSource>();
        effectPlayer.clip = effectAudio;
    }

    public void Restart()
    {
        if (!inAction)
        {
            inAction = true;

            effectPlayer.PlayOneShot(effectAudio);

            StartCoroutine(DelayRestart());
        }
    }

    private IEnumerator DelayRestart()
    {
        yield return new WaitForSeconds(1);

        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(currentSceneIndex);
    }
}
