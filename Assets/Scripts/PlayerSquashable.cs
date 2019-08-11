using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSquashable : MonoBehaviour {
  void OnEnable() {
    Tumble.OnPlayerSquashed += HandlePlayerSquashed;
  }

  void OnDisable() {
    Tumble.OnPlayerSquashed -= HandlePlayerSquashed;
  }

  void HandlePlayerSquashed(GameObject player) {
    player.GetComponent<RigidBodyPlayerMovement>().enabled = false;
    player.GetComponent<CapsuleCollider>().enabled = false;

    StartCoroutine(AnimateSquash(player));
  }

  IEnumerator AnimateSquash(GameObject player) {
    float time = .4f;
    float elapsedTime = 0;

    var initialScale = player.transform.localScale;
    var initialPosition = player.transform.position;

    while (elapsedTime < time) {
      player.transform.localScale = new Vector3(
        initialScale.x,
        Mathf.SmoothStep(initialScale.y, 0.0f, (elapsedTime / time)),
        initialScale.z
      );

      player.transform.position = new Vector3(
        initialPosition.x,
        Mathf.SmoothStep(initialPosition.y, GameConsts.PlayerSquashDownwardDistance, (elapsedTime / time)),
        initialPosition.z
      );

      elapsedTime += Time.deltaTime;
      yield return null;
    }
  }
}
