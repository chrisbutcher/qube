using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
  public void Start() {
    if (Application.platform == RuntimePlatform.WebGLPlayer) {
      var button = GameObject.FindGameObjectWithTag("HideableQuitButton");
      // button.gameObject.active = false;
      button.SetActive(false);
    };
  }

  public void Quit() {
    Application.Quit();
  }
}
