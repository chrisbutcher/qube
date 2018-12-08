using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MarkerDetonationSubscribed : MonoBehaviour {
  public static event SharedEvents.DestroyScoredCube OnCubeDestroy;

  void OnEnable() {
    PlayerMarker.OnMarkerDetonation += HandleMarkerDetonation;
    AdvantageMarkers.OnAdvantageMarkerDetonation += HandleMarkerDetonation;
  }

  void OnDisable() {
    PlayerMarker.OnMarkerDetonation -= HandleMarkerDetonation;
    AdvantageMarkers.OnAdvantageMarkerDetonation -= HandleMarkerDetonation;
  }

  void HandleMarkerDetonation(GameObject detonatedMarker) {
    var positionDetonatedIsMovingTo = GetComponent<Tumble>().PositionMovingTo;

    if (Util.Vec3sEqualXandZ(positionDetonatedIsMovingTo, detonatedMarker.transform.position)) {
      GetComponent<Destroyable>().MarkedForDestruction = true;

      if (OnCubeDestroy != null) {
        OnCubeDestroy();
      }
    }
  }
}
