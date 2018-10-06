using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puzzle : MonoBehaviour {
  List<GameObject> puzzleCubes = new List<GameObject>();

  bool active = false;
  float sleepingFor = 0f;

  public enum State { Roll, RollPaused, DetonationPaused };
  Dictionary<State, float> StateDelays = new Dictionary<State, float>() {
    {State.Roll, 1f},
    {State.RollPaused, 0.46f},
    {State.DetonationPaused, 2.16f},
  };
  Queue<State> stateQueue = new Queue<State>();

  void OnEnable() {
    BoardManager.OnCubeDetonation += HandleCubeDetonation;
  }

  void OnDisable() {
    BoardManager.OnCubeDetonation -= HandleCubeDetonation;
  }

  void Awake() {
    ResetQueue();
  }

  void Start() {
  }

  void HandleCubeDetonation(GameObject destroyedCube) {
    if (stateQueue.Count > 0) {
      if (stateQueue.Peek() != State.DetonationPaused) {
        stateQueue.Enqueue(State.DetonationPaused);
      }
    } else {
      stateQueue.Enqueue(State.DetonationPaused);
    }
  }

  // TODO: Good candidate to switch to a Coroutine?
  void Update() {
    if (active) {
      if (sleepingFor > 0f) {
        sleepingFor -= Time.deltaTime * GameManager.instance.TumbleSpeedMultiplier();
      } else {
        if (AllCubesStationary()) {
          GameManager.instance.boardManager.DestroyAnyStationaryDestructionAwaitingCubes();

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
  }

  public void Build(GameObject cubePrefab, List<Util.CubePositionAndType> positionsAndTypes) {
    int index = 0;
    foreach (var positionAndType in positionsAndTypes) {
      GameObject cube = (GameObject)Instantiate(cubePrefab, positionAndType.position, Quaternion.identity);
      cube.GetComponent<CubeType>().CurrentType = positionAndType.type;

      puzzleCubes.Add(cube);
      index++;
    }
  }

  public void MoveAllCubes(Vector3 direction, float duration) {
    foreach (var c in puzzleCubes) {
      if (c != null && !c.GetComponent<Tumble>().isFalling) {
        c.GetComponent<Tumble>().TumbleInDirection(direction, duration);
      }
    }
  }

  public void Activate() {
    active = true;
  }

  public bool IsActive() {
    return active;
  }

  public void CleanUpDestroyedCubes() {
    puzzleCubes.RemoveAll(cube => cube == null);
  }

  // NOTE: Overly defensive?
  bool AllCubesStationary() {
    return !puzzleCubes.Exists(c => c != null && c.GetComponent<Tumble>().isMoving);
  }

  void ResetQueue() {
    stateQueue.Clear();
    stateQueue.Enqueue(State.Roll);
    stateQueue.Enqueue(State.RollPaused);
  }
}
