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
 */

public class StartMenuScript : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void ViewPreviousStats()
    {
        if (FindObjectOfType<CheckpointManager>())
            SceneManager.LoadScene("EndScene");
    }
}
