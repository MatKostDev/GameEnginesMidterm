using UnityEngine;

public class LerpBetweenTwoPoints : MonoBehaviour
{
    public Vector3 startPosition;
    public Vector3 endPosition;
    public float   speed;

    float m_interpolationParam;

    bool m_isMovingToEnd = true;

    Transform m_transform;
    
    void Start()
    {
        m_transform = transform;

        m_transform.position = startPosition;
    }

    void Update()
    {
        m_interpolationParam += speed * Time.deltaTime;
        if (m_interpolationParam < 0f || m_interpolationParam > 1f)
        {
            m_interpolationParam = 0f;
            m_isMovingToEnd      = !m_isMovingToEnd;
        }

        if (m_isMovingToEnd)
        {
            m_transform.position = Vector3.Lerp(startPosition, endPosition, m_interpolationParam);
        }
        else
        {
            m_transform.position = Vector3.Lerp(endPosition, startPosition, m_interpolationParam);
        }
    }
}
