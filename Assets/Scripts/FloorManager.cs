using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FloorManager : MonoBehaviour {
  public GameObject FloorCubePrefab;
  const float FLOOR_HEIGHT = -GameConsts.CubeSize;

  public static event SharedEvents.CubeStackDropped OnCubeStackDropped;

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

  void HandleCubeScored(GameObject scoredCube, MarkerType.Type scoredByMarkerType) {
    var cubeType = scoredCube.GetComponent<CubeType>().CurrentType;

    if (cubeType == CubeType.Type.Forbidden) {
      GameManager.instance.CurrentPuzzlePlayerMadeMistakes = true;

      DropLast();
    }
  }

  public void Reset() {
    floorStacks = new Stack<FloorStack>();
  }

  public void Add(int width, bool animate) {
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

    if (animate) {
      StartCoroutine(AnimateInFloorStack(floorStack.ParentGameObject));
    }
  }

  IEnumerator AnimateInFloorStack(GameObject floorStackParent) {
    float time = .7f; // TODO constantize
    float elapsedTime = 0;

    var targetPosition = floorStackParent.transform.position;
    var initialPosition = targetPosition - (Vector3.forward * 6); // TODO constantize

    floorStackParent.transform.position = initialPosition;

    while (elapsedTime < time) {
      floorStackParent.transform.position = new Vector3(
        initialPosition.x,
        initialPosition.y,
        Mathf.SmoothStep(initialPosition.z, targetPosition.z, (elapsedTime / time))
      );

      elapsedTime += Time.deltaTime;
      yield return null;
    }

    // Compensate for some floating point innaccuracy that messes with Util.Vec3sEqualXandZ
    floorStackParent.transform.position = targetPosition; // TODO: Not working??
  }

  public void DropLast() {
    var droppedStack = floorStacks.Pop();

    if (droppedStack != null) {
      droppedStack.Drop();
      OnCubeStackDropped();
    }
  }

  public bool IsFloorBelowVec3(Vector3 vec) {
    foreach (var fs in floorStacks) {
      if (fs.IsFloorBelowVec3(vec))
        return true;
    }

    return false;
  }

  public float SideToSidePositionOnFloor(Vector3 position) {
    var firstFloorStack = floorStacks.Peek();

    var normalizedPosition = position.x + (GameConsts.CubeSize / 2);
    var sideToSidePercentage = normalizedPosition / firstFloorStack.Width();

    return Mathf.Lerp(1f, -1f, sideToSidePercentage);
  }

  public GameObject GetNearestRightMostFloorCube() {
    var nearestFloorStack = floorStacks.Peek();

    return nearestFloorStack.GetNearestRightMostFloorCube();
  }
}
