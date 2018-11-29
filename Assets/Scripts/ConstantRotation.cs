using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstantRotation : MonoBehaviour {

  public float RotationSpeed = 90f;

  void Update() {
    transform.RotateAround(transform.position, transform.up, Time.deltaTime * RotationSpeed);
  }
}
