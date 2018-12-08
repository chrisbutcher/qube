using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedEvents {
  public delegate void MarkerDetonationEvent(GameObject destroyedMarker);
  public delegate void CubeScored(GameObject destroyedCube);
  public delegate void DestroyScoredCube();
  public delegate void CubeFell(GameObject fallenCube);
}
