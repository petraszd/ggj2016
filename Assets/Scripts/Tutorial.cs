using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tutorial : MonoBehaviour {
  private static Tutorial instance = null;

  public Image image;
  private Material imageMat;

  void Awake()
  {
    if (instance == null) {
      instance = this;
      DontDestroyOnLoad(gameObject);
    } else if (instance != this) {
      Destroy(gameObject);
    }
  }

	void Start ()
  {
    imageMat = image.material;
    SetAlpha(1.0f);
    StartCoroutine(ImageFadeOut());
	}

  IEnumerator ImageFadeOut()
  {
    yield return new WaitForSeconds(1.5f);
    for (;;) {
      yield return null;
      SetAlpha(imageMat.color.a - Time.deltaTime * 2.0f);
      if (imageMat.color.a <= Mathf.Epsilon) {
        break;
      }
    }
    SetAlpha(1.0f);
    image.enabled = false;
  }

  void SetAlpha(float newAlpha)
  {
    Color newColor = imageMat.color;
    newColor.a = newAlpha;
    imageMat.color = newColor;
  }
}
