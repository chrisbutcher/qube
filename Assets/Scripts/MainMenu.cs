using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
  public void StartGame() {
    GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>().Play();
    StartCoroutine(LoadGame(2f));
  }

  IEnumerator LoadGame(float delay) {
    GameObject.FindGameObjectWithTag("MenuFader").GetComponent<Animator>().SetTrigger("Fade");

    yield return new WaitForSeconds(delay);
    SceneManager.LoadScene("Main");
  }

  public void Quit() {
    Application.Quit();
  }
}
