using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public int totalNumberOfCheckpoints;

    Vector3 m_lastCheckpointPosition;
    Vector3 m_lastCheckpointRotation;

    const string DLL_NAME = "GameEnginesMidtermDLL";

    [DllImport(DLL_NAME)]
    private static extern void ResetLogger();

    [DllImport(DLL_NAME)]
    private static extern void SaveCheckpointTime(float RTBC);

    [DllImport(DLL_NAME)]
    private static extern float GetTotalTime();

    [DllImport(DLL_NAME)]
    private static extern float GetCheckpointTime(int index);

    [DllImport(DLL_NAME)]
    private static extern int GetNumCheckpoints();

    float m_lastTime = 0f;

    void Start()
    {
        DontDestroyOnLoad(this);

        m_lastTime = Time.timeSinceLevelLoad;

        SceneManager.sceneLoaded += OnSceneLoaded;

        ResetCheckpointLogs();
    }

    public int NumCheckpointsReached()
    {
        return GetNumCheckpoints();
    }

    public float GetTimeForCheckpointIndex(int index)
    {
        if (index >= GetNumCheckpoints())
        {
            return -1f;
        }
        else
        {
            return GetCheckpointTime(index);
        }
    }

    public float GetTotalRunTime()
    {
        return GetTotalTime();
    }

    public void ResetCheckpointLogs()
    {
        ResetLogger();
    }

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
        m_lastCheckpointPosition = a_position;
        m_lastCheckpointRotation = a_rotation;

        if (isNewCheckpoint)
        {
            SaveCheckpointTime(Time.timeSinceLevelLoad);
            if (NumCheckpointsReached() >= totalNumberOfCheckpoints)
            {
                OnLastCheckpointReached();
            }
        }
    }

    void OnLastCheckpointReached()
    {
        SceneManager.LoadScene("EndScene");
    }

    void OnSceneLoaded(Scene a_scene, LoadSceneMode a_sceneMode)
    {
        if (a_scene.name == "PlayScene")
        {
            m_lastTime = Time.timeSinceLevelLoad;
            ResetCheckpointLogs();
        }
    }
}
