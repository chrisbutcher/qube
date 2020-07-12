using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
  public void ResumeGame() {
    GameManager.GameManagerInstance().PlayerPaused = false;
    GameManager.GameManagerInstance().ActivateGame();

    GameObject.FindGameObjectWithTag("PauseMenu").GetComponent<Canvas>().enabled = false;
  }

  public void BackToMenu() {
    SceneManager.LoadScene("Menu");
  }

  public void Quit() {
    Application.Quit();
  }
}
