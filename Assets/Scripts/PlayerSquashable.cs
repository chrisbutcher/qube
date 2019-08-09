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
    player.GetComponent<PlayerMovement>().enabled = false;
    player.GetComponent<CharacterController>().enabled = false;
    player.GetComponent<CapsuleCollider>().enabled = false;

    // FIXME: Broken at the moment...
    // StartCoroutine(AnimateSquash(player));
  }

  IEnumerator AnimateSquash(GameObject player) {
    float time = .7f;
    float elapsedTime = 0;

    var initialScale = player.transform.localScale;

    while (elapsedTime < time) {
      player.transform.localScale = new Vector3(
        initialScale.x,
        Mathf.SmoothStep(initialScale.y, 0.0f, (elapsedTime / time)),
        initialScale.y
      );

      elapsedTime += Time.deltaTime;
      yield return null;
    }
  }
}
