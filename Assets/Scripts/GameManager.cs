using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Dynamic;
using System.IO;

public class GameManager : MonoBehaviour {
  // TODO: Implement rest of gameplay! Reference:
  // https://gamefaqs.gamespot.com/ps/197636-intelligent-qube/faqs/40016
  // https://www.youtube.com/watch?v=BZM9kTGFeko&t=101s

  // GAMEPLAY TO IMPLEMENT:
  // * [ ] You will also receive a bonus at the end of each stage, based on how many rows you have left on the playing field.

  // BlOCK SCALE (bottom right)
  // * [ ] As the puzzles get larger, you can drop more Qubes over the edge before you start losing rows off the grid

  // GETTING SQUISHED
  // * [ ] If you do get squished, twom ajor things will happen.  Firstly, the puzzle will accelerate, denying you the chance to complete it.  Any remaining Qubes will be dropped off the edge, and
  // *   they will count towards your allowed dropped Qube total, as measured by the Block Scale.  If this allowed number is exceeded, you will lose rows off the grid accordingly. 
  // * [ ] When squished, even Forbidden Qubes will add to the Block Scale counter.
  // * [ ] Once squished, if there are any puzzles remaining in the current wave, you will be forced to do the same puzzle over again.This time, however, the over-the-block indicators that are present
  //   on the easiest play mode will be in effect for the duration of that puzzle, highlighting for you the positions of any marked or Advantage squares.If the puzzle in which you originally got squished
  //   was the last puzzle of the current wave, you will not have to repeat it(the new wave clears this effect).  Similarly, if you get squished again while repeating a puzzle, you will not have to do it a third time.

  // Stage bonuses:
  // After each stage: For every row remaining on grid - 1,000 points
  //                   up to maximum of: 27,000 (1st Stage)
  //                                     39,000 (3rd Stage)
  //                                     40,000 (all other Stages)
  //                   TODO: Implement per-stage row bonus maximums

  const string STAGE_DEFINITIONS_FILENAME = "stage_definitions.json";

  public static GameManager instance = null;
  public BoardManager boardManager;
  public CubeRotationMonitor cubeRotationMonitor;
  public Scoreboard scoreboard;

  public GameObject PlayerPrefab;
  public List<GameObject> Players;

  StageDefinitions stageDefinitions;
  public int CurrentStageIndex = 0;
  public Stage CurrentStage;
  public int CurrentWaveIndex = 0;
  public Wave CurrentWave;

  public int CurrentWaveBlockScaleUsed = 0; // TODO: This meter resets for each wave of puzzles
  public int CurrentWaveBlockScaleAvailable = 3; // TODO: Find out size of block scale per stage/wave.

  public bool CurrentPuzzlePlayerMadeMistakes = false; // Mistake == capturing a forbidden cube, or dropping a non-forbidden cube
  public int CurrentStageScore;

  public bool PlayerSquashed = false;

  const float PlayerStartingPosY = -0.5f;

  void OnEnable() {
    Destroyable.OnCubeScored += HandleCubeScored;
    Tumble.OnCubeFell += HandleCubeFell;
    Tumble.OnPlayerSquashed += HandlePlayerSquashed;
  }

  void OnDisable() {
    Destroyable.OnCubeScored -= HandleCubeScored;
    Tumble.OnCubeFell -= HandleCubeFell;
    Tumble.OnPlayerSquashed -= HandlePlayerSquashed;
  }

  void Awake() {
    SingletonSetup();

    LoadStageDefinitions();

    CurrentStage = stageDefinitions.Stages[CurrentStageIndex];
    CurrentWave = CurrentStage.Waves[CurrentWaveIndex];

    boardManager = GetComponent<BoardManager>();
    cubeRotationMonitor = GetComponent<CubeRotationMonitor>();
    scoreboard = GameObject.FindGameObjectWithTag("UI").GetComponent<Scoreboard>();
    scoreboard.HideAnnounce();
  }

  void Start() {
    boardManager.LoadStage(CurrentWave.Width, 15); // TODO: Determine dynamic floor depth as stages change

    var player = (GameObject)Instantiate(PlayerPrefab, new Vector3(1.5f, PlayerStartingPosY, -7.5f), Quaternion.identity); // TODO: Do not hard code initial player position
    player.name = "Player";

    Players.Add(player);

    LoadWaveAndPerformSideEffects(CurrentStage.PuzzlesPerWave, CurrentWave.Width, CurrentWave.Depth);
  }

