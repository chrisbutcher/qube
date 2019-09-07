using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorStack : MonoBehaviour {
  const int FLOOR_STACK_HEIGHT = 5;

  List<GameObject> cubes = new List<GameObject>();

  private int width;

  void Awake() {
  }

  void Start() {
  }

  void Update() {
  }

  // TODO rename this Add, and let callers simply ask to have a new stack of floor cubes added (eventually animated)
  public void Build(GameObject floorCubePrefab, Vector3 position, int floorStackWidth) {
    for (int y = 0; y < FLOOR_STACK_HEIGHT; y++) {
      for (int x = 0; x < floorStackWidth; x++) {
        GameObject cube = (GameObject)Instantiate(floorCubePrefab, position + new Vector3(x, -y, 0f), Quaternion.identity);
        Util.ParentInstanceUnderEmpty(cube, "FloorGroup");

        cubes.Add(cube);
        width = floorStackWidth;
      }
    }
  }

  public bool IsFloorBelowVec3(Vector3 vec) {
    var quantizedVec = Util.FloorVec3(vec);

    foreach (var cube in cubes) {
      if (Util.Vec3sEqualXandZ(quantizedVec, cube.transform.position))
        return true;
    }

    return false;
  }

  public float CubeDepth() {
    if (cubes.Count > 0) {
      return cubes[0].transform.position.z;
    } else {
      return 0f;
    }
  }

  public void Drop() {
    foreach (var cube in cubes) {
      var rb = cube.GetComponent<Rigidbody>();
      rb.detectCollisions = true;
      rb.useGravity = true;
      rb.constraints = RigidbodyConstraints.None;
    }
  }

  public float Width() {
    return (float)width;
  }
}
