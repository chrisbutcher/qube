using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedEvents {
  public delegate void CubeDestructionPause();
  public delegate void CubeFell(GameObject fallenCube);
  public delegate void CubeStartedRotating(GameObject rotatingCube);
  public delegate void CubeFinishedRotating(GameObject rotatedCube);
  public delegate void CubeStackDropped();
  public delegate void CubeScored(GameObject scoredCube, MarkerType.Type scoredByMarkerType);
  public delegate void DestroyMarkedCubes();
  public delegate void MarkerDetonationEvent(Vector3 detonatedMarkerPosition, MarkerType.Type detonatedMarkerType);
  public delegate void PlayerSquashed(GameObject player);
}
