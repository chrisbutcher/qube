using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedFromFalling : MonoBehaviour {
  const float CubeDestroyedAtDistance = -30f;

  void Update() {
    if (transform.position.y <= CubeDestroyedAtDistance) {
      Destroy(this.gameObject);
    }
  }
}
