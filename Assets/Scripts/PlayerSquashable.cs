using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSquashable : MonoBehaviour {
  Vector3 preSquashScale;
  Vector3 preSquashPosition;

  void OnEnable() {
    Tumble.OnPlayerSquashed += HandlePlayerSquashed;
  }

  void OnDisable() {
    Tumble.OnPlayerSquashed -= HandlePlayerSquashed;
  }

  void Start() {
  }

  void HandlePlayerSquashed(GameObject unusedPlayer) {
    this.gameObject.GetComponent<RigidBodyPlayerMovement>().enabled = false;
    this.gameObject.GetComponent<CapsuleCollider>().enabled = false;

    GameManager.GameManagerInstance().DisablePlayerControlsAndWalkAnimation();

    StartCoroutine(AnimateSquash(this.gameObject));
  }

  public void UnSquashPlayer() {
    this.gameObject.transform.localScale = preSquashScale;
    this.gameObject.transform.position = preSquashPosition;

    this.gameObject.GetComponent<RigidBodyPlayerMovement>().enabled = true;
    this.gameObject.GetComponent<CapsuleCollider>().enabled = true;
    GameManager.GameManagerInstance().GetPlayerControls(0).Enable();
  }

  IEnumerator AnimateSquash(GameObject player) {
    float time = .4f;
    float elapsedTime = 0;

    preSquashScale = player.transform.localScale;
    preSquashPosition = player.transform.position;

    GameManager.GameManagerInstance().GetSoundManager().PlaySquashed();

    while (elapsedTime < time) {
      player.transform.localScale = new Vector3(
        preSquashScale.x,
        Mathf.SmoothStep(preSquashScale.y, 0.0f, (elapsedTime / time)),
        preSquashScale.z
      );

      player.transform.position = new Vector3(
        preSquashPosition.x,
        Mathf.SmoothStep(preSquashPosition.y, GameConsts.PlayerSquashDownwardDistance, (elapsedTime / time)),
        preSquashPosition.z
      );

      elapsedTime += Time.deltaTime;
      yield return null;
    }
  }
}
