using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour {
  Text CurrentStage;
  Text CurrentWave;
  Text Score;
  Text Turns;
  Text BlockScale;

  void Start() {
    CurrentStage = GameObject.FindGameObjectWithTag("UI-CurrentStage").GetComponent<Text>();
    CurrentWave = GameObject.FindGameObjectWithTag("UI-CurrentWave").GetComponent<Text>();
    Score = GameObject.FindGameObjectWithTag("UI-Score").GetComponent<Text>();
    Turns = GameObject.FindGameObjectWithTag("UI-Turns").GetComponent<Text>();
    BlockScale = GameObject.FindGameObjectWithTag("UI-BlockScale").GetComponent<Text>();
  }

  void Update() {
    CurrentStage.text = (GameManager.instance.CurrentStageIndex + 1).ToString();
    CurrentWave.text = $"Wave: {GameManager.instance.CurrentWaveIndex + 1}";
    Score.text = $"Score: {GameManager.instance.CurrentStageScore}";
    Turns.text = $"{GameManager.instance.boardManager.CurrentPuzzleOrNextPuzzleUp().RotationsSinceFirstCubeDestroyed} / {GameManager.instance.boardManager.CurrentPuzzleOrNextPuzzleUp().TypicalRotationNumber}";
    BlockScale.text = $"{GameManager.instance.CurrentWaveBlockScaleUsed} / {GameManager.instance.CurrentWaveBlockScaleAvailable}";
  }
}
