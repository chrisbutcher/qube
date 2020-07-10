using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour {
  public void StartGame() {
    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().Play();
    StartCoroutine(LoadGame(2f));
  }

  IEnumerator LoadGame(float delay) {
    GameObject.FindGameObjectWithTag("MenuFader").GetComponent<Animator>().SetTrigger("Fade");

    yield return new WaitForSeconds(delay);

    SceneManager.LoadScene("Main");
  }

  public void LoadStage1() {
    PersistentState.SelectedStage = 0;
    StartGame();
  }

  public void LoadStage2() {
    PersistentState.SelectedStage = 1;
    StartGame();
  }

  public void LoadStage3() {
    PersistentState.SelectedStage = 2;
    StartGame();
  }

  public void LoadStage4() {
    PersistentState.SelectedStage = 3;
    StartGame();
  }

  public void LoadStage5() {
    PersistentState.SelectedStage = 4;
    StartGame();
  }

  public void LoadStage6() {
    PersistentState.SelectedStage = 5;
    StartGame();
  }

  public void LoadStage7() {
    PersistentState.SelectedStage = 6;
    StartGame();
  }

  public void LoadStage8() {
    PersistentState.SelectedStage = 7;
    StartGame();
  }

  public void LoadStage9() {
    PersistentState.SelectedStage = 8;
    StartGame();
  }
}
