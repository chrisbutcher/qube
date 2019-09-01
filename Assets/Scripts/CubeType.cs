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

  public Type CurrentType;

  void Start() {
    var materials = GetComponent<Renderer>().materials;

    switch (CurrentType) {
      case Type.Advantage:
        materials[0].color = Color.green;
        break;
      case Type.Forbidden:
        materials[0].color = Color.black;
        break;
      case Type.Normal:
        break;
    }
  }
}
