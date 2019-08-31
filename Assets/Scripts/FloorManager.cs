using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FloorManager : MonoBehaviour {
  public GameObject FloorCubePrefab;
  const float FLOOR_HEIGHT = -GameConsts.CubeSize;

  Stack<FloorStack> floorStacks;

  void Awake() {
    Reset();
  }

  void OnEnable() {
    Destroyable.OnCubeScored += HandleCubeScored;
  }

  void OnDisable() {
    Destroyable.OnCubeScored -= HandleCubeScored;
  }

  void HandleCubeScored(GameObject scoredCube) {
    var cubeType = scoredCube.GetComponent<CubeType>().CurrentType;

    if (cubeType == CubeType.Type.Forbidden) {
      GameManager.instance.CurrentPuzzlePlayerMadeMistakes = true;

      DropLast();
    }
  }

  public void Reset() {
    floorStacks = new Stack<FloorStack>();
  }

  public void Add(int width) {
    var nextStackLocation = new Vector3(0, FLOOR_HEIGHT, 0);
    FloorStack lastStack = null;

    if (floorStacks.Count > 0) {
      lastStack = floorStacks.Peek();
    }

    if (lastStack != null) {
      var lastStackDepth = lastStack.CubeDepth();
      nextStackLocation.z = lastStackDepth - 1;
    }

    var floorStack = gameObject.AddComponent<FloorStack>();
    floorStack.Build(FloorCubePrefab, nextStackLocation, width);
    floorStacks.Push(floorStack);
  }

  public void DropLast() {
    var droppedStack = floorStacks.Pop();

    if (droppedStack != null) {
      droppedStack.Drop();
    }
  }

  public bool IsFloorBelowVec3(Vector3 vec) {
    foreach (var fs in floorStacks) {
      if (fs.IsFloorBelowVec3(vec))
        return true;
    }

    return false;
  }

  public float SideToSidePositionOnFloor(Transform transform) {
    var firstFloorStack = floorStacks.Peek();

    var normalizedPosition = transform.position.x + (GameConsts.CubeSize / 2);
    var sideToSidePercentage = normalizedPosition / firstFloorStack.Width();

    return Mathf.Lerp(1f, -1f, sideToSidePercentage);
  }
}
