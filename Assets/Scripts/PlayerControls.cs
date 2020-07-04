using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {
  bool Disabled = false;

  void Start() {
  }

  void Update() {
  }

  public bool isDetonatingPlayerMarker() {

    // Debug.Log("Hi");

    if (Disabled) {
      return false;
    }

    // Debug.Log("Ho");

    // return Input.GetKeyDown(KeyCode.X);
    return Input.GetButtonDown("SetClearPlayerMarker");
  }

  public bool isDetonatingAdvantageMarkers() {
    if (Disabled) {
      return false;
    }


    // return Input.GetKeyDown(KeyCode.Z);
    return Input.GetButtonDown("DetonateAdvantageZone");
  }

  public bool isSpeedingUpCubes() {
    // "SpeedUpCubes"

    // return Input.GetKey(KeyCode.LeftShift);
    return Input.GetButton("SpeedUpCubes");
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
