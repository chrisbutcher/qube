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

        // TODO: Find any markers with `MarkerType>().CurrentType = MarkerType.Type.DestroyedMarker` and destroy them
        // instantly if they overlap positionally with this new player marker
        var existingPlayerMarkersArray = GameObject.FindObjectsOfType<MarkerType>();
        var existingPlayerMarkers = new List<MarkerType>(existingPlayerMarkersArray);

        foreach (var foo in existingPlayerMarkers) {
          Debug.Log(foo.CurrentType);
        }

        CurrentPlayerMarker.transform.position = new Vector3(
          quantizedPlayerPosition.x,
          DEFAULT_MARKER_HEIGHT,
          quantizedPlayerPosition.z
        );
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
