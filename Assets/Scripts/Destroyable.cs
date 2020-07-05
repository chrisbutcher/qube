using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyable : MonoBehaviour {
  public bool DestroyedByPlayerOrByFalling = false;

  public bool MarkedForDestruction = false;
  public bool AnimatingDestruction = false;

  public MarkerType.Type MarkerTypeDestroyedBy;

  public static event SharedEvents.CubeDestructionPause OnCubeDestructionPause;
  public static event SharedEvents.CubeScored OnCubeScored;

  bool BoxColliderDisabled = false;

  void OnEnable() {
    MarkerDetonationSubscribed.OnMarkedCubesDestroy += HandleCubeDestroyed;
    Puzzle.OnMarkedCubesDestroy += HandleCubeDestroyed;
  }

  void OnDisable() {
    MarkerDetonationSubscribed.OnMarkedCubesDestroy -= HandleCubeDestroyed;
    Puzzle.OnMarkedCubesDestroy -= HandleCubeDestroyed;
  }

  void HandleCubeDestroyed() {
    if (this.MarkedForDestruction && !this.GetComponent<Tumble>().isMoving && !AnimatingDestruction) {
      if (OnCubeDestructionPause != null) {
        OnCubeDestructionPause();
      }

      AnimatingDestruction = true;
      StartCoroutine(SquashCubeAndScore());
    }
  }

  IEnumerator SquashCubeAndScore() {
    float time = .5f; // TODO: Constantize
    float elapsedTime = 0;

    // Creating a new transform, initialized with worldspace defaults ...
    Transform newParent = new GameObject().transform;

    // ... and assigning it to the destroyed cube's parent transform.
    //
    // This is so that we can apply scale / position changes to it, that apply to the child cube the same way, regardless of how the child cube is oriented.
    // In this case, squashing it "downward" after it's possibly rotated 0, 90, 180, 270 or 360 degress.

    float initialYPos = newParent.position.y;

    while (elapsedTime < time) {
      newParent.localScale = new Vector3(
        1f,
        Mathf.SmoothStep(1.0f, 0.0f, (elapsedTime / time)),
        1f
      );

      newParent.position = new Vector3(
        newParent.position.x,
        Mathf.SmoothStep(initialYPos, -0.50f, (elapsedTime / time)),
        newParent.position.z
      );

      elapsedTime += Time.deltaTime;

      if (elapsedTime > (time / 2) && !BoxColliderDisabled) {
        BoxColliderDisabled = true;
        GetComponent<BoxCollider>().enabled = false;
      }

      yield return null;
      this.gameObject.transform.SetParent(newParent);
    }

    Destroy(newParent.gameObject);

    DestroyedByPlayerOrByFalling = true;
    OnCubeScored(this.gameObject, MarkerTypeDestroyedBy);

    this.gameObject.SetActive(false);

    Destroy(this.gameObject);

    GameManager.instance.boardManager.CleanUpDestroyedCubes();
  }
}
