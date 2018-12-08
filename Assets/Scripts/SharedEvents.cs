using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedEvents {
  public delegate void MarkerDetonationEvent(GameObject destroyedMarker);
  public delegate void CubeScored(GameObject scoredCube);
  public delegate void CubeDestructionPause();
  public delegate void DestroyMarkedCubes();
  public delegate void CubeFell(GameObject fallenCube);
}
