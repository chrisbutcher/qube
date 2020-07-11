using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameRulesPlayer : MonoBehaviour {
  RawImage rawImage;

  public List<Texture> tutorialImages;

  void Start() {
    rawImage = GetComponent<RawImage>();
    StartCoroutine(PlayTutorial());
  }

  void OnEnable() {
    StartCoroutine(PlayTutorial());
  }

  IEnumerator PlayTutorial() {
    WaitForSeconds waitForSeconds;

    while (true) {
      waitForSeconds = new WaitForSeconds(1);
      yield return waitForSeconds;

      rawImage.texture = tutorialImages[0];
      waitForSeconds = new WaitForSeconds(2);
      yield return waitForSeconds;

      rawImage.texture = tutorialImages[1];
      waitForSeconds = new WaitForSeconds(1);
      yield return waitForSeconds;

      rawImage.texture = tutorialImages[2];
      waitForSeconds = new WaitForSeconds(1);
      yield return waitForSeconds;

      rawImage.texture = tutorialImages[3];
      waitForSeconds = new WaitForSeconds(1);
      yield return waitForSeconds;

      rawImage.texture = tutorialImages[4];
      waitForSeconds = new WaitForSeconds(1);
      yield return waitForSeconds;

      rawImage.texture = tutorialImages[5];
      waitForSeconds = new WaitForSeconds(3.5f);
      yield return waitForSeconds;

      rawImage.texture = tutorialImages[6];
      waitForSeconds = new WaitForSeconds(2);
      yield return waitForSeconds;

      rawImage.texture = tutorialImages[7];
      waitForSeconds = new WaitForSeconds(1);
      yield return waitForSeconds;

      rawImage.texture = tutorialImages[8];
      waitForSeconds = new WaitForSeconds(1);
      yield return waitForSeconds;

      rawImage.texture = tutorialImages[9];
      waitForSeconds = new WaitForSeconds(3);
      yield return waitForSeconds;

      rawImage.texture = tutorialImages[10];
      waitForSeconds = new WaitForSeconds(3.5f);
      yield return waitForSeconds;

      rawImage.texture = tutorialImages[11];
      waitForSeconds = new WaitForSeconds(3);
      yield return waitForSeconds;

      rawImage.texture = tutorialImages[12];
      waitForSeconds = new WaitForSeconds(5);
      yield return waitForSeconds;

      rawImage.texture = tutorialImages[13];
      waitForSeconds = new WaitForSeconds(2);
      yield return waitForSeconds;

      rawImage.texture = tutorialImages[14];
      waitForSeconds = new WaitForSeconds(4);
      yield return waitForSeconds;
    }
  }
}
