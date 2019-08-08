using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
  public static GameManager instance = null;
  public BoardManager boardManager;

  public GameObject PlayerPrefab;
  public List<GameObject> Players;

  void Awake() {
    SingletonSetup();

    boardManager = GetComponent<BoardManager>();

    var player = (GameObject)Instantiate(PlayerPrefab, new Vector3(0f, 0f, -3), Quaternion.identity);
    Players.Add(player);
  }

  void Update() {
    if (Input.GetKeyDown(KeyCode.BackQuote)) {
      boardManager.ActivateNextPuzzle();
    }
  }

  public float TumbleSpeedMultiplier() {
    return Input.GetKey(KeyCode.LeftShift) ? 4f : 1f;
  }

  void SingletonSetup() {
    if (instance == null) {
      instance = this;
    } else if (instance != this) {
      Destroy(gameObject);
    }
    DontDestroyOnLoad(gameObject);
  }
}
