using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class EndMenuScript : MonoBehaviour
{
    public List<TMP_Text> checkpointTimeDisplays = new List<TMP_Text>();

    [Header("On Game Completed Effect")]
    [SerializeField] [Range(0, 1)] float onFinishVignetteIntensity;
    [SerializeField] Color onFinishVignetteColor;
    [SerializeField] float onFinishVignetteFadeSpeed;

    Volume   m_postProcessVolume;
    Vignette m_vignette;

    float m_vignetteInitialIntensity;
    Color m_vignetteInitialColor;
    float m_vignetteFadeParam = 1f;

    CheckpointManager m_checkpointManager;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;

        m_postProcessVolume = FindObjectOfType<Volume>();
        if (!m_postProcessVolume)
            throw new System.NullReferenceException(nameof(m_postProcessVolume));

        m_postProcessVolume.profile.TryGet(out m_vignette);

        m_vignetteInitialIntensity  = m_vignette.intensity.value;
        m_vignetteInitialColor      = m_vignette.color.value;

        m_checkpointManager = FindObjectOfType<CheckpointManager>();

        SetCheckpointTimeDisplays();

        if (m_checkpointManager.didGameJustFinish)
        {
            m_checkpointManager.didGameJustFinish = false;

            m_vignette.intensity.value = onFinishVignetteIntensity;
            m_vignette.color.value     = onFinishVignetteColor;

            m_vignetteFadeParam = 0f;
        }
    }

    void Update()
    {
        if (m_vignetteFadeParam < 1f)
        {
            m_vignetteFadeParam += onFinishVignetteFadeSpeed * Time.deltaTime;

            m_vignette.intensity.value = Mathf.Lerp(onFinishVignetteIntensity, m_vignetteInitialIntensity, m_vignetteFadeParam);
            m_vignette.color.value     = Color.Lerp(onFinishVignetteColor,     m_vignetteInitialColor,     m_vignetteFadeParam);
        }
    }

    public void GoToStartMenu()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null); //reset in case a button was previously selected
        SceneManager.LoadScene("StartScene");
    }

    void SetCheckpointTimeDisplays()
    {
        for (int i = 0; i < checkpointTimeDisplays.Count; i++)
        {
            float checkpointTime = m_checkpointManager.GetTimeForCheckpointIndex(i);

            if (checkpointTime < 0f) //invalid
            {
                checkpointTimeDisplays[i].text = "DNF";
            }
            else
            {
                float minutes = Mathf.Floor(checkpointTime / 60);
                float seconds = Mathf.RoundToInt(checkpointTime % 60);

                if (seconds < 10f)
                {
                    checkpointTimeDisplays[i].text = minutes.ToString() + ":0" + seconds.ToString();
                }
                else
                {
                    checkpointTimeDisplays[i].text = minutes.ToString() + ":" + seconds.ToString();
                }
            }
        }
    }
}
