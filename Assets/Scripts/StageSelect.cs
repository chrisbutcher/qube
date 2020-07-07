using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageSelect : MonoBehaviour {
  public void LoadStage2() {
    // TODO: Store selected stage in a static var and read in GameManager
    // https://answers.unity.com/questions/229589/passing-parameters-between-scences.html 
    SceneManager.LoadScene("Main");
  }
}
