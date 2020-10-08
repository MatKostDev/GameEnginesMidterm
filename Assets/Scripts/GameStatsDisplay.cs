using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStatsDisplay : MonoBehaviour
{
    CheckpointManager m_checkpointManager;

    void Start()
    {
        m_checkpointManager = FindObjectOfType<CheckpointManager>();

        for (int i = 0; i < m_checkpointManager.NumCheckpointsReached(); i++)
        {
            Debug.Log((i + 1).ToString() + ": " + m_checkpointManager.GetTimeForCheckpointIndex(i));
        }
    }
    
    void Update()
    {
        
    }
}
