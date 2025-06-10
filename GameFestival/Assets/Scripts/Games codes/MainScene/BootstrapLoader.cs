using UnityEngine;
using UnityEngine.SceneManagement;

public class BootstrapLoader : MonoBehaviour
{
    [Tooltip("Main Scene Name")]
    public string mainSceneName = "Main Game Map";

    void Awake()
    {
        PlayerPrefs.DeleteKey("SkinPurchased");
        PlayerPrefs.DeleteKey("SkinCount");
        PlayerPrefs.DeleteKey("SkinSelected"); 
        PlayerPrefs.Save();

        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        SceneManager.LoadScene(mainSceneName);
    }
}
