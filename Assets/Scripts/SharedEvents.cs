﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedEvents {
  public delegate void CubeDestructionPause();
  public delegate void CubeFell(GameObject fallenCube);
  public delegate void CubeStartedRotating(GameObject rotatingCube);
  public delegate void CubeFinishedRotating(GameObject rotatedCube);
  public delegate void CubeScored(GameObject scoredCube);
  public delegate void DestroyMarkedCubes();
  public delegate void MarkerDetonationEvent(GameObject destroyedMarker);
  public delegate void PlayerSquashed(GameObject player);
}
