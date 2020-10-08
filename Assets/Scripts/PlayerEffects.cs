using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PlayerEffects : MonoBehaviour
{
    [Header("Death Effect")]
    [SerializeField] [Range(0, 1)] float onDeathVignetteIntensity;
    [SerializeField] Color onDeathVignetteColor;
    [SerializeField] float onDeathVignetteFadeSpeed;

    [Header("Checkpoint Reached Effect")]
    [SerializeField] [Range(0, 1)] float onCheckpointVignetteIntensity;
    [SerializeField] Color onCheckpointVignetteColor;
    [SerializeField] float onCheckpointVignetteFadeSpeed;

    Volume   m_postProcessVolume;
    Vignette m_vignette;

    float m_vignetteInitialIntensity;
    Color m_vignetteInitialColor;
    float m_vignetteFadeParam = 1f;

    float m_effectVignetteIntensity;
    Color m_effectVignetteColor;
    float m_effectVignetteFadeSpeed;

    void Start()
    {
        m_postProcessVolume = FindObjectOfType<Volume>();
        if (!m_postProcessVolume)
            throw new System.NullReferenceException(nameof(m_postProcessVolume));

        m_postProcessVolume.profile.TryGet(out m_vignette);

        m_vignetteInitialIntensity = m_vignette.intensity.value;
        m_vignetteInitialColor     = m_vignette.color.value;
    }

    void Update()
    {
        if (m_vignetteFadeParam < 1f)
        {
            m_vignetteFadeParam += m_effectVignetteFadeSpeed * Time.deltaTime;

            m_vignette.intensity.value = Mathf.Lerp(m_effectVignetteIntensity, m_vignetteInitialIntensity, m_vignetteFadeParam);
            m_vignette.color.value     = Color.Lerp(m_effectVignetteColor,     m_vignetteInitialColor,     m_vignetteFadeParam);
        }
    }

    public void OnDie()
    {
        m_vignette.intensity.value  = onDeathVignetteIntensity;
        m_vignette.color.value      = onDeathVignetteColor;

        m_effectVignetteIntensity = onDeathVignetteIntensity;
        m_effectVignetteColor     = onDeathVignetteColor;
        m_effectVignetteFadeSpeed = onDeathVignetteFadeSpeed;

        m_vignetteFadeParam = 0f;
    }

    public void OnCheckpointReached()
    {
        m_vignette.intensity.value  = onCheckpointVignetteIntensity;
        m_vignette.color.value      = onCheckpointVignetteColor;

        m_effectVignetteIntensity = onCheckpointVignetteIntensity;
        m_effectVignetteColor     = onCheckpointVignetteColor;
        m_effectVignetteFadeSpeed = onCheckpointVignetteFadeSpeed;

        m_vignetteFadeParam = 0f;
    }
}
