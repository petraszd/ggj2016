using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class IntroManager : MonoBehaviour
{
  public delegate void IntroEvent();
  public static event IntroEvent OnStartPlaying;

  public float frameTimeout = 3.0f;
  public Image uiImage;
  public FrameConfig[] frames;

  private float nextFrameTimer = 0.0f;
  private int frameIndex = -1;

  private bool isStarted = false;

  void Start()
  {
  }

  void Update()
  {
    if (Input.GetButtonDown("Cancel")) {
      Application.Quit();
    } else if (!isStarted) {
      WaitingForAnyKey();
    } else {
      ShowingComics();
    }
  }

  void WaitingForAnyKey()
  {
    if (Input.anyKeyDown) {
      isStarted = true;
      frameIndex = -1;
      nextFrameTimer = 0.0f;
      Next();
    }
  }

  void ShowingComics()
  {
    nextFrameTimer += Time.deltaTime;
    if (Input.anyKeyDown || nextFrameTimer > frameTimeout) {
      Next();
    }
  }

  void Next()
  {
    nextFrameTimer = 0.0f;
    frameIndex++;
    if (frameIndex >= frames.Length) {
      EmitStartPlaying();
      SceneManager.LoadScene("Game");
    } else {
      ChangeFrame();
    }
  }

  void ChangeFrame()
  {
    FrameConfig cfg = frames[frameIndex];
    uiImage.sprite = cfg.sprite;
    if (cfg.clip != null) {
      SoundFXManager.Instance.PlayComicClip(cfg.clip);
    }
  }

  void EmitStartPlaying()
  {
    if (OnStartPlaying != null) {
      OnStartPlaying();
    }
  }
}
