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
  // * [ ] When you complete a puzzle perfectly (i.e. no missed Normal or Advantage Qubes, no captured Forbidden Qubes), you get an extra row added to the end of the grid.
  //   * To successfully complete a puzzle, you must capture all Normal and Advantage Qubes before they fall off the end of the grid, while leaving all Forbidden Qubes intact.  If you accomplish this, you will receive a 'Perfect' and an
  //     extra row will be added to the end of the grid.
  // * [ ] When you fail, you risk shrinking your play area.
  //   * [x] You will lose one row off of the grid for each Forbidden Qube that you capture...
  //   * [ ] and for every Normal or Advantage Qube that drops off the end of the grid above a certain allowable number within each wave.
  // * [ ] You will receive points for each captured block, and if you do well you will receive a bonus at the completion of each puzzle, based on how many turns it took you to complete it (see scoring, below).
  // * [ ] You will also receive a bonus at the end of each stage, based on how many rows you have left on the playing field.

  // FORBIDDEN CUBES, CONT'D
  // * [ ] If you do happen score a Forbidden cube, you will hear a horrible crash, you will lose a row off the end of the grid, and the puzzle will not be completed perfectly.

  // ADVANTAGE MARKERS
  // * [ ] DOUBLE-CHECK You can have more than one active captured Advantage square at one time, and when you bomb the grid, all Advantage Zones will be activated at once, creating massive capturing areas.  You can also chain together Advantage-style captures,
  //   if the triggered Advantage Zone contains one of more new Advantage Qubes. Trigger the new zones either right away or at a later rotation, as the situation may call for.
  // * [ ] Within an Advantage Zone, if you mark a square before bombing, that square will be protected from the triggered Advantage Zone.

  // BlOCK SCALE (bottom right)
  // * [ ] Each wave allows you to drop a certain number of Qubes off the end without penalty, as indicated by a string of rectangles on the bottom right corner of the screen, called the Block Scale.
  // * [ ] As Qubes drop, the Block Scale  will fill up.  Once it is completely full, you will lose one row off the grid  for the next Qube that goes over the edge, after which the Block Scale starts
  //   again empty.  This meter resets for each wave of puzzles.  As the puzzles get larger, you can drop more Qubes over the edge before you start losing rows off the grid

  // GETTING SQUISHED
  // * [ ] If you do get squished, twom ajor things will happen.  Firstly, the puzzle will accelerate, denying you the chance to complete it.  Any remaining Qubes will be dropped off the edge, and
  // *   they will count towards your allowed dropped Qube total, as measured by the Block Scale.  If this allowed number is exceeded, you will lose rows off the grid accordingly.  Please note that
  // *   when squished, even Forbidden Qubes will add to the Block Scale counter.
  // * [ ] Oncc squished, if there are any puzzles remaining in the current wave, you will be forced to do the same puzzle over again.This time, however, the over-the-block indicators that are present
  //   on the easiest play mode will be in effect for the duration of that puzzle, highlighting for you the positions of any marked or Advantage squares.If the puzzle in which you originally got squished
  //   was the last puzzle of the current wave, you will not have to repeat it(the new wave clears this effect).  Similarly, if you get squished again while repeating a puzzle, you will not have to do it a third time.

  // TYPICAL ROTATION NUMBER / FRACTION
  // * [ ] Node puzzles have `typical_rotations_needed` defined on them already
  // * [ ] The Typical Rotation Number, or TRN, is the number of turns that the game expects you to complete each puzzle in.  It is analogous to 'par' in golf.
  //   If you complete a puzzle in more turns than the TRN, which is displayed in the upper right corner of the screen, you will receive a lower end-of-puzzle bonus than if you complete it in exactly the TRN, which in turn
  //   is less than the bonus you receive for completing it in LESS than the TRN.
  // * [ ] The rotation or turn counter starts once you capture your first Qube, with that turn being rotation #1
  // * [ ] Each rotation of the Qubes adds 1 to the counter, which will change in color from blue when the current number of rotations is below the TRN, to white when it matches the TRN, to red when it exceeds the TRN.

  // Within each puzzle: Capture Qube with marked square - 100 points
  //                     Capture Qube with Advantage Zone - 200 points

  // After each puzzle: Completed puzzle over TRN - 1,000 points     "Perfect"
  //                    Completed puzzle on TRN - 5,000 points       "Brilliant"
  //                    Completed puzzle under TRN - 10,000 points   "True Genius!!"

  // After each stage: For every row remaining on grid - 1,000 points
  //                   up to maximum of: 27,000 (1st Stage)
  //                                     39,000 (3rd Stage)
  //                                     40,000 (all other Stages)
  //              TODO: Implement per-stage row bonus maximums

  const string STAGE_DEFINITIONS_FILENAME = "stage_definitions.json";

  public static GameManager instance = null;
  public BoardManager boardManager;

  public GameObject PlayerPrefab;
  public List<GameObject> Players;

  StageDefinitions stageDefinitions;
  public int CurrentStageIndex = 0;
  public Stage CurrentStage;
  public int CurrentWaveIndex = 0;
  public Wave CurrentWave;

  void OnEnable() {
    Destroyable.OnCubeScored += HandleCubeScored;
    Tumble.OnCubeFell += HandleCubeFell;
  }

  void OnDisable() {
    Destroyable.OnCubeScored -= HandleCubeScored;
    Tumble.OnCubeFell -= HandleCubeFell;
  }

  void Awake() {
    SingletonSetup();

    LoadStageDefinitions();

    CurrentStage = stageDefinitions.Stages[CurrentStageIndex];
    CurrentWave = CurrentStage.Waves[CurrentWaveIndex];

    boardManager = GetComponent<BoardManager>();
  }

  void Start() {
    boardManager.LoadStage(CurrentWave.Width, 15); // TODO: Determine dynamic floor depth as stages change
    boardManager.LoadWave(CurrentStage.PuzzlesPerWave, CurrentWave.Width, CurrentWave.Depth);
    StartCoroutine(ActivateNextPuzzleAfterDelay(GameConsts.PostWaveLoadPause));

    var player = (GameObject)Instantiate(PlayerPrefab, new Vector3(1.5f, 0f, -7.5f), Quaternion.identity); // TODO: Do not hard code initial player position
    Players.Add(player);
  }

  void HandleCubeScored(GameObject scoredCube) {
    PossiblyLoadNextWaveOrStage(scoredCube);
  }

  void HandleCubeFell(GameObject fallenCube) {
    PossiblyLoadNextWaveOrStage(fallenCube);
  }

  void PossiblyLoadNextWaveOrStage(GameObject fallenOrDestroyedCube) {
    if (boardManager.HasActivePuzzle() == false) {

      if (boardManager.CurrentWavePuzzleCount() > 0) { // If the current wave has an already loaded puzzle...
        StartCoroutine(ActivateNextPuzzleAfterDelay(GameConsts.PostWaveLoadPause));
      } else {
        // TODO: Load next wave, unless end of stage
        Debug.Log("END OF WAVE");

        if (CurrentWaveIndex + 1 <= CurrentStage.Waves.Count) {
          CurrentWaveIndex += 1;
          CurrentWave = CurrentStage.Waves[CurrentWaveIndex];
          boardManager.LoadWave(CurrentStage.PuzzlesPerWave, CurrentWave.Width, CurrentWave.Depth);
          StartCoroutine(ActivateNextPuzzleAfterDelay(GameConsts.PostWaveLoadPause));
        } else {
          Debug.Log("END OF STAGE");
        }
      }
    }
  }

  IEnumerator ActivateNextPuzzleAfterDelay(float afterDelay) {
    yield return new WaitForSeconds(afterDelay);

    boardManager.ActivateNextPuzzle(); // activate it, to continue this wave
  }

  public float TumbleSpeedMultiplier() {
    return Input.GetKey(KeyCode.LeftShift) ? 4f : 1f;
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
