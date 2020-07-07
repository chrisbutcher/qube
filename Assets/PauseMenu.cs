using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {
  public void ResumeGame() {
    GameManager.instance.PlayerPaused = false;
    GameManager.instance.ActivateGame();

    GameObject.FindGameObjectWithTag("PauseMenu").GetComponent<Canvas>().enabled = false;
  }

  public void Quit() {
    Application.Quit();
  }
}
