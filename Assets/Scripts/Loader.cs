using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Loader : MonoBehaviour {
  public GameObject gameManager;
  public GameObject[] exampleGameObjects;

  void Awake() {
    foreach (var e in exampleGameObjects) {
      Destroy(e);
    }

    if (GameManager.instance == null) {
      Instantiate(gameManager);
    }
  }

  void Start() {
    GameManager.instance.ActivateGame();
  }
}
