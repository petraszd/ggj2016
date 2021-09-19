using UnityEngine;

public class Parallax : MonoBehaviour
{
  public float xMultiplier = 0.25f;
  public float yMultiplier = 0.125f;

  Vector3 originalPosition;

  void Start ()
  {
    originalPosition = transform.position;
  }

  void Update ()
  {
    Vector3 camPos = Camera.main.transform.position;

    Vector3 newPosition = originalPosition;
    newPosition.x = newPosition.x - camPos.x * xMultiplier;
    newPosition.y = newPosition.y - camPos.y * yMultiplier;

    transform.position = newPosition;
  }
}
