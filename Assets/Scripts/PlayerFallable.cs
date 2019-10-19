using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFallable : MonoBehaviour {
  const string FloorCubeTag = "Floor Cube";
  const float PlayerFloodRayCastDistance = 2f;

  void OnEnable() {
    FloorManager.OnCubeStackDropped += HandleCubeStackDropped;
  }

  void OnDisable() {
    FloorManager.OnCubeStackDropped -= HandleCubeStackDropped;
  }

  void HandleCubeStackDropped() {
    RaycastHit hit;
    LayerMask layerMask = Physics.AllLayers;

    var playerPosition = GetComponent<Rigidbody>().position;

    if (Physics.Raycast(playerPosition, Vector3.down, out hit, PlayerFloodRayCastDistance, layerMask, QueryTriggerInteraction.Collide)) {
      var cubeRB = hit.collider.GetComponent<Rigidbody>();

      if (hit.collider.tag == FloorCubeTag && cubeRB.useGravity == true) {
        var playerRB = GetComponent<Rigidbody>();
        playerRB.detectCollisions = true;
        playerRB.useGravity = true;
        playerRB.constraints = RigidbodyConstraints.None;
      }
    }
  }

  void Start() {
  }

  void Update() {
  }
}
