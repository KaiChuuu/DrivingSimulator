using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DashboardPanel : MonoBehaviour
{
    public TextMeshProUGUI songName; 
    public RectTransform textRectTransform;
    public float textScrollSpeed = 50f;
    float songNameWidth;
    float textLeftDistance;

    void Update()
    {
        textRectTransform.anchoredPosition += Vector2.left * textScrollSpeed * Time.deltaTime;

        if (textRectTransform.anchoredPosition.x < textLeftDistance)
        {
            textRectTransform.anchoredPosition = new Vector2(songNameWidth, textRectTransform.anchoredPosition.y);
        }
    }

    public void UpdateSongName(string name)
    {
        songName.text = name;
        songNameWidth = songName.preferredWidth;
        textLeftDistance = -songNameWidth + 100;
    }
}
