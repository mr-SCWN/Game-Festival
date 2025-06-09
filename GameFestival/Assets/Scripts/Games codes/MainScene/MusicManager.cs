using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;
    private AudioSource _audio;

    private const string MainSceneName = "Main Game Map";

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // getting AudioSource
        _audio = GetComponent<AudioSource>();
        if (_audio == null)
        {
            Debug.LogError("MusicManager: нет AudioSource на объекте!");
            return;
        }

        _audio.loop = true;
        _audio.playOnAwake = false;
        _audio.volume = 1f;

        
        if (SceneManager.GetActiveScene().name == MainSceneName)
            _audio.Play();

        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // after loading new scene
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (_audio == null) return;

        if (scene.name == MainSceneName)
        {
            // main scene - continue music
            if (!_audio.isPlaying)
                _audio.Play();
        }
        else
        {
            // another scene - pause music
            if (_audio.isPlaying)
                _audio.Pause();
        }
    }
}
