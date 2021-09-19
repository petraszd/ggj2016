using UnityEngine;
using System.Collections;

public class Trap : Killer
{
  private Animator animator;

  void Start()
  {
    animator = GetComponent<Animator>();
  }

  protected override void OnTriggered()
  {
    animator.SetTrigger("close");
  }
}
