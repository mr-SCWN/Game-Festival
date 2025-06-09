using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
    [Tooltip("Main Scene Name")]
    public string mainSceneName = "Main Game Map";

    void Start()
    {
        SceneManager.LoadScene(mainSceneName);
    }
}
