using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance {get; private set;}
        public int coin; 
        public int grey_fish;
        public int green_fish;
        public int gold_fish;

        private void Awake() {
        if (Instance == null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else {
            Destroy(gameObject);
        }
    }
}
