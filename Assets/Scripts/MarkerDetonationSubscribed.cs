using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MarkerDetonationSubscribed : MonoBehaviour {
  public static event SharedEvents.DestroyMarkedCubes OnMarkedCubesDestroy;

  void OnEnable() {
    PlayerMarker.OnMarkerDetonation += HandleMarkerDetonation;
    AdvantageMarkers.OnAdvantageMarkerDetonation += HandleMarkerDetonation;
  }

  void OnDisable() {
    PlayerMarker.OnMarkerDetonation -= HandleMarkerDetonation;
    AdvantageMarkers.OnAdvantageMarkerDetonation -= HandleMarkerDetonation;
  }

  void HandleMarkerDetonation(Vector3 detonatedMarkerPosition, MarkerType.Type detonatedMarkerType) {
    var positionDetonatedIsMovingTo = GetComponent<Tumble>().PositionMovingTo;

    // If the subscribed cube is in the current puzzle
    if (GameManager.instance.boardManager.CubeIsInCurrentPuzzle(this.gameObject)) {
      // And its destination position (defaults to current position) is the same as the detonated marker...
      if (Util.Vec3sEqualXandZ(positionDetonatedIsMovingTo, detonatedMarkerPosition)) {
        // ... mark it for destruction!
        var destroyable = GetComponent<Destroyable>();
        destroyable.MarkedForDestruction = true;

        destroyable.MarkerTypeDestroyedBy = detonatedMarkerType;

        if (OnMarkedCubesDestroy != null) {
          OnMarkedCubesDestroy();
        }
      }
    }

  }
}
