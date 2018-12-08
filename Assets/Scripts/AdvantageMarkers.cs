using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvantageMarkers : MonoBehaviour {
  public GameObject MarkerPrefab;
  public List<GameObject> AdvantageMarkersList = new List<GameObject>();

  public static event SharedEvents.MarkerDetonationEvent OnAdvantageMarkerDetonation;

  void OnEnable() {
    Destroyable.OnCubeScored += HandleCubeScored;
  }

  void OnDisable() {
    Destroyable.OnCubeScored -= HandleCubeScored;
  }

  void HandleCubeScored(GameObject scoredCube) {
    if (scoredCube.GetComponent<CubeType>().CurrentType == CubeType.Type.Advantage) {
      var destroyedAdvantageCubePosition = scoredCube.transform.position;

      for (float x = -1; x < 2; x++) {
        for (float z = -1; z < 2; z++) {
          var newAdvantageMarkerPosition = new Vector3(
            destroyedAdvantageCubePosition.x + x,
            PlayerMarker.DEFAULT_MARKER_HEIGHT,
            destroyedAdvantageCubePosition.z + z
          );

          if (GameManager.instance.boardManager.floorManager.IsFloorBelowVec3(newAdvantageMarkerPosition)) {
            var advantageMarker = (GameObject)Instantiate(MarkerPrefab, newAdvantageMarkerPosition, Quaternion.identity);
            ChangeColorOfGameObjectAndAllChildren(advantageMarker, Color.green);

            AdvantageMarkersList.Add(advantageMarker);
          }
        }
      }
    }
  }

  void Start() {
  }

  void Update() {
    if (Input.GetKeyDown(KeyCode.Z)) {

      // NOTE: Copy list of advantage markers, so we don't mutate while iterating it via HandleCubeScored.
      var advantageMarkersListToIterate = new List<GameObject>(AdvantageMarkersList);
      AdvantageMarkersList.Clear();

      if (advantageMarkersListToIterate.Count > 0) {
        for (int i = 0; i < advantageMarkersListToIterate.Count; i++) {
          var advantageMarker = advantageMarkersListToIterate[i];

          if (OnAdvantageMarkerDetonation != null) {
            // Note: See -- PROTECTING QUBES -- section of FAQ.
            if (!GetComponent<PlayerMarker>().OtherMarkerOverlapping(advantageMarker)) {
              OnAdvantageMarkerDetonation(advantageMarker);
            }
          }

          advantageMarkersListToIterate[i] = null;
          Destroy(advantageMarker);
        }

        advantageMarkersListToIterate.Clear();
      }
    }
  }

  void ChangeColorOfGameObjectAndAllChildren(GameObject obj, Color newColor) {
    var advantageMarkerMaterials = obj.GetComponent<Renderer>().materials;
    advantageMarkerMaterials[0].color = newColor;

    var renderers = obj.GetComponentsInChildren<Renderer>();
    foreach (var r in renderers) {
      foreach (var m in r.materials) {
        m.color = newColor;
      }
    }
  }
}
