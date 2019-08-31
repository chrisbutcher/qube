using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BoardManager : MonoBehaviour {
  public GameObject CubePrefab;
  public GameObject FloorCubePrefab;

  // Queue<Puzzle> puzzles = new Queue<Puzzle>();
  Stack<Puzzle> puzzles = new Stack<Puzzle>();
  Puzzle CurrentPuzzle;

  public FloorManager floorManager;

  private PuzzleLoader puzzleLoader;

  void Awake() {
    floorManager = gameObject.AddComponent<FloorManager>();
    floorManager.FloorCubePrefab = FloorCubePrefab;

    puzzleLoader = new PuzzleLoader();
  }

  public void LoadStage(int width, int depth) {
    // TODO: Have floor manager reset itself first
    floorManager.Reset();

    for (int i = 0; i < depth; i++) {
      floorManager.Add(width);
    }
  }

  public void LoadWave(int numPuzzles, int width, int depth) {
    for (int i = 0; i < numPuzzles; i++) {
      var internalPuzzle = puzzleLoader.LoadPuzzle(width, depth);
      var puzzle = gameObject.AddComponent<Puzzle>();
      var positionOffset = new Vector3(0f, 0f, -(i * depth));

      puzzle.Build(CubePrefab, internalPuzzle, positionOffset);
      puzzles.Push(puzzle);
    }
  }

  void RemoveAllPuzzles() {
    foreach (var p in puzzles) {
      p.DestroyAll();
    }

    puzzles.Clear();
  }

  public int CurrentWavePuzzleCount() {
    return puzzles.Count;
  }

  public Puzzle CurrentPuzzleOrNextPuzzleUp() {
    if (CurrentPuzzle != null) {
      return CurrentPuzzle;
    }

    return puzzles.Peek();
  }

  public bool HasActivePuzzle() {
    if (CurrentPuzzle != null) {
      if (CurrentPuzzle.ActiveGameObjectCubeCount() == 0) {
        return false;
      }

      return true;
    } else {
      return false;
    }
  }

  public bool CubeIsInCurrentPuzzle(GameObject cube) {
    if (CurrentPuzzle != null) {
      return CurrentPuzzle.CubeIsInCurrentPuzzle(cube);
    } else {
      return false;
    }
  }

  public void ActivateNextPuzzle() {
    if (puzzles.Count > 0) {
      CurrentPuzzle = puzzles.Pop();

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
