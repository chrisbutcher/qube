using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {
  public bool Disabled = false;

  void Start() {

  }

  void Update() {

  }

  public bool isDetonatingPlayerMarker() {
    if (Disabled) {
      return false;
    }

    return Input.GetKeyDown(KeyCode.X);
  }

  public bool isDetonatingAdvantageMarkers() {
    if (Disabled) {
      return false;
    }

    return Input.GetKeyDown(KeyCode.Z);
  }

  public bool isSpeedingUpCubes() {
    if (Disabled) {
      return false;
    }

    return Input.GetKey(KeyCode.LeftShift);
  }

  public Vector3 getPlayerMovementDirection() {
    if (Disabled) {
      return Vector3.zero;
    }

    return new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
  }

  public void Enable() {
    Disabled = false;
  }

  public void Disable() {
    Disabled = true;
  }
}
