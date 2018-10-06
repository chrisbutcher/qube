using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fallable : MonoBehaviour {
  void Update() {
    if (transform.position.y <= -50f) {
      Destroy(this.gameObject);
    }
  }
}
