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
    CurrentPlayerMarker.GetComponent<MarkerType>().CurrentType = MarkerType.Type.PlayerMarker;
    CurrentPlayerMarker.SetActive(false);
  }

  void Update() {
    if (!GameManager.instance.isGameActive()) {
      return;
    }

    if (GameManager.instance.GetPlayerControls(0).isDetonatingPlayerMarker()) {
      if (!CurrentPlayerMarker.activeInHierarchy) {
        var quantizedPlayerPosition = GetComponent<RigidBodyPlayerMovement>().QuantizedPlayerPosition;

        if (GameManager.instance.Players[0].GetComponent<AdvantageMarkers>().HasAdvantageMarkerOnPosition(quantizedPlayerPosition)) {
          return;
        }

        CurrentPlayerMarker.SetActive(true);

        var newCurrentPlayerMarkerPosition = new Vector3(
          quantizedPlayerPosition.x,
          DEFAULT_MARKER_HEIGHT,
          quantizedPlayerPosition.z
        );

        var existingPlayerMarkers = GameObject.FindObjectsOfType<MarkerType>();

        foreach (var existingPlayerMarker in existingPlayerMarkers) {
          if (existingPlayerMarker.CurrentType.Equals(MarkerType.Type.DestroyedMarker)) {
            var differenceBetweenDestroyedMarkerAndCurrentPlayerMarker = newCurrentPlayerMarkerPosition -
              existingPlayerMarker.transform.position;

            if (differenceBetweenDestroyedMarkerAndCurrentPlayerMarker == Vector3.zero) {
              existingPlayerMarker.gameObject.SetActive(false);
            }
          }
        }

        CurrentPlayerMarker.transform.position = newCurrentPlayerMarkerPosition;

      } else {
        if (OnMarkerDetonation != null) {
          OnMarkerDetonation(CurrentPlayerMarker.transform.position, CurrentPlayerMarker.GetComponent<MarkerType>().CurrentType);
        }

        CurrentPlayerMarker.SetActive(false);

        MarkerType.SpawnDestroyedMarkerAt(CurrentPlayerMarker.transform.position);
      }
    }
  }

  public bool OtherMarkerOverlapping(Vector3 otherMarkerPosition) {
    var playerMarkerOverlapping = CurrentPlayerMarker.transform.position == otherMarkerPosition;
    return CurrentPlayerMarker.activeInHierarchy && playerMarkerOverlapping;
  }
}
