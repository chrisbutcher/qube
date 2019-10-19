using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControls : MonoBehaviour {
  public bool Disabled = false;

  void Start() {

  }

  void Update() {

  }

  public bool isDetonatingAdvantageCubes() {
    return Input.GetKeyDown(KeyCode.Z);
  }

  public void Enable() {
    Disabled = false;
  }

  public void Disable() {
    Disabled = true;
  }
}
