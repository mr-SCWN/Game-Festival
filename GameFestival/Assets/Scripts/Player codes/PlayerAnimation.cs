using UnityEngine;
[RequireComponent(typeof(SpriteRenderer), typeof(PlayerController))]
public class PlayerAnimation : MonoBehaviour
{
    SpriteRenderer sr;
    PlayerController pc;
    SkinData skin;
    float frameTimer;
    int   frameIndex;

    void Awake() {
      sr = GetComponent<SpriteRenderer>();
      pc = GetComponent<PlayerController>();
    }

    public void SetSkin(SkinData s) {
      skin = s;
      sr.sprite = skin.idle;
      frameIndex = 0;
      frameTimer = 0;
    }

    void Update() {
      if (skin == null) return;

      Vector2 mv = pc.movement;
      if (mv.magnitude < 0.1f) {
        sr.sprite = skin.idle;
        return;
      }

      Sprite[] run = mv.x >= 0 ? skin.runRight : skin.runLeft;
      frameTimer += Time.deltaTime;
      if (frameTimer >= 0.15f) {
        frameTimer = 0f;
        frameIndex = (frameIndex + 1) % run.Length;
      }
      sr.sprite = run[frameIndex];
    }
}