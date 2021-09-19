using UnityEngine;
using System.Collections;

// TODO: class is too big. It is suppose to be splitted into several classes
public class Chicken : MonoBehaviour
{
  // Delegates
  public delegate void ChickenEvent();
  public static event ChickenEvent OnChichenDeath;
  public static event ChickenEvent OnChichenBorn;

  // Public props
  public RayConfig rayCfg;

  public float runningSpeed = 4.0f;
  public float crouchingSpeed = 2.0f;
  public float downSpeed = 6.0f;
  public float upSpeed = 4.0f;

  public float skinWidth = 0.1f;

  public BoxCollider2D[] normalColliders;
  public BoxCollider2D[] packedColliders;

  // Implementation
  private Animator animator;
  private ParticleSystem[] particleSystems;
  private SpriteRenderer spriteRenderer;
  private BoxCollider2D[] colliders;
  private LayerMask blockersMask;

  private bool isFlyingInput = false;
  private bool isCrouching = false;

  private struct HitSearchParam {
    public Vector2 start;
    public Vector2 stepLen;
  }
  private HitSearchParam[] hitSearchParams;
  private Rigidbody2D rb2d;

  void Start()
  {
    animator = GetComponentInChildren<Animator>();
    particleSystems = GetComponentsInChildren<ParticleSystem>();
    spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    rb2d = GetComponent<Rigidbody2D>();

    colliders = packedColliders;
    ChangeColliders(normalColliders); // Disables packed; enables normal

    hitSearchParams = new HitSearchParam[colliders.Length];

    blockersMask = LayerMask.GetMask("Blockers");
  }

  void Update()
  {
    bool isPackInput = IsPackingInputActive();
    isFlyingInput = IsFlyingInputActive() && !isPackInput;

    bool isGrounded = IsGrounded();
    if (isGrounded && isPackInput) {
      isFlyingInput = false;
      PackDown();
    } else {
      PackUp();
    }

    rb2d.MovePosition(rb2d.position + new Vector2(GetHorizontalDelta(), GetVerticalDelta()));
  }

  void OnTriggerEnter2D(Collider2D other)
  {
    if (other.tag == "Killer") {
      EmitChickenDeath();
      SoundFXManager.Instance.PlayDeathFx();
      spriteRenderer.enabled = false;
      animator.enabled = false;
      enabled = false;
      PlayAllParticles();

      if (!Input.touchSupported) {  // TODO: Stupid hack for mobiles
        Killer killer = other.gameObject.GetComponent<Killer>();
        if (killer != null) {
          killer.Trigger();
        }
      }
    }
  }

  void PlayAllParticles()
  {
    foreach (ParticleSystem ps in particleSystems) {
      ps.Play();
    }
  }

  void EmitChickenDeath()
  {
    if (OnChichenDeath != null) {
      OnChichenDeath();
    }
  }

  bool IsFlyingInputActive()
  {
    bool result = Input.GetButton("Fly") || Input.GetButtonDown("Fly");
    if (Input.touchSupported) {
      foreach (Touch touch in Input.touches) {
        if (touch.position.x < Screen.width / 2) {
          result = true;
        }
      }
    }
    return result;
  }

  bool IsPackingInputActive()
  {
    bool result = Input.GetButton("Pack") || Input.GetButtonDown("Pack");
    if (Input.touchSupported) {
      foreach (Touch touch in Input.touches) {
        if (touch.position.x > Screen.width / 2) {
          result = true;
        }
      }
    }
    return result;
  }

  float GetCorrectionWidth()
  {
    // Can't explain why I am using skinWidth or even ta hell skinWidth is
    return skinWidth / 2.0f - Mathf.Epsilon;
  }

  float GetHorizontalDelta()
  {
    RaycastHit2D hit = GetRightHit();
    float result = Time.deltaTime * GetHorizontalMovingSpeed();
    if (hit) {
      // TODO: remove if hiting means death
      // Hack to move chicken away if it is inside a block
      if (hit.distance == 0.0f) {
        result = -GetCorrectionWidth();
      } else {
        result = 0.0f;
      }
    }
    return result;
  }

  float GetHorizontalMovingSpeed()
  {
    if (isCrouching) {
      return crouchingSpeed;
    }
    return runningSpeed;
  }

  float GetVerticalDelta()
  {
    if (TryToFly()) {
      return GetFlyingDelta();
    }
    return GetNoFlyingDelta();
  }

