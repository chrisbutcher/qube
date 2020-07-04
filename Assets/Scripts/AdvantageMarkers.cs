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

  public bool HasAdvantageMarkerOnPosition(Vector3 position) {
    foreach (var advantageMarker in AdvantageMarkersList) {
      if (Util.Vec3sEqualXandZ(advantageMarker.transform.position, position)) {
        return true;
      }
    }

    return false;
  }

  public void ClearAllAdvantageMarkers() {
    for (int i = 0; i < AdvantageMarkersList.Count; i++) {
      var advantageMarker = AdvantageMarkersList[i];
      AdvantageMarkersList[i] = null;
      Destroy(advantageMarker);
    }

    AdvantageMarkersList.Clear();
  }

  void HandleCubeScored(GameObject scoredCube, MarkerType.Type scoredByMarkerType) {
    if (scoredCube.GetComponent<CubeType>().CurrentType == CubeType.Type.Advantage) {
      var advantageMarker = (GameObject)Instantiate(MarkerPrefab, scoredCube.transform.position + new Vector3(0f, 0.005f, 0f), Quaternion.identity);
      advantageMarker.GetComponent<MarkerType>().CurrentType = MarkerType.Type.AdvantageMarker;
      Util.ChangeColorOfGameObjectAndAllChildren(advantageMarker, Color.green); // TODO: Move this color setting logic to MarkerType

      AdvantageMarkersList.Add(advantageMarker);
    }
  }

  void Update() {
    if (GameManager.instance.GetPlayerControls(0).isDetonatingAdvantageMarkers()) {
      // NOTE: Copy list of advantage markers, so we don't mutate while iterating it via HandleCubeScored.
      var advantageMarkersListToIterate = new List<GameObject>(AdvantageMarkersList);
      AdvantageMarkersList.Clear();

      if (advantageMarkersListToIterate.Count > 0) {
        for (int i = 0; i < advantageMarkersListToIterate.Count; i++) {
          var advantageMarker = advantageMarkersListToIterate[i];

          for (float x = -1; x < 2; x++) {
            for (float z = -1; z < 2; z++) {
              var advantageMarkerBlastPosition = new Vector3(
                advantageMarker.transform.position.x + x,
                PlayerMarker.DEFAULT_MARKER_HEIGHT,
                advantageMarker.transform.position.z + z
              );

              if (OnAdvantageMarkerDetonation != null) {
                // Note: See -- PROTECTING QUBES -- section of FAQ.
                if (!GetComponent<PlayerMarker>().OtherMarkerOverlapping(advantageMarkerBlastPosition)) {
                  OnAdvantageMarkerDetonation(advantageMarkerBlastPosition, MarkerType.Type.AdvantageMarker);
                }
              }
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
