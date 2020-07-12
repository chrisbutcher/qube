using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour {
  AudioSource oldAudioSource;

  //

  public AudioClip FallingOffEdge;
  public AudioClip FloorFallingApart;
  public AudioClip GreatScore;
  public AudioClip PlayerMarkerExplosion;
  public AudioClip QubeExlosion1;
  public AudioClip QubeExlosion2;
  public AudioClip QubeExlosion3;
  public AudioClip SetPlayerMarker;
  public AudioClip Squashed;

  DebouncedAudioClip FallingOffEdgeClip;
  DebouncedAudioClip FloorFallingApartClip;
  DebouncedAudioClip GreatScoreClip;
  DebouncedAudioClip PlayerMarkerExplosionClip;
  DebouncedAudioClip QubeExlosion1Clip;
  DebouncedAudioClip QubeExlosion2Clip;
  DebouncedAudioClip QubeExlosion3Clip;
  DebouncedAudioClip SetPlayerMarkerClip;
  DebouncedAudioClip SquashedClip;

  AudioSource audioSource; // New!

  public void PlayFallingOffEdge() {
    FallingOffEdgeClip.PlayWithDebounce(0.1f);
  }

  public void PlayFloorFallingApart() {
    FloorFallingApartClip.PlayWithDebounce(0.1f);
  }

  public void PlayGreatScore() {
    GreatScoreClip.PlayWithDebounce(0.1f);
  }

  public void PlayPlayerMarkerExplosion() {
    PlayerMarkerExplosionClip.PlayWithDebounce(0.1f);
  }

  public void PlayQubeExlosion1() {
    QubeExlosion1Clip.PlayWithDebounce(0.1f);
  }

  public void PlayQubeExlosion2() {
    QubeExlosion2Clip.PlayWithDebounce(0.1f);
  }

  public void PlayQubeExlosion3() {
    QubeExlosion3Clip.PlayWithDebounce(0.1f);
  }

  public void PlaySetPlayerMarker() {
    SetPlayerMarkerClip.PlayWithDebounce(0.1f);
  }

  public void PlaySquashed() {
    SquashedClip.PlayWithDebounce(0.1f);
  }

  class DebouncedAudioClip {
    public AudioSource audioSource;
    AudioClip audioClip;

    float debounceDelay = 0f;

    public DebouncedAudioClip(AudioSource audioSource, AudioClip audioClip) {
      this.audioSource = audioSource;
      this.audioClip = audioClip;
    }

    public void Update() {
      if (this.debounceDelay > 0f) {
        this.debounceDelay -= Time.deltaTime;
      }
    }

    public void PlayWithDebounce(float playNoSoonerThan) {
      if (debounceDelay <= 0f) {
        audioSource.PlayOneShot(audioClip);
        this.debounceDelay = playNoSoonerThan;
      }
    }
  }

  class DebouncedAudioSource {
    public AudioSource audioSource;

    float debounceDelay = 0f;

    public DebouncedAudioSource(AudioSource audioSource) {
      this.audioSource = audioSource;
    }

    public void Update() {
      if (this.debounceDelay > 0f) {
        this.debounceDelay -= Time.deltaTime;
      }
    }

    public void PlayWithDebounce(float playNoSoonerThan) {
      if (debounceDelay <= 0f) {
        this.audioSource.Play();
        this.debounceDelay = playNoSoonerThan;
      }
    }
  }

  DebouncedAudioSource cubeTumble;

  void OnEnable() {
    Tumble.OnCubeFinishedRotating += DebouncedPlayCubeTumbleSound;
  }

  void OnDisable() {
    Tumble.OnCubeFinishedRotating -= DebouncedPlayCubeTumbleSound;
  }

  void Start() {
    oldAudioSource = GameObject.FindGameObjectWithTag("CubeGroup").GetComponent<AudioSource>();
    cubeTumble = new DebouncedAudioSource(oldAudioSource);

    //

    audioSource = GetComponent<AudioSource>();

    FallingOffEdgeClip = new DebouncedAudioClip(audioSource, FallingOffEdge);
    FloorFallingApartClip = new DebouncedAudioClip(audioSource, FloorFallingApart);
    GreatScoreClip = new DebouncedAudioClip(audioSource, GreatScore);
    PlayerMarkerExplosionClip = new DebouncedAudioClip(audioSource, PlayerMarkerExplosion);
    QubeExlosion1Clip = new DebouncedAudioClip(audioSource, QubeExlosion1);
    QubeExlosion2Clip = new DebouncedAudioClip(audioSource, QubeExlosion2);
    QubeExlosion3Clip = new DebouncedAudioClip(audioSource, QubeExlosion3);
    SetPlayerMarkerClip = new DebouncedAudioClip(audioSource, SetPlayerMarker);
    SquashedClip = new DebouncedAudioClip(audioSource, Squashed);
  }

  void Update() {
    if (!GameManager.GameManagerInstance().isGameActive()) {
      return;
    }

    cubeTumble.Update();

    // New!

    FallingOffEdgeClip.Update();
    FloorFallingApartClip.Update();
    GreatScoreClip.Update();
    PlayerMarkerExplosionClip.Update();
    QubeExlosion1Clip.Update();
    QubeExlosion2Clip.Update();
    QubeExlosion3Clip.Update();
    SetPlayerMarkerClip.Update();
    SquashedClip.Update();
  }

  void DebouncedPlayCubeTumbleSound(GameObject _rotatedCube) {
    cubeTumble.PlayWithDebounce(0.2f);
  }
}
