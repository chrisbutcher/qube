using UnityEngine;
using System.Collections;

public class FrameRateCounter : MonoBehaviour {
  float deltaTime = 0.0f;

  const int updateEveryNFrames = 120;
  int framesUntilUpdate = 0;

  GUIStyle style;
  Rect rect;
  string text;

  void Start() {
    int w = Screen.width, h = Screen.height;

    rect = new Rect(0, 0, w, h - 20);

    style = new GUIStyle();
    style.alignment = TextAnchor.LowerLeft;
    style.fontSize = h * 2 / 100;
    style.normal.textColor = Color.white;
  }

  void Update() {
    deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
  }

  void OnGUI() {
    if (framesUntilUpdate > 0) {
      framesUntilUpdate -= 1;
      GUI.Label(rect, text, style);
      return;
    } else {
      framesUntilUpdate = updateEveryNFrames;
    }

    float msec = deltaTime * 1000.0f;
    float fps = 1.0f / deltaTime;
    text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
    GUI.Label(rect, text, style);
  }
}