using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public int totalNumberOfCheckpoints;

    Vector3 m_lastCheckpointPosition;
    Vector3 m_lastCheckpointRotation;

    List<float> m_checkpointTimes = new List<float>();

    int m_numberofCheckpointsReached = 0;

    public Vector3 GetLastCheckpointPosition()
    {
        return m_lastCheckpointPosition;
    }

    public Vector3 GetLastCheckpointRotation()
    {
        return m_lastCheckpointRotation;
    }

    public void SetCheckpoint(Vector3 a_position, Vector3 a_rotation, bool isNewCheckpoint = true)
    {
        m_checkpointTimes.Add(Time.timeSinceLevelLoad);

        m_lastCheckpointPosition = a_position;
        m_lastCheckpointRotation = a_rotation;

        if (isNewCheckpoint)
        {
            m_numberofCheckpointsReached++;
            if (m_numberofCheckpointsReached >= totalNumberOfCheckpoints)
            {
                OnLastCheckpointReached();
            }
        }
    }

    void OnLastCheckpointReached()
    {
        SceneManager.LoadScene("EndScene");
    }
}
