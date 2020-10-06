using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenuScript : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("PlayScene");
    }

    public void ViewPreviousStats()
    {
        SceneManager.LoadScene("EndScene");
    }
}
