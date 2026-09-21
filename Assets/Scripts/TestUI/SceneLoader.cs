using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private const string DeathScene = "DeathScene";
    private const string EnemyAI = "EnemyAI";

    public void LoadDeathScene()
    {
        SceneManager.LoadScene(DeathScene);
    }

    public void ReplayGame()
    {
        SceneManager.LoadScene(EnemyAI);
    }
}