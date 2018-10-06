using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMarker : MonoBehaviour {
  public const float DEFAULT_MARKER_HEIGHT = -0.495f;

  public GameObject PlayerMarkerPrefab;
  public GameObject CurrentPlayerMarker = null;

  public static event SharedEvents.MarkerDetonationEvent OnMarkerDetonation;

  void Start() {
    CurrentPlayerMarker = (GameObject)Instantiate(PlayerMarkerPrefab, Vector3.zero, Quaternion.identity);
    CurrentPlayerMarker.SetActive(false);
  }

  void Update() {
    if (Input.GetKeyDown(KeyCode.X)) {
      if (!CurrentPlayerMarker.activeInHierarchy) {
        CurrentPlayerMarker.SetActive(true);

        var quantizedPlayerPosition = GetComponent<PlayerMovement>().QuantizedPlayerPosition;

        CurrentPlayerMarker.transform.position = new Vector3(
          quantizedPlayerPosition.x,
          DEFAULT_MARKER_HEIGHT,
          quantizedPlayerPosition.z
        );
      } else {
        if (OnMarkerDetonation != null) {
          OnMarkerDetonation(CurrentPlayerMarker);
        }

        CurrentPlayerMarker.SetActive(false);
      }
    }
  }

  public bool OtherMarkerOverlapping(GameObject otherMarker) {
    var playerMarkerOverlapping = CurrentPlayerMarker.transform.position == otherMarker.transform.position;
    return CurrentPlayerMarker.activeInHierarchy && playerMarkerOverlapping;
  }
}
