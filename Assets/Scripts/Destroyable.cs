using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour {

  public bool MarkedForDestruction = false;

  public static event SharedEvents.CubeDestructionPause OnCubeDestructionPause;
  public static event SharedEvents.CubeScored OnCubeScored;

  void OnEnable() {
    MarkerDetonationSubscribed.OnMarkedCubesDestroy += HandleCubeDestroyed;
    Puzzle.OnMarkedCubesDestroy += HandleCubeDestroyed;
  }

  void OnDisable() {
    MarkerDetonationSubscribed.OnMarkedCubesDestroy -= HandleCubeDestroyed;
    Puzzle.OnMarkedCubesDestroy -= HandleCubeDestroyed;
  }

  void HandleCubeDestroyed() {
    if (this.MarkedForDestruction && !this.GetComponent<Tumble>().isMoving) {
      if (OnCubeDestructionPause != null) {
        OnCubeDestructionPause();
      }
      StartCoroutine(Fade());
    }
  }

  IEnumerator Fade() {
    float time = 1f;
    float elapsedTime = 0;
    float initialYPos = this.gameObject.transform.position.y;

    while (elapsedTime < time) {
      this.gameObject.transform.localScale = new Vector3(
        1f,
        Mathf.SmoothStep(1.0f, 0.0f, (elapsedTime / time)),
        1f
      );

      this.gameObject.transform.position = new Vector3(
        this.gameObject.transform.position.x,
        Mathf.SmoothStep(initialYPos, -0.5f, (elapsedTime / time)),
        this.gameObject.transform.position.z
      );

      elapsedTime += Time.deltaTime;
      yield return null;
    }

    OnCubeScored(this.gameObject);

    this.gameObject.SetActive(false);
    Destroy(this.gameObject);

    GameManager.instance.boardManager.CleanUpDestroyedCubes();
  }

  void Start() {
  }

  void Update() {
  }
}
