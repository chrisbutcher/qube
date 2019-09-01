using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerType : MonoBehaviour {

  public enum Type {
    PlayerMarker,
    AdvantageMarker,
  }

  public Type CurrentType;

  void Start() {
  }
}
