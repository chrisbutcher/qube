﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Util {
  public static Vector3 FloorVec3(Vector3 original) {
    return new Vector3(Mathf.Ceil(original.x), Mathf.Ceil(original.y), Mathf.Ceil(original.z));
  }

  public static bool Vec3sEqualXandZ(Vector3 vec1, Vector3 vec2) {
    // NOTE: Casting these to ints and comparing didn't do anything
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

  public static void ChangeColorOfGameObjectAndAllChildren(GameObject obj, Color newColor) {
    var advantageMarkerMaterials = obj.GetComponent<Renderer>().materials;
    advantageMarkerMaterials[0].color = newColor;

    var renderers = obj.GetComponentsInChildren<Renderer>();
    foreach (var r in renderers) {
      foreach (var m in r.materials) {
        m.color = newColor;
      }
    }
  }

  public static void ParentInstanceUnderEmpty(GameObject gameObject, string emptyObjectName) { // TODO Better name for these?
    var floorGroup = GameObject.Find(emptyObjectName);
    gameObject.transform.SetParent(floorGroup.transform, false);
  }

  public static void ParentInstanceUnderEmpty(GameObject gameObject, GameObject parentGameObject) {
    gameObject.transform.SetParent(parentGameObject.transform, false);
  }
}
