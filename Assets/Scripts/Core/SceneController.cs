using UnityEngine;

public class SceneController : MonoBehaviour
{
    public void LoadScene(int index)
    {
        // Load the scene with the given index
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(index);
    }
}
