using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkerType : MonoBehaviour {

  public enum Type {
    PlayerMarker,
    AdvantageMarker,
    DestroyedMarker,
  }

  public float FloatDestroyInSeconds;

  public Type CurrentType;

  public static void SpawnDestroyedMarkerAt(Vector3 spawnPosition) {
    var prefab = GameManager.instance.Players[0].GetComponent<PlayerMarker>().PlayerMarkerPrefab;
    var destroyedPlayerMarker = (GameObject)Instantiate(prefab, spawnPosition, Quaternion.identity);
    destroyedPlayerMarker.GetComponent<MarkerType>().CurrentType = MarkerType.Type.DestroyedMarker;
    destroyedPlayerMarker.GetComponent<MarkerType>().FloatDestroyInSeconds = .5f;
    Util.ChangeColorOfGameObjectAndAllChildren(destroyedPlayerMarker, Color.red);
  }

  void Update() {
    if (CurrentType is Type.DestroyedMarker && FloatDestroyInSeconds > 0f) {
      FloatDestroyInSeconds -= Time.deltaTime;

      if (FloatDestroyInSeconds < 0f) {
        Destroy(this.gameObject);
      }
    }
  }
}
