using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Util {
  public static Vector3 FloorVec3(Vector3 original) {
    return new Vector3(Mathf.Ceil(original.x), Mathf.Ceil(original.y), Mathf.Ceil(original.z));
  }

  public static bool Vec3sEqualXandZ(Vector3 vec1, Vector3 vec2) {
    return vec1.x == vec2.x && vec1.z == vec2.z;
  }

  public struct CubePositionAndType {
    public Vector3 position;
    public CubeType.Type type;

    public CubePositionAndType(Vector3 position, CubeType.Type type) {
      this.position = position;
      this.type = type;
    }
  }
}
