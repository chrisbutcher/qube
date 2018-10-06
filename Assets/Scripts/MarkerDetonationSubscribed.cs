using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MarkerDetonationSubscribed : MonoBehaviour {
  void OnEnable() {
    PlayerMarker.OnMarkerDetonation += HandleDetonation;
    AdvantageMarkers.OnAdvantageMarkerDetonation += HandleDetonation;
  }

  void OnDisable() {
    PlayerMarker.OnMarkerDetonation -= HandleDetonation;
    AdvantageMarkers.OnAdvantageMarkerDetonation -= HandleDetonation;
  }

  void HandleDetonation(GameObject detonatedMarker) {
    var positionDetonatedIsMovingTo = GetComponent<Tumble>().PositionMovingTo;

    if (Util.Vec3sEqualXandZ(positionDetonatedIsMovingTo, detonatedMarker.transform.position)) {
      GameManager.instance.boardManager.CubesAwaitingDestruction.Add(gameObject);
      GameManager.instance.boardManager.DestroyAnyStationaryDestructionAwaitingCubes();
    }
  }
}
