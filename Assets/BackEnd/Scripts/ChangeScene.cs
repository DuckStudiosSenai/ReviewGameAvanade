using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    public void ChangeSceneMethod(string sceneName)
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneName);
    }
}
