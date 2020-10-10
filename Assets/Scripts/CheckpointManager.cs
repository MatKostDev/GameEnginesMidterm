using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CheckpointManager : MonoBehaviour
{
    public int totalNumberOfCheckpoints;

    public CheckpointUIManager checkpointUI;

    [HideInInspector] public bool didGameJustFinish = false;

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

    void Start()
    {
        DontDestroyOnLoad(this);

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
        float newCheckpointTime = Time.timeSinceLevelLoad;

        m_lastCheckpointPosition = a_position;
        m_lastCheckpointRotation = a_rotation;

        if (isNewCheckpoint)
        {
            SaveCheckpointTime(newCheckpointTime);

            if (NumCheckpointsReached() >= totalNumberOfCheckpoints)
            {
                OnLastCheckpointReached();
            }
            else
            {
                checkpointUI.ActivateCheckpointsPopup(true, NumCheckpointsReached(), newCheckpointTime);
            }
        }
    }

    void OnLastCheckpointReached()
    {
        didGameJustFinish = true;
        SceneManager.LoadScene("EndScene");
    }

    void OnSceneLoaded(Scene a_scene, LoadSceneMode a_sceneMode)
    {
        if (a_scene.name == "PlayScene")
        {
            ResetCheckpointLogs();

            if (!checkpointUI)
            {
                checkpointUI = FindObjectOfType<CheckpointUIManager>();
            }
        }
    }
}
