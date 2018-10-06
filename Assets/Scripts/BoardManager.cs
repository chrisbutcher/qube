using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BoardManager : MonoBehaviour {
  public GameObject CubePrefab;
  public GameObject FloorCubePrefab;

  Queue<Puzzle> puzzles = new Queue<Puzzle>();
  Puzzle CurrentPuzzle;
  public FloorManager floorManager;

  const int FLOOR_WIDTH = 4;
  const int FLOOR_LENGTH = 10;

  // TODO track which player destroyed which cube
  public List<GameObject> CubesAwaitingDestruction = new List<GameObject>();

  public static event SharedEvents.CubeDestroyed OnCubeDetonation;

  void Awake() {
    floorManager = gameObject.AddComponent<FloorManager>();

    for (int i = 0; i < FLOOR_LENGTH; i++) {
      floorManager.Add(FloorCubePrefab, FLOOR_WIDTH);
    }
  }

  void Start() {
    var puzzle = gameObject.AddComponent<Puzzle>();

    puzzle.Build(
      CubePrefab, new List<Util.CubePositionAndType> {
        new Util.CubePositionAndType(new Vector3(0, 0, 0), CubeType.Type.Forbidden),
        new Util.CubePositionAndType(new Vector3(1, 0, 0), CubeType.Type.Normal),
        new Util.CubePositionAndType(new Vector3(2, 0, 0), CubeType.Type.Advantage),
        new Util.CubePositionAndType(new Vector3(3, 0, 0), CubeType.Type.Normal),
      }
    );

    puzzles.Enqueue(puzzle);
  }

  void Update() {
  }

  public void ActivateNextPuzzle() {
    if (puzzles.Count > 0) {
      var CurrentPuzzle = puzzles.Dequeue();
      if (CurrentPuzzle != null) {
        CurrentPuzzle.Activate();
      }
    }
  }

  public void CleanUpDestroyedCubes() {
    if (CurrentPuzzle != null) {
      CurrentPuzzle.CleanUpDestroyedCubes();
    }
  }

  public void DestroyAnyStationaryDestructionAwaitingCubes() {
    for (int i = 0; i < CubesAwaitingDestruction.Count; i++) {
      var cubeToDestroy = CubesAwaitingDestruction[i];

      if (!cubeToDestroy.GetComponent<Tumble>().isMoving) {
        OnCubeDetonation(cubeToDestroy);

        cubeToDestroy.SetActive(false);
        Destroy(cubeToDestroy);
        CubesAwaitingDestruction[i] = null;
      }
    }

    CubesAwaitingDestruction.RemoveAll(cube => cube == null);
    CleanUpDestroyedCubes();
  }
}
