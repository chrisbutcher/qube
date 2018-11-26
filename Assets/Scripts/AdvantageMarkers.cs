using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvantageMarkers : MonoBehaviour {
  public GameObject MarkerPrefab;
  public List<GameObject> AdvantageMarkersList = new List<GameObject>();

  public static event SharedEvents.MarkerDetonationEvent OnAdvantageMarkerDetonation;

  void OnEnable() {
    BoardManager.OnCubeDetonation += HandleCubeDetonation;
  }

  void OnDisable() {
    BoardManager.OnCubeDetonation -= HandleCubeDetonation;
  }

  void HandleCubeDetonation(GameObject destroyedCube) {
    if (destroyedCube.GetComponent<CubeType>().CurrentType == CubeType.Type.Advantage) {
      var destroyedAdvantageCubePosition = destroyedCube.transform.position;

      for (float x = -1; x < 2; x++) {
        for (float z = -1; z < 2; z++) {
          var newAdvantageMarkerPosition = new Vector3(
            destroyedAdvantageCubePosition.x + x,
            PlayerMarker.DEFAULT_MARKER_HEIGHT,
            destroyedAdvantageCubePosition.z + z
          );

          if (GameManager.instance.boardManager.floorManager.IsFloorBelowVec3(newAdvantageMarkerPosition)) {
            var advantageMarker = (GameObject)Instantiate(MarkerPrefab, newAdvantageMarkerPosition, Quaternion.identity);
            var advantageMarkerMaterials = advantageMarker.GetComponent<Renderer>().materials;
            advantageMarkerMaterials[0].color = Color.green;

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

      // NOTE: Copy list of advantage markers, so we don't mutate while iterating it via HandleCubeDetonation.
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
}
