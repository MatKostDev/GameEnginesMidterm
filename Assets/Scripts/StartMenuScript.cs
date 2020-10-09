using System;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * Mathew Kostrzewa
 * 100591924
 * Game Engines Midterm
 * Ontario Tech University
 * Fall 2020
 *
 * Assets used: https://assetstore.unity.com/packages/3d/environments/dungeons/dungeon-traps-50655
 *              https://assetstore.unity.com/packages/3d/environments/dungeons/decrepit-dungeon-lite-33936
 *              https://www.youtube.com/watch?v=tJkQs5xEhPU
 */

public class StartMenuScript : MonoBehaviour
{
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible   = true;
    }

    public void StartGame()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null); //reset in case a button was previously selected
        SceneManager.LoadScene("PlayScene");
    }

    public void ViewPreviousStats()
    {
        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null); //reset in case a button was previously selected

        if (FindObjectOfType<CheckpointManager>())
            SceneManager.LoadScene("EndScene");
    }
}
