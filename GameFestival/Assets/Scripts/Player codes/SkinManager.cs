using UnityEngine;

public class SkinManager : MonoBehaviour
{
    [Header("Available skins")]
    public SkinData[] skins;   

    const string PREFS_KEY_SELECTED = "SkinSelected";

    PlayerAnimation anim;
    int currentIndex = 0;

    void Awake()
    {
        anim = GetComponent<PlayerAnimation>();

        
        currentIndex = PlayerPrefs.GetInt(PREFS_KEY_SELECTED, 0);
        currentIndex = Mathf.Clamp(currentIndex, 0, skins.Length - 1);

      
        Equip(currentIndex);
    }

    
    public void Equip(int index)
    {
        if (index < 0 || index >= skins.Length) return;
        currentIndex = index;
        anim.SetSkin(skins[index]);

        
        PlayerPrefs.SetInt(PREFS_KEY_SELECTED, currentIndex);
        PlayerPrefs.Save();
    }

    public int Current => currentIndex;
}
