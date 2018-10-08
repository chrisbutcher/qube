using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class PuzzleLoader {

  public bool LoadPuzzle(int width, int height) {
    const string FILENAME = "puzzles.json";

    string filePath = Path.Combine(Application.streamingAssetsPath, FILENAME);

    if (File.Exists(filePath)) {
      Debug.Log("File exists!");

      var fileContents = File.ReadAllText(filePath);
      JObject puzzles = JObject.Parse(fileContents); // TODO: Just keep this object around? Singleton?
      Debug.Log(puzzles.Count);

      var key = string.Format("{0}x{1}", width, height);
      var puzzlesOfSize = (JArray)puzzles[key];
      Debug.Log(puzzlesOfSize.Count);
    }

    return false;
  }
}
