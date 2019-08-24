using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Dynamic;
using System.IO;

public class GameManager : MonoBehaviour {
  const string STAGE_DEFINITIONS_FILENAME = "stage_definitions.json";

  public static GameManager instance = null;
  public BoardManager boardManager;

  public GameObject PlayerPrefab;
  public List<GameObject> Players;

  StageDefinitions stageDefinitions;

  public int CurrentStageIndex = 0;
  public Stage CurrentStage;
  public int CurrentWave;

  void Awake() {
    SingletonSetup();

    LoadStageDefinitions();
    CurrentStage = stageDefinitions.Stages[CurrentStageIndex];

    boardManager = GetComponent<BoardManager>();

    var player = (GameObject)Instantiate(PlayerPrefab, new Vector3(1.5f, 0f, -3.5f), Quaternion.identity);
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

  void LoadStageDefinitions() {
    string filePath = Path.Combine(Application.streamingAssetsPath, STAGE_DEFINITIONS_FILENAME);

    if (File.Exists(filePath)) {
      var jsonString = File.ReadAllText(filePath);
      stageDefinitions = StageDefinitions.FromJson(jsonString);
    }
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
