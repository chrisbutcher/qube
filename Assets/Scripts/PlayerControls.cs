using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {
  bool Disabled = false;

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

    var horizontalMotion = Input.GetAxis("HorizontalKeyboard") + Input.GetAxis("HorizontalDpad") + Input.GetAxis("HorizontalStick");
    var verticalMotion = Input.GetAxis("VerticalKeyboard") + Input.GetAxis("VerticalDpad") + Input.GetAxis("VerticalStick");

    return new Vector3(horizontalMotion, 0f, verticalMotion);
  }

  public void Enable() {
    Disabled = false;
  }

  public void Disable() {
    Disabled = true;
  }
}
