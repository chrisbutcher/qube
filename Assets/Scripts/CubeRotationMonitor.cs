﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeRotationMonitor : MonoBehaviour {
  public bool CubeScoredThisPuzzle = false;
  bool trackedEndOfRotation = false;

  void OnEnable() {
    Destroyable.OnCubeScored += HandleCubeScored;
    Tumble.OnCubeStartedRotating += HandleCubeStartedRotation;
    Tumble.OnCubeFinishedRotating += HandleCubeRotated;
  }

  void OnDisable() {
    Destroyable.OnCubeScored += HandleCubeScored;
    Tumble.OnCubeStartedRotating -= HandleCubeStartedRotation;
    Tumble.OnCubeFinishedRotating -= HandleCubeRotated;
  }

  void HandleCubeScored(GameObject scoredCube, MarkerType.Type scoredByMarkerType) {
    if (!CubeScoredThisPuzzle) {
      CubeScoredThisPuzzle = true;

      var puzzle = GameManager.GameManagerInstance().boardManager.CurrentPuzzleOrNextPuzzleUp();
      puzzle.RotationsSinceFirstCubeDestroyed += 1;
    }
  }

  void HandleCubeStartedRotation(GameObject _rotatingCube) {
    if (trackedEndOfRotation)
      trackedEndOfRotation = false;
  }

  void HandleCubeRotated(GameObject _rotatedCube) {
    if (trackedEndOfRotation == false && CubeScoredThisPuzzle == true) {
      trackedEndOfRotation = true;

      var puzzle = GameManager.GameManagerInstance().boardManager.CurrentPuzzleOrNextPuzzleUp();
      if (puzzle.PuzzleContainsNonForbiddenCubes()) {
        puzzle.RotationsSinceFirstCubeDestroyed += 1; // Only count if remaining cubes are NOT all forbidden cubes
      }
    }
  }
}
