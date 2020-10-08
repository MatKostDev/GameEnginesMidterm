using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.UI;

public class CheckpointUIManager : MonoBehaviour
{
    public List<Image> checkpointIcons = new List<Image>();

    public float checkpointIconFadeSpeed;

    List<TMP_Text> m_checkpointTimesText = new List<TMP_Text>();

    float m_checkpointIconFadeParam = 0f;

    void Start()
    {
        foreach (var icon in checkpointIcons)
        {
            m_checkpointTimesText.Add(icon.GetComponentInChildren<TMP_Text>());
            icon.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (m_checkpointIconFadeParam < 1f)
        {
            m_checkpointIconFadeParam += checkpointIconFadeSpeed * Time.deltaTime;

            float newAlpha = Mathf.Lerp(6f, 0f, m_checkpointIconFadeParam);

            Color newColor = checkpointIcons[0].color;
            newColor.a     = newAlpha;

            for (int i = 0; i < checkpointIcons.Count; i++)
            {
                m_checkpointTimesText[i].alpha = newAlpha;
                checkpointIcons[i].color       = newColor;
            }
        }
    }

    public void ActivateCheckpointsPopup(int a_numCheckpointsReached, float a_newestCheckpointTime)
    {
        for (int i = 0; i < a_numCheckpointsReached; i++)
        {
            checkpointIcons[i].gameObject.SetActive(true);

            m_checkpointIconFadeParam = 0f;

            if (i != a_numCheckpointsReached - 1)
                continue;

            float minutes = Mathf.Floor(a_newestCheckpointTime / 60);
            float seconds = Mathf.RoundToInt(a_newestCheckpointTime % 60);

            if (seconds < 10f)
            {
                m_checkpointTimesText[i].text = minutes.ToString() + ":0" + seconds.ToString();
            }
            else
            {
                m_checkpointTimesText[i].text = minutes.ToString() + ":" + seconds.ToString();
            }
        }
    }
}
