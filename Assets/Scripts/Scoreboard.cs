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
  Text PostPuzzleAnnounce;

  const int updateEveryNFrames = 15;

  float ShowAnnounceForNSeconds = 0f;
  int framesUntilUpdate = updateEveryNFrames;

  void Awake() {
    CurrentStage = GameObject.FindGameObjectWithTag("UI-CurrentStage").GetComponent<Text>();
    CurrentWave = GameObject.FindGameObjectWithTag("UI-CurrentWave").GetComponent<Text>();
    Score = GameObject.FindGameObjectWithTag("UI-Score").GetComponent<Text>();
    Turns = GameObject.FindGameObjectWithTag("UI-Turns").GetComponent<Text>();
    BlockScale = GameObject.FindGameObjectWithTag("UI-BlockScale").GetComponent<Text>();
    PostPuzzleAnnounce = GameObject.FindGameObjectWithTag("UI-PostPuzzleAnnounce").GetComponent<Text>();
  }

  void Start() {
    CurrentStage.text = "";
    CurrentWave.text = "";
    Score.text = "";
    Turns.text = "";
    BlockScale.text = "";
    PostPuzzleAnnounce.text = "";
  }

  void Update() {
    if (ShowAnnounceForNSeconds > 0f) {
      ShowAnnounceForNSeconds -= Time.deltaTime;
    } else {
      HideAnnounce();
    }

    framesUntilUpdate -= 1;

    if (framesUntilUpdate <= 0) {
      framesUntilUpdate = updateEveryNFrames;
    } else {
      return;
    }

    CurrentStage.text = (GameManager.GameManagerInstance().CurrentStageIndex + 1).ToString();
    Score.text = $"Score: {GameManager.GameManagerInstance().CurrentStageScore}";

    var rotationsSinceFirstCubeDestroyed = GameManager.GameManagerInstance().boardManager.CurrentPuzzleOrNextPuzzleUp().RotationsSinceFirstCubeDestroyed;
    var typicalRotationNumber = GameManager.GameManagerInstance().boardManager.CurrentPuzzleOrNextPuzzleUp().TypicalRotationNumber;

    Turns.text = $"{rotationsSinceFirstCubeDestroyed} / {typicalRotationNumber}";

    if (rotationsSinceFirstCubeDestroyed <= typicalRotationNumber) {
      Turns.color = Color.cyan;
    } else {
      Turns.color = Color.red;
    }

    // Block scale UI start (move to function?)
    var blockScaleAscii = "";
    for (int i = 0; i < GameManager.GameManagerInstance().CurrentWaveBlockScaleAvailable - GameManager.GameManagerInstance().CurrentWaveBlockScaleUsed; i++) {
      blockScaleAscii += "[ ] ";
    }

    for (int i = 0; i < GameManager.GameManagerInstance().CurrentWaveBlockScaleUsed; i++) {
      blockScaleAscii += "[x] ";
    }

    BlockScale.text = blockScaleAscii;
    // Block scale UI end

    // Wave UI ░ █
    var waveCount = GameManager.GameManagerInstance().CurrentStage.Waves.Count;
    var waveAscii = "";

    for (int i = 0; i < GameManager.GameManagerInstance().CurrentWaveIndex + 1; i++) {
      waveAscii += "█ ";
    }

    for (int i = 0; i < GameManager.GameManagerInstance().CurrentStage.Waves.Count - (GameManager.GameManagerInstance().CurrentWaveIndex + 1); i++) {
      waveAscii += "░ ";
    }

    CurrentWave.text = waveAscii;
  }

  public void ShowAnnounce(string text, float duration) {
    PostPuzzleAnnounce.text = text;
    ShowAnnounceForNSeconds = duration;
  }

  public void HideAnnounce() {
    PostPuzzleAnnounce.text = "";
    ShowAnnounceForNSeconds = 0f;
  }
}
