using UnityEngine;

public class SkinManager : MonoBehaviour
{
    [Header("Available skins")]
    public SkinData[] skins;   // проставили в инспекторе четырьмя ассетами

    PlayerAnimation anim;
    int currentIndex = 0;

    void Awake() {
      anim = GetComponent<PlayerAnimation>();
      Equip(0);
    }

    // вызывайте этот метод, чтобы сменить скин:
    public void Equip(int index) {
      if (index < 0 || index >= skins.Length) return;
      currentIndex = index;
      anim.SetSkin(skins[index]);
    }

    public int Current => currentIndex;
}