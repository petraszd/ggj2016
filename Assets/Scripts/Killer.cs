using UnityEngine;
using System.Collections;

abstract public class Killer : MonoBehaviour
{
  public void Trigger()
  {
    OnTriggered();
  }

  abstract protected void OnTriggered();
}
