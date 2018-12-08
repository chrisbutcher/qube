using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour {

  public bool MarkedForDestruction = false;

  public static event SharedEvents.CubeScored OnCubeScored;

  void OnEnable() {
    MarkerDetonationSubscribed.OnCubeDestroy += HandleCubeDestroyed;
    Puzzle.OnCubeDestroy += HandleCubeDestroyed;
  }

  void OnDisable() {
    MarkerDetonationSubscribed.OnCubeDestroy -= HandleCubeDestroyed;
    Puzzle.OnCubeDestroy -= HandleCubeDestroyed;
  }

  void HandleCubeDestroyed() {
    if (this.MarkedForDestruction && !this.GetComponent<Tumble>().isMoving) {
      OnCubeScored(this.gameObject);

      this.gameObject.SetActive(false);
      Destroy(this.gameObject);

      GameManager.instance.boardManager.CleanUpDestroyedCubes();
    }
  }

  void Start() {
  }

  void Update() {
  }
}
