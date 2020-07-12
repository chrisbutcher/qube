using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// NOTE: This script is used to rotate player marker arrows.
public class ConstantRotation : MonoBehaviour {
  public float RotationSpeed = 90f;

  void Update() {
    if (!GameManager.GameManagerInstance().isGameActive()) {
      return;
    }

    transform.RotateAround(transform.position, transform.up, Time.deltaTime * RotationSpeed);
  }
}
