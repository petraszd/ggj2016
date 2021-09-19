using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MobileManager : MonoBehaviour
{
  public Canvas buttonCanvas;

  void Start ()
  {
    if (Input.touchSupported) {
      buttonCanvas.enabled = true;
    } else {
      Destroy(gameObject);
    }
  }
}
