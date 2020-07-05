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

  Text CurrentPuzzleStateQueue;

  Text PostPuzzleAnnounce;
  float ShowAnnounceForNSeconds = 0f;

  void Awake() {
    CurrentStage = GameObject.FindGameObjectWithTag("UI-CurrentStage").GetComponent<Text>();
    CurrentWave = GameObject.FindGameObjectWithTag("UI-CurrentWave").GetComponent<Text>();
    Score = GameObject.FindGameObjectWithTag("UI-Score").GetComponent<Text>();
    Turns = GameObject.FindGameObjectWithTag("UI-Turns").GetComponent<Text>();
    BlockScale = GameObject.FindGameObjectWithTag("UI-BlockScale").GetComponent<Text>();
    PostPuzzleAnnounce = GameObject.FindGameObjectWithTag("UI-PostPuzzleAnnounce").GetComponent<Text>();
    // CurrentPuzzleStateQueue = GameObject.FindGameObjectWithTag("UI-CurrentPuzzleStateQueue").GetComponent<Text>();
  }

  void Update() {
    // if (!GameManager.instance.isGameActive()) {
    //   return;
    // }

    // TODO: Update every few frames?
    CurrentStage.text = (GameManager.instance.CurrentStageIndex + 1).ToString();
    CurrentWave.text = $"Wave: {GameManager.instance.CurrentWaveIndex + 1}";
    Score.text = $"Score: {GameManager.instance.CurrentStageScore}";
    Turns.text = $"{GameManager.instance.boardManager.CurrentPuzzleOrNextPuzzleUp().RotationsSinceFirstCubeDestroyed} / {GameManager.instance.boardManager.CurrentPuzzleOrNextPuzzleUp().TypicalRotationNumber}";
    BlockScale.text = $"{GameManager.instance.CurrentWaveBlockScaleUsed} / {GameManager.instance.CurrentWaveBlockScaleAvailable}";
    // CurrentPuzzleStateQueue.text = GameManager.instance.CurrentPuzzleStateQueueStatus();

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
