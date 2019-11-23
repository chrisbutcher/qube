using UnityEngine;

public class SoundManager : MonoBehaviour {
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
    cubeTumble = new DebouncedAudioSource(GameObject.FindGameObjectWithTag("CubeGroup").GetComponent<AudioSource>());
  }

  void Update() {
    cubeTumble.Update();
  }

  void DebouncedPlayCubeTumbleSound(GameObject _rotatedCube) {
    cubeTumble.PlayWithDebounce(0.5f);
  }
}
