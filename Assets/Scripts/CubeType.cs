using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeType : MonoBehaviour {

  [System.Serializable]
  public enum Type {
    Normal,
    Advantage,
    Forbidden,
  }

  public bool HasStartedToRoll = false;
  public Type CurrentType;

  Type TypeAsOfLastFrame;
  Material[] materials;

  void Start() {
    materials = GetComponent<Renderer>().materials;

  }

  void Update() {
    if (!GameManager.instance.isGameActive()) {
      return;
    }

    if (!HasStartedToRoll) {
      return;
    }

    if (TypeAsOfLastFrame != CurrentType) {
      TypeAsOfLastFrame = CurrentType;

      switch (CurrentType) {
        case Type.Advantage:
          materials[0].SetColor("_albedo", Color.green);
          // materials[0].color = Color.green;
          break;
        case Type.Forbidden:
          materials[0].SetColor("_albedo", Color.black);
          // materials[0].color = Color.black;
          break;
        case Type.Normal:
          break;
      }
    }
  }
}
