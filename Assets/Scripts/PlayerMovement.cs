using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
  Vector3 PLAYER_OFFSET = new Vector3(.5f, 0, .5f);

  public Vector3 QuantizedPlayerPosition;

  public float speed = 6.0F;
  public float jumpSpeed = 8.0F;
  public float gravity = 20.0F;

  private Vector3 moveDirection = Vector3.zero;

  void Awake() {
    setQuantizedPlayerPosition();
  }

  void Update() {
    setQuantizedPlayerPosition();

    CharacterController controller = GetComponent<CharacterController>();

    if (controller.isGrounded) {
      moveDirection = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
      moveDirection = transform.TransformDirection(moveDirection);
      moveDirection *= speed;

      if (Input.GetButton("Jump")) {
        moveDirection.y = jumpSpeed;
      }
    }

    moveDirection.y -= gravity * Time.deltaTime;

    var deltaMoveDirection = moveDirection * Time.deltaTime;

    var playerPositionWithOffset = getPlayerPositionWithOffset();

    var moveDirectionX = new Vector3(deltaMoveDirection.x, moveDirection.y, 0f);
    var newPositionX = playerPositionWithOffset + (moveDirectionX);

    var moveDirectionZ = new Vector3(0f, deltaMoveDirection.y, deltaMoveDirection.z);
    var newPositionZ = playerPositionWithOffset + (moveDirectionZ);

    if (!GameManager.instance.boardManager.floorManager.IsFloorBelowVec3(newPositionX)) {
      deltaMoveDirection.x = 0f;
    }

    if (!GameManager.instance.boardManager.floorManager.IsFloorBelowVec3(newPositionZ)) {
      deltaMoveDirection.z = 0f;
    }

    controller.Move(deltaMoveDirection);
  }

  private Vector3 getPlayerPositionWithOffset() {
    return transform.position - PLAYER_OFFSET;
  }

  private void setQuantizedPlayerPosition() {
    QuantizedPlayerPosition = Util.FloorVec3(getPlayerPositionWithOffset());
  }
}
