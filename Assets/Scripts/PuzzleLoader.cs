using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class PuzzleLoader {
  public struct InternalCube {
    public Vector3 position;
    public CubeType.Type type;

    public InternalCube(Vector3 p, CubeType.Type t) {
      position = p;
      type = t;
    }
  }

  public struct InternalPuzzle {
    public List<InternalCube> cubes;
    public int typicalRotationsNeeded;

    public InternalPuzzle(List<InternalCube> c, int trn) {
      cubes = c;
      typicalRotationsNeeded = trn;
    }
  }

  private static Dictionary<string, List<InternalPuzzle>> loadedPuzzles = new Dictionary<string, List<InternalPuzzle>>();

  public InternalPuzzle LoadPuzzle(int width, int height) {
    if (loadedPuzzles.Count == 0) {
      LoadAndParsePuzzles();
    }

    var key = string.Format("{0}x{1}", width, height);
    var puzzlesOfSize = loadedPuzzles[key];

    var randomIndex = (int)Random.Range(0, puzzlesOfSize.Count);
    // var randomIndex = 0; // NOTE: For testing a simple 4x2 puzzle

    var puzzle = puzzlesOfSize[randomIndex];

    return puzzle;
  }

  // TODO: Rewrite/use https://app.quicktype.io/ instead of hand-rolled loader/parser
  public void LoadAndParsePuzzles() {
    var jsonPuzzlesBySizes = (JObject)JObject.Parse(PersistentState.puzzlesJSONString);

    foreach (var jsonPuzzlesBySize in jsonPuzzlesBySizes) {
      var sizeKey = jsonPuzzlesBySize.Key;
      var jsonPuzzles = (JArray)jsonPuzzlesBySize.Value;

      foreach (JObject jsonPuzzle in jsonPuzzles) {
        List<InternalCube> cubes = null;

        int typicalRotationsNeeded = int.Parse((string)jsonPuzzle["typical_rotations_needed"]);
        int width = int.Parse((string)jsonPuzzle["width"]);
        int height = int.Parse((string)jsonPuzzle["height"]);

        var jsonPuzzleRows = (JArray)jsonPuzzle["rows"];

        float puzzleDepth = 0f;

        foreach (JObject puzzleRow in jsonPuzzleRows) {
          var jsonCubes = (JArray)puzzleRow["row"];

          float puzzlePosition = 0f;
          foreach (JObject jsonCube in jsonCubes) {
            var jsonCubeType = (string)jsonCube["type"];

            CubeType.Type type;

            if (jsonCubeType == "*") {
              type = CubeType.Type.Advantage;
            } else if (jsonCubeType == "x") {
              type = CubeType.Type.Forbidden;
            } else {
              type = CubeType.Type.Normal;
            }

            InternalCube cube = new InternalCube(new Vector3(puzzlePosition, 0f, -puzzleDepth), type);

            if (cubes == null) {
              cubes = new List<InternalCube>(width * height);
            }
            cubes.Add(cube);

            puzzlePosition += 1;
          }

          puzzleDepth += 1;
        }

        InternalPuzzle puzzle = new InternalPuzzle(cubes, typicalRotationsNeeded);

        if (!loadedPuzzles.ContainsKey(sizeKey)) {
          loadedPuzzles.Add(sizeKey, new List<InternalPuzzle>(jsonPuzzles.Count));
        }

        loadedPuzzles[sizeKey].Add(puzzle);
      }
    }
  }
}
