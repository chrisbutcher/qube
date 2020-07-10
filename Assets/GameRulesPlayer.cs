using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class GameRulesPlayer : MonoBehaviour {
  VideoPlayer videoPlayer;
  RawImage rawImage;

  void Start() {
    videoPlayer = GetComponent<VideoPlayer>();
    rawImage = GetComponent<RawImage>();

    StartCoroutine(PlayVideo());
  }

  void OnEnable() {
    videoPlayer = GetComponent<VideoPlayer>();
    rawImage = GetComponent<RawImage>();

    StartCoroutine(PlayVideo());
  }

  IEnumerator PlayVideo() {
    videoPlayer.Prepare();

    WaitForSeconds waitForSeconds = new WaitForSeconds(1);

    while (!videoPlayer.isPrepared) {
      yield return waitForSeconds;
      break;
    }

    rawImage.texture = videoPlayer.texture;
    videoPlayer.Play();
  }
}