  void LoadWaveAndPerformSideEffects(int numPuzzles, int width, int depth) {
    float playerDistanceBack = (numPuzzles * depth) + 1.5f;
    var playerPosition = new Vector3(1.5f, PlayerStartingPosY, -playerDistanceBack);

    var playerRb = Players[0].GetComponent<Rigidbody>();
    playerRb.MovePosition(playerPosition);

    boardManager.LoadWave(CurrentStage.PuzzlesPerWave, CurrentWave.Width, CurrentWave.Depth);
    CurrentWaveBlockScaleUsed = 0;

    StartCoroutine(ActivateNextPuzzleAfterDelay(GameConsts.PostWaveLoadPause));
  }

  void HandleCubeScored(GameObject scoredCube, MarkerType.Type scoredByMarkerType) {
    // Score captured non-forbidden cubes
    // TODO: Constantize
    if (scoredByMarkerType is MarkerType.Type.AdvantageMarker) {
      CurrentStageScore += 200;
    } else if (scoredByMarkerType is MarkerType.Type.PlayerMarker) {
      CurrentStageScore += 100;
    }

    PossiblyLoadNextWaveOrStage(scoredCube);
  }

  void HandleCubeFell(GameObject fallenCube) {
    var cubeType = fallenCube.GetComponent<CubeType>().CurrentType;

    if (cubeType != CubeType.Type.Forbidden) {
      CurrentPuzzlePlayerMadeMistakes = true;

      CurrentWaveBlockScaleUsed += 1;

      if (CurrentWaveBlockScaleUsed >= CurrentWaveBlockScaleAvailable) {
        boardManager.floorManager.DropLast();
      }
    }

    PossiblyLoadNextWaveOrStage(fallenCube);
  }

  void HandlePlayerSquashed(GameObject player) {
    PlayerSquashed = true;
  }

  void PossiblyLoadNextWaveOrStage(GameObject fallenOrDestroyedCube) {
    if (boardManager.HasActivePuzzle() == false) {

      Players[0].GetComponent<AdvantageMarkers>().ClearAllAdvantageMarkers();

      // Score based on rotations used to solve puzzle, vs. TRN
      // TODO: Constantize
      if (CurrentPuzzlePlayerMadeMistakes == false) { // TODO: Double check these bonuses are only applied if player made no mistakes
        var justCompletedPuzzle = boardManager.CurrentPuzzleOrNextPuzzleUp();
        if (justCompletedPuzzle.RotationsSinceFirstCubeDestroyed < justCompletedPuzzle.TypicalRotationNumber) {
          CurrentStageScore += 10000;
          scoreboard.ShowAnnounce("True Genius!!", 2f); // under TRN
        } else if (justCompletedPuzzle.RotationsSinceFirstCubeDestroyed == justCompletedPuzzle.TypicalRotationNumber) {
          CurrentStageScore += 5000;
          scoreboard.ShowAnnounce("Brilliant!!", 2f); // on TRN
        } else if (justCompletedPuzzle.RotationsSinceFirstCubeDestroyed > justCompletedPuzzle.TypicalRotationNumber) {
          CurrentStageScore += 1000;
          scoreboard.ShowAnnounce("Perfect", 2f); // above TRN
        }

        boardManager.floorManager.Add(CurrentWave.Width);
      }

      if (boardManager.CurrentWavePuzzleCount() > 0) { // If the current wave has an already loaded puzzle...
        StartCoroutine(ActivateNextPuzzleAfterDelay(GameConsts.PostWaveLoadPause));
      } else {
        if (CurrentWaveIndex + 1 <= CurrentStage.Waves.Count) {
          CurrentWaveIndex += 1;
          CurrentWave = CurrentStage.Waves[CurrentWaveIndex];
          LoadWaveAndPerformSideEffects(CurrentStage.PuzzlesPerWave, CurrentWave.Width, CurrentWave.Depth);
        } else {
          Debug.Log("END OF STAGE");
        }
      }
    }
  }

  IEnumerator ActivateNextPuzzleAfterDelay(float afterDelay) {
    yield return new WaitForSeconds(afterDelay);

    cubeRotationMonitor.CubeScoredThisPuzzle = false;
    CurrentPuzzlePlayerMadeMistakes = false;

    boardManager.ActivateNextPuzzle(); // activate it, to continue this wave
  }

  public float TumbleSpeedMultiplier() {
    return Input.GetKey(KeyCode.LeftShift) || PlayerSquashed ? 4f : 1f;
  }

  void LoadStageDefinitions() {
    string filePath = Path.Combine(Application.streamingAssetsPath, STAGE_DEFINITIONS_FILENAME);

    if (File.Exists(filePath)) {
      var jsonString = File.ReadAllText(filePath);
      stageDefinitions = StageDefinitions.FromJson(jsonString);
    }
  }

  void SingletonSetup() {
    if (instance == null) {
      instance = this;
    } else if (instance != this) {
      Destroy(gameObject);
    }
    DontDestroyOnLoad(gameObject);
  }
}
