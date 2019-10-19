using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyedFromFalling : MonoBehaviour {
  void Update() {
    if (transform.position.y <= -50f) {
      Destroy(this.gameObject);
    }
  }
}
