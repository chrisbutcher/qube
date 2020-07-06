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
    framesUntilUpdate -= 1;

    if (framesUntilUpdate <= 0) {
      framesUntilUpdate = updateEveryNFrames;
    } else {
      return;
    }

    CurrentStage.text = (GameManager.instance.CurrentStageIndex + 1).ToString();
    Score.text = $"Score: {GameManager.instance.CurrentStageScore}";

    var rotationsSinceFirstCubeDestroyed = GameManager.instance.boardManager.CurrentPuzzleOrNextPuzzleUp().RotationsSinceFirstCubeDestroyed;
    var typicalRotationNumber = GameManager.instance.boardManager.CurrentPuzzleOrNextPuzzleUp().TypicalRotationNumber;

    Turns.text = $"{rotationsSinceFirstCubeDestroyed} / {typicalRotationNumber}";

    if (rotationsSinceFirstCubeDestroyed <= typicalRotationNumber) {
      Turns.color = Color.cyan;
    } else {
      Turns.color = Color.red;
    }

    // Block scale UI start (move to function?)
    var blockScaleAscii = "";
    for (int i = 0; i < GameManager.instance.CurrentWaveBlockScaleAvailable - GameManager.instance.CurrentWaveBlockScaleUsed; i++) {
      blockScaleAscii += "☐";
    }

    for (int i = 0; i < GameManager.instance.CurrentWaveBlockScaleUsed; i++) {
      blockScaleAscii += "☒";
    }

    BlockScale.text = blockScaleAscii;
    // Block scale UI end

    // Wave UI ░ █
    var waveCount = GameManager.instance.CurrentStage.Waves.Count;
    var waveAscii = "";

    for (int i = 0; i < GameManager.instance.CurrentWaveIndex + 1; i++) {
      waveAscii += "█ ";
    }

    for (int i = 0; i < GameManager.instance.CurrentStage.Waves.Count - (GameManager.instance.CurrentWaveIndex + 1); i++) {
      waveAscii += "░ ";
    }

    CurrentWave.text = waveAscii;

    if (ShowAnnounceForNSeconds > 0f) {
      ShowAnnounceForNSeconds -= Time.deltaTime;
    } else {
      HideAnnounce();
    }
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
