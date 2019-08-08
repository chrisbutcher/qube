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

  private PuzzleLoader puzzleLoader;

  void Awake() {
    floorManager = gameObject.AddComponent<FloorManager>();

    for (int i = 0; i < FLOOR_LENGTH; i++) {
      floorManager.Add(FloorCubePrefab, FLOOR_WIDTH);
    }

    puzzleLoader = new PuzzleLoader();
  }

  void Start() {
    PushNewPuzzle();
  }

  void Update() {
    if (Input.GetKeyDown(KeyCode.O)) {
      RemoveAllPuzzles();
      PushNewPuzzle();
    }
  }

  void PushNewPuzzle() {
    var internalPuzzle = puzzleLoader.LoadPuzzle(4, 2);
    var puzzle = gameObject.AddComponent<Puzzle>();

    puzzle.Build(CubePrefab, internalPuzzle);
    puzzles.Enqueue(puzzle);
  }

  void RemoveAllPuzzles() {
    foreach (var p in puzzles) {
      p.DestroyAll();
    }

    puzzles.Clear();
  }

  public void ActivateNextPuzzle() {
    if (puzzles.Count > 0) {
      CurrentPuzzle = puzzles.Dequeue();

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
}
