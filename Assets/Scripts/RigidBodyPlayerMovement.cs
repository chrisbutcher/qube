using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigidBodyPlayerMovement : MonoBehaviour {
  Vector3 PLAYER_OFFSET = new Vector3(.5f, 0, .5f);

  public float Speed = 1000f;
  public Vector3 QuantizedPlayerPosition;

  // NOTE: In order to get smooth player movement while moving it via rb.MovePosition below, I had to set Fixed Timestep (in project settings)
  // to 0.005 instead of the default 0.02. This increases CPU usage FYI.
  Rigidbody rb;
  Quaternion lookDirection;

  Vector3 lastFramePosition;

  Animator animator;
  void Awake() {
    rb = GetComponent<Rigidbody>();
    animator = GetComponentInChildren<Animator>();
  }

  // https://www.reddit.com/r/Unity3D/comments/1ee65w/having_wonky_collisions_rigidbodies_being_weird/
  void FixedUpdate() {
    setQuantizedPlayerPosition();

    Vector3 directionVector = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
    Vector3 deltaMoveDirection = directionVector * Speed * Time.deltaTime;

    var playerPositionWithOffset = getPlayerPositionWithOffset();

    var moveDirectionX = new Vector3(deltaMoveDirection.x, deltaMoveDirection.y, 0f);
    var newPositionX = playerPositionWithOffset + (moveDirectionX);

    var moveDirectionZ = new Vector3(0f, deltaMoveDirection.y, deltaMoveDirection.z);
    var newPositionZ = playerPositionWithOffset + (moveDirectionZ);

    if (!GameManager.instance.boardManager.floorManager.IsFloorBelowVec3(newPositionX)) {
      deltaMoveDirection.x = 0f;
    }

    if (!GameManager.instance.boardManager.floorManager.IsFloorBelowVec3(newPositionZ)) {
      deltaMoveDirection.z = 0f;
    }

    rb.MovePosition(rb.position + deltaMoveDirection);

    var currentPosition = rb.position;
    var velocity = (currentPosition - lastFramePosition) / Time.deltaTime;
    animator.SetFloat("Speed", velocity.magnitude);

    lastFramePosition = currentPosition;

    if (directionVector != Vector3.zero) {
      lookDirection = Quaternion.LookRotation(directionVector);
      rb.MoveRotation(Quaternion.LookRotation(directionVector));
    }
  }

  private Vector3 getPlayerPositionWithOffset() {
    return rb.position - PLAYER_OFFSET;
  }

  private void setQuantizedPlayerPosition() {
    QuantizedPlayerPosition = Util.FloorVec3(getPlayerPositionWithOffset());
  }
}
