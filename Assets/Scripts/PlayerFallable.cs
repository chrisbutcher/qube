using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallable : MonoBehaviour {
  const string FloorCubeTag = "Floor Cube";
  const float PlayerFloodRayCastDistance = 2f;
  const float PlayerFallDistanceBeforeGameOver = -32f;

  public bool PlayerFalling = false;

  Rigidbody playerRB;
  Animator playerAnimator;
  RigidbodyConstraints livingPlayerRBConstraints;

  void OnEnable() {
    FloorManager.OnCubeStackDropped += HandleCubeStackDropped;
  }

  void OnDisable() {
    FloorManager.OnCubeStackDropped -= HandleCubeStackDropped;
  }

  void Start() {
    playerRB = GetComponent<Rigidbody>();
    playerAnimator = GetComponentInChildren<Animator>();
  }

  void HandleCubeStackDropped() {
    RaycastHit hit;
    LayerMask layerMask = Physics.AllLayers;

    var playerPosition = playerRB.position;

    if (Physics.Raycast(playerPosition, Vector3.down, out hit, PlayerFloodRayCastDistance, layerMask, QueryTriggerInteraction.Collide)) {
      var cubeRB = hit.collider.GetComponent<Rigidbody>();

      if (hit.collider.tag == FloorCubeTag && cubeRB.useGravity == true) {
        PlayerFalling = true;

        GameManager.GameManagerInstance().GetSoundManager().PlayFallingOffEdge();

        livingPlayerRBConstraints = playerRB.constraints;

        GameManager.GameManagerInstance().DisablePlayerControlsAndWalkAnimation();
        playerAnimator.SetTrigger("Falling");

        // playerRB.detectCollisions = true;
        playerRB.useGravity = true;
        playerRB.constraints = RigidbodyConstraints.None;
        playerRB.AddRelativeTorque(8f, -2f, 3f);
        playerRB.drag = 1f;
        playerRB.angularDrag = 1f;
      }
    }
  }

  void LateUpdate() {
    if (playerRB.position.y <= PlayerFallDistanceBeforeGameOver && PlayerFalling == true) {
      PlayerFalling = false;

      playerAnimator.SetFloat("Speed", 0f);
      playerAnimator.ResetTrigger("Falling");
      playerAnimator.SetTrigger("NoLongerFalling");

      // playerRB.detectCollisions = true;
      playerRB.useGravity = false;
      playerRB.constraints = livingPlayerRBConstraints;

      playerRB.drag = 0f;
      playerRB.angularDrag = 0f;
      playerRB.angularVelocity = Vector3.zero;
      playerRB.velocity = Vector3.zero;

      playerRB.MoveRotation(Quaternion.LookRotation(Vector3.forward));

      GameManager.GameManagerInstance().RestartStageAfterGameOver(0);
    }
  }
}
