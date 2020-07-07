using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BoardManager : MonoBehaviour {
  public GameObject CubePrefab;
  public GameObject FloorCubePrefab;

  Stack<Puzzle> puzzles = new Stack<Puzzle>();
  Puzzle CurrentPuzzle;

  public FloorManager floorManager;

  private PuzzleLoader puzzleLoader;
  PuzzleLoader.InternalPuzzle currentInternalPuzzle;

  int currentWaveWidth;
  int currentWaveDepth;

  void Awake() {
    floorManager = gameObject.AddComponent<FloorManager>();
    floorManager.FloorCubePrefab = FloorCubePrefab;

    puzzleLoader = new PuzzleLoader();
    puzzleLoader.LoadAndParsePuzzles();
  }

  public void LoadStage(int width, int depth) {
    floorManager.Reset();

    for (int i = 0; i < depth; i++) {
      floorManager.Add(width, false);
    }
  }

  public void LoadWave(int numPuzzles, int width, int depth) {
    currentWaveWidth = width;
    currentWaveDepth = depth;

    for (int i = 0; i < numPuzzles; i++) {
      var puzzle = gameObject.AddComponent<Puzzle>();
      var positionOffset = -(i * depth);

      puzzle.Build(CubePrefab, width, depth, positionOffset);
      puzzles.Push(puzzle);
    }
  }

  public void RemoveAllPuzzles() {
    if (CurrentPuzzle != null) {
      CurrentPuzzle.CleanUpDestroyedCubes();
      CurrentPuzzle.DestroyAll();
      CurrentPuzzle = null;
    }

    foreach (var p in puzzles) {
      p.DestroyAll();
    }

    puzzles.Clear();
  }

  public void RemoveFloor() {
    floorManager.RemoveAllFloorStacks();
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

  public void ActivateNextPuzzle(bool replayingPreviousPuzzle) {
    if (puzzles.Count > 0) {
      CurrentPuzzle = puzzles.Pop();

      if (CurrentPuzzle != null) {
        if (replayingPreviousPuzzle) {
          CurrentPuzzle.Activate(currentInternalPuzzle);
        } else {
          currentInternalPuzzle = puzzleLoader.LoadPuzzle(currentWaveWidth, currentWaveDepth);
          CurrentPuzzle.Activate(currentInternalPuzzle);
        }
      }
    }
  }

  public void CleanUpDestroyedCubes() {
    if (CurrentPuzzle != null) {
      CurrentPuzzle.CleanUpDestroyedCubes();
    }
  }
}
