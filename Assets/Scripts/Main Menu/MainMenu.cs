using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject ControlPanel;

    public void StartGame()
    {
        SceneManager.LoadScene("Map");
    }

    public void ControlsOpen()
    {
        ControlPanel.SetActive(true);
    }

    public void ControlsClose()
    {
        ControlPanel.SetActive(false);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
