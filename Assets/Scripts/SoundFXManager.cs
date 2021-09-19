using UnityEngine;
using System.Collections;

public class SoundFXManager : MonoBehaviour
{
  private static SoundFXManager instance = null;
  public static SoundFXManager Instance
  {
    get {
      return instance;
    }
  }

  public AudioSource musicSource;
  public AudioClip gameMusic;
  public AudioSource fxSource;
  public AudioSource randomNoiseSource;

  public AudioClip[] dieClips;
  public AudioClip[] randomClips;

  public float randomWaitFrom = 4.0f;
  public float randomWaitTo = 10.0f;
  public bool isRandomNoises = false;

  void OnEnable()
  {
    IntroManager.OnStartPlaying += OnStartPlaying;
    Chicken.OnChichenBorn += OnChichenBorn;
    Chicken.OnChichenDeath += OnChichenDeath;
  }

  void OnDisable()
  {
    IntroManager.OnStartPlaying -= OnStartPlaying;
    Chicken.OnChichenBorn -= OnChichenBorn;
    Chicken.OnChichenDeath -= OnChichenDeath;
  }

  void Start()
  {
    StartCoroutine(LoopRandomNoises());
  }

  void Awake()
  {
    if (instance == null) {
      instance = this;
      DontDestroyOnLoad(gameObject);
    } else if (instance != this) {
      Destroy(gameObject);
    }
  }

  void OnStartPlaying()
  {
    musicSource.Stop();
    musicSource.clip = gameMusic;
    musicSource.Play();
  }

  void OnChichenBorn()
  {
    isRandomNoises = true;
  }

  void OnChichenDeath()
  {
    isRandomNoises = false;
  }

  public void PlayDeathFx()
  {
    PlayClip(fxSource, dieClips);
  }

  public void PlayComicClip(AudioClip clip)
  {
    fxSource.PlayOneShot(clip);
  }

  private void PlayClip(AudioSource source, AudioClip clip)
  {
    source.Stop();
    source.clip = clip;
    source.Play();
  }

  private void PlayClip(AudioSource source, AudioClip[] clips)
  {
    PlayClip(source, clips[Random.Range(0, clips.Length)]);
  }

  IEnumerator LoopRandomNoises()
  {
    yield return null;

    for (;;) {
      yield return new WaitForSeconds(Random.Range(randomWaitFrom, randomWaitTo));
      if (isRandomNoises) {
        PlayClip(randomNoiseSource, randomClips);
      }
    }
  }
}
