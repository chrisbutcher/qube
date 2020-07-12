using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour {
  List<GameObject> puzzleCubes = new List<GameObject>();

  public bool active = false;
  float sleepingFor = 0f;

  public int RotationsSinceFirstCubeDestroyed;
  public int TypicalRotationNumber;

  public enum State { Roll, RollPaused, DetonationPaused };
  Dictionary<State, float> StateDelays = new Dictionary<State, float>() {
    {State.Roll, 1f},
    {State.RollPaused, 0.46f}, // TODO Constantize?
    {State.DetonationPaused, 2.16f},
  };
  Queue<State> stateQueue = new Queue<State>();

  public void DestroyAll() {
    foreach (var p in puzzleCubes) {
      Destroy(p);
    }

    puzzleCubes.Clear();
  }

  public static event SharedEvents.DestroyMarkedCubes OnMarkedCubesDestroy;

  void OnEnable() {
    Destroyable.OnCubeDestructionPause += HandleCubeDestructionPause;
  }

  void OnDisable() {
    Destroyable.OnCubeDestructionPause -= HandleCubeDestructionPause;
  }

  void Awake() {
    ResetQueue();
  }

  void Start() {
  }

  void HandleCubeDestructionPause() {
    if (active) {
      if (stateQueue.Count > 0) {

        if (!stateQueue.Contains(State.DetonationPaused)) {
          stateQueue.Enqueue(State.DetonationPaused);
        }
      } else {
        stateQueue.Enqueue(State.DetonationPaused);
      }
    }
  }

  // TODO: Good candidate to switch to a Coroutine?
  void Update() {
    if (!GameManager.GameManagerInstance().isGameActive()) {
      return;
    }

    if (active) {
      if (sleepingFor > 0f) {
        sleepingFor -= Time.deltaTime * GameManager.GameManagerInstance().TumbleSpeedMultiplier();
      } else {
        if (AllCubesStationary()) {
          if (OnMarkedCubesDestroy != null) {
            OnMarkedCubesDestroy();
          }

          sleepingFor = 0f;

          if (stateQueue.Count > 0) {
            var poppedState = stateQueue.Dequeue();

            if (poppedState == State.Roll) {
              float rollDuration;
              sleepingFor = rollDuration = StateDelays[poppedState];

              MoveAllCubes(Vector3.back, rollDuration);
            } else {
              var sleepDelay = StateDelays[poppedState];
              sleepingFor = sleepDelay;
            }
          } else {
            ResetQueue();
          }
        }
      }
    }

    if (puzzleCubes.Count == 0) {
      active = false;
    }
  }

  public void Build(GameObject cubePrefab, int width, int depth, int positionOffset) {
    for (int z = 0; z > -depth; z--) {
      for (int x = 0; x < width; x++) {
        var position = new Vector3(x, 0f, z + positionOffset);

        GameObject cube = (GameObject)Instantiate(cubePrefab, position, Quaternion.identity);
        Util.ParentInstanceUnderEmpty(cube, "CubeGroup");
        puzzleCubes.Add(cube);
      }
    }
  }

  public void MoveAllCubes(Vector3 direction, float duration) {
    foreach (var c in puzzleCubes) {
      if (c != null && !c.GetComponent<Tumble>().isFalling) {
        if (!c.GetComponent<CubeType>().HasStartedToRoll) {
          c.GetComponent<CubeType>().HasStartedToRoll = true;
        }

        c.GetComponent<Tumble>().TumbleInDirection(direction, duration);
      }
    }
  }

  public void Activate(PuzzleLoader.InternalPuzzle puzzleToActivateAs) {
    TypicalRotationNumber = puzzleToActivateAs.typicalRotationsNeeded;

    for (int i = 0; i < puzzleToActivateAs.cubes.Count; i++) {
      var internalCube = puzzleToActivateAs.cubes[i];
      var puzzleCube = puzzleCubes[i];

      puzzleCube.GetComponent<CubeType>().CurrentType = internalCube.type;
    }

    active = true;
  }

  public bool IsActive() {
    return active;
  }

  public int CubeCount() {
    return puzzleCubes.Count;
  }

  public bool CubeIsInCurrentPuzzle(GameObject cube) {
    foreach (var c in puzzleCubes) {
      if (c == cube) {
        return true;
      }
    }

    return false;
  }

  public int ActiveGameObjectCubeCount() {
    int activeCount = 0;
    foreach (var c in puzzleCubes) {

      if (c != null) {
        if (c.GetComponent<Destroyable>().DestroyedByPlayerOrByFalling == false) {
          activeCount += 1;
        }
      }
    }

    return activeCount;
  }

  public bool PuzzleContainsNonForbiddenCubes() {
    return puzzleCubes.Exists(c => c != null && c.GetComponent<CubeType>().CurrentType != CubeType.Type.Forbidden);
  }

  public bool PuzzleContainsOnlyForbiddenCubes() {
    return puzzleCubes.TrueForAll(c => c == null || c.GetComponent<CubeType>().CurrentType == CubeType.Type.Forbidden);
  }

  public void CleanUpDestroyedCubes() {
    puzzleCubes.RemoveAll(cube => cube == null);
  }

  // NOTE: Overly defensive?
  bool AllCubesStationary() {
    return !puzzleCubes.Exists(c => c != null && c.GetComponent<Tumble>().isMoving);
  }

  public string StateQueueStatus() {
    var output = "";
    foreach (var state in stateQueue) {
      if (output != "") {
        output += ", ";
      }

      output += "[" + state.ToString() + "]";
    }

    return output;
  }

  void ResetQueue() {
    stateQueue.Clear();
    stateQueue.Enqueue(State.Roll);
    stateQueue.Enqueue(State.RollPaused);
  }
}
