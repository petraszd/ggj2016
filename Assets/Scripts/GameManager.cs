using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
  public Canvas restartCanvas;
  public float waitTillActive = 3.0f;

  void OnEnable()
  {
    Chicken.OnChichenDeath += OnChichenDeath;
  }

  void OnDisable()
  {
    Chicken.OnChichenDeath -= OnChichenDeath;
  }

  void Update()
  {
    if (Input.GetButtonDown("Cancel")) {
      Application.Quit();
    }
  }

  void OnChichenDeath()
  {
    StartCoroutine(WaitForRestart());
  }

  IEnumerator WaitForRestart()
  {
    yield return new WaitForSeconds(waitTillActive);
    restartCanvas.enabled = true;
    for (;;) {
      yield return null;
      if (Input.GetButtonDown("Cancel")) {
        Application.Quit();
        break;
      } else if (Input.anyKey) {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
      }
    }
  }
}
