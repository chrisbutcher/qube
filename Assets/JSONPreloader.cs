using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;
using System;

public class JSONPreloader : MonoBehaviour {
  const string PUZZLES_FILENAME = "puzzles.json";
  // const string FILENAME = "puzzles_test.json";

  const string STAGE_DEFINITIONS_FILENAME = "stage_definitions.json";
  // const string STAGE_DEFINITIONS_FILENAME = "stage_definitions_alt.json";

  void Awake() {
    LoadStageDefinitions();
    LoadPuzzles();
  }

  void LoadStageDefinitions() {
    string filePath = Path.Combine(Application.streamingAssetsPath, STAGE_DEFINITIONS_FILENAME);

    if (filePath.Contains("://") || filePath.Contains(":///")) {
      StartCoroutine(LoadStageDefinitionsWebAndAndroid(filePath, (jsonString) => {
        PersistentState.stageDefinitions = StageDefinitions.FromJson(jsonString);
      }));
    } else {
      StartCoroutine(LoadStageDefinitionsPC(filePath, (jsonString) => {
        PersistentState.stageDefinitions = StageDefinitions.FromJson(jsonString);
      }));
    }
  }

  void LoadPuzzles() {
    string filePath = Path.Combine(Application.streamingAssetsPath, PUZZLES_FILENAME);

    if (filePath.Contains("://") || filePath.Contains(":///")) {
      StartCoroutine(LoadStageDefinitionsWebAndAndroid(filePath, (jsonString) => {
        PersistentState.puzzlesJSONString = jsonString;
      }));
    } else {
      StartCoroutine(LoadStageDefinitionsPC(filePath, (jsonString) => {
        PersistentState.puzzlesJSONString = jsonString;
      }));
    }
  }

  IEnumerator LoadStageDefinitionsPC(string filePath, Action<string> storeAction) {
    var jsonString = File.ReadAllText(filePath);
    storeAction(jsonString);
    yield return null;
  }

  IEnumerator LoadStageDefinitionsWebAndAndroid(string filePath, Action<string> storeAction) {
    using (UnityWebRequest webRequest = UnityWebRequest.Get(filePath)) {
      // Request and wait for the desired page.
      yield return webRequest.SendWebRequest();

      if (webRequest.isNetworkError) {
      } else {
        var jsonString = webRequest.downloadHandler.text;
        storeAction(jsonString);

        yield return null;
      }
    }
  }
}