  float GetFlyingDelta()
  {
    RaycastHit2D hit = GetUpHit();
    float result = 0.0f;
    if (hit) {
      if (hit.distance == 0.0f) {
        result = -GetCorrectionWidth();
      }
    } else if (isFlyingInput) {
      result = Time.deltaTime * upSpeed;
    }
    return result;
  }

  float GetNoFlyingDelta()
  {
    RaycastHit2D hit = GetDownHit();
    float delta = Time.deltaTime * downSpeed;
    if (hit && delta > hit.distance) {
      delta = hit.distance - GetCorrectionWidth();
    }
    return Mathf.Min(0.0f, -delta);
  }

  bool IsGrounded()
  {
    RaycastHit2D hit = GetDownHit();
    return hit.distance < skinWidth;
  }

  bool TryToFly()
  {
    if (!isFlyingInput) {
      animator.SetBool("fly", false);
      return false;
    }

    animator.SetBool("fly", true);
    return true;
  }

  void PackDown()
  {
    animator.SetBool("down", true);
    ChangeColliders(packedColliders);
    isCrouching = true;
  }

  void PackUp()
  {
    animator.SetBool("down", false);
    ChangeColliders(normalColliders);
    isCrouching = false;
  }

  void ChangeColliders(BoxCollider2D[] newColliders) {
    if (newColliders == colliders) {
      return;
    }

    foreach (BoxCollider2D coll in colliders) {
      coll.enabled = false;
    }
    colliders = newColliders;
    foreach (BoxCollider2D coll in colliders) {
      coll.enabled = true;
    }
  }

  RaycastHit2D GetRightHit()
  {
    for (int i = 0; i < colliders.Length; ++i) {
      BoxCollider2D coll = colliders[i];
      Vector2 center = coll.bounds.center;
      Vector2 extents = coll.bounds.extents;
      Vector2 tr = center + extents;
      Vector2 br = center + new Vector2(extents.x, -extents.y);
      Vector2 stepLen = ((tr.y - br.y) / (rayCfg.nRightRays - 1)) * Vector2.down;

      hitSearchParams[i].start = tr;
      hitSearchParams[i].stepLen = stepLen;
    }
    return GetHit(Vector2.right, rayCfg.nRightRays, rayCfg.RightRayLength);
  }

  RaycastHit2D GetDownHit()
  {
    for (int i = 0; i < colliders.Length; ++i) {
      BoxCollider2D coll = colliders[i];
      Vector2 center = coll.bounds.center;
      Vector2 extents = coll.bounds.extents;
      Vector2 lb = center + new Vector2(-extents.x, -extents.y);
      Vector2 rb = center + new Vector2(extents.x, -extents.y);
      Vector2 stepLen = ((rb.x - lb.x) / (rayCfg.nDownRays - 1)) * Vector2.right;

      hitSearchParams[i].start = lb;
      hitSearchParams[i].stepLen = stepLen;
    }
    return GetHit(Vector2.down, rayCfg.nDownRays, rayCfg.downRayLength);
  }

  RaycastHit2D GetUpHit()
  {
    for (int i = 0; i < colliders.Length; ++i) {
      BoxCollider2D coll = colliders[i];
      Vector2 center = coll.bounds.center;
      Vector2 extents = coll.bounds.extents;
      Vector2 lt = center + new Vector2(-extents.x, extents.y);
      Vector2 rt = center + new Vector2(extents.x, extents.y);
      Vector2 stepLen = ((rt.x - lt.x) / (rayCfg.nUpRays - 1)) * Vector2.right;

      hitSearchParams[i].start = lt;
      hitSearchParams[i].stepLen = stepLen;
    }
    return GetHit(Vector2.up, rayCfg.nUpRays, rayCfg.upRayLength);
  }

  RaycastHit2D GetHit(Vector2 direction, int nSteps, float rayLength)
  {
    RaycastHit2D result = new RaycastHit2D();
    float minDistance = Mathf.Infinity;

    foreach (HitSearchParam param in hitSearchParams) {
      for (int i = 0; i < nSteps; ++i) {
        Vector2 origin = param.start + param.stepLen * i;
        RaycastHit2D hit = Physics2D.Raycast(origin, direction, rayLength, blockersMask);
        if (hit && hit.distance < minDistance) {
          minDistance = hit.distance;
          result = hit;
        }
      }
    }
    return result;
  }
}
