using UnityEngine;
using System.Collections;

public class RandomSprite : MonoBehaviour
{
  public Sprite[] sprites;
  public bool randomFlipX = true;
  public bool randomFlipY = true;

  void Start()
  {
    SpriteRenderer render = GetComponent<SpriteRenderer>();
    render.sprite = sprites[Random.Range(0, sprites.Length)];
    if (randomFlipX && Random.value < 0.5f) {
      render.flipX = true;
    }
    if (randomFlipY && Random.value < 0.5f) {
      render.flipY = true;
    }
  }
}
