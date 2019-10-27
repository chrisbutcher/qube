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

  // Stage bonuses:
  // After each stage: For every row remaining on grid - 1,000 points
  //                   up to maximum of: 27,000 (1st Stage)
  //                                     39,000 (3rd Stage)
  //                                     40,000 (all other Stages)
  //                   TODO: Implement per-stage row bonus maximums

  // Perf:
  // * [ ] Maybe disable floor cube colliders until they are close to end of floor, able to drop and be able to be interacted with?

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
  int PlayerSquashedForNPuzzles = 0;

  const float PlayerStartingPosY = -0.5f;

  bool InPostPuzzlePause = false;

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

    boardManager = GetComponent<BoardManager>();
    cubeRotationMonitor = GetComponent<CubeRotationMonitor>();
    scoreboard = GameObject.FindGameObjectWithTag("UI").GetComponent<Scoreboard>();
    scoreboard.HideAnnounce();
  }

  void Start() {
    var player = (GameObject)Instantiate(PlayerPrefab, new Vector3(1.5f, PlayerStartingPosY, -7.5f), Quaternion.identity); // TODO: Do not hard code initial player position
    player.name = "Player";
    Players.Add(player);

    SetAndLoadStage(1); // TODO
  }

  void SetAndLoadStage(int stageIndex) {
    CurrentStageIndex = stageIndex;

    CurrentStage = stageDefinitions.Stages[CurrentStageIndex];
    CurrentWave = CurrentStage.Waves[CurrentWaveIndex];

    // NOTE: Floor depth = ((stage starting wave depth x stage puzzles per wave) * 2) + 3.
    // e.g. ((2 * 3) * 2) + 3 = 15 for stage 1
    //      ((5 * 3) * 2) + 3 = 33 for stage 2
    //      ((4 * 3) * 2) + 3 = 27 for stage 3 ...
    var stageInitialFloorDepth = ((CurrentWave.Depth * CurrentStage.PuzzlesPerWave) * 2) + 3;

    boardManager.LoadStage(CurrentWave.Width, stageInitialFloorDepth);
    // boardManager.LoadStage(CurrentWave.Width, 17);

    LoadWaveAndPerformSideEffects(CurrentStage.PuzzlesPerWave, CurrentWave.Width, CurrentWave.Depth);

    // TODO: Attempting to smoothly set camera behind player when loading stage. WIP.
    var playerRb = Players[0].GetComponent<Rigidbody>();
    var camera = GameObject.FindGameObjectWithTag("MainCamera");
    camera.transform.position = playerRb.transform.position + Vector3.back * 3;
  }

  public void RestartStageAfterGameOver(int playerIndex) {
    // TODO: Reset score, delete all current puzzles, reset all other relevant state.

    boardManager.RemoveAllPuzzles();
    boardManager.RemoveFloor();

    CurrentStageScore = 0;
    CurrentWaveIndex = 0;

    if (PlayerSquashed) {
      Players[0].GetComponent<PlayerSquashable>().UnSquashPlayer();
      PlayerSquashed = false;
    }

    CurrentPuzzlePlayerMadeMistakes = false;

    SetAndLoadStage(CurrentStageIndex);
    GetPlayerControls(playerIndex).Enable();
  }

  void LoadWaveAndPerformSideEffects(int numPuzzles, int width, int depth) {
    float playerDistanceBack = (numPuzzles * depth) + 1.5f;
    var playerPosition = new Vector3(1.5f, PlayerStartingPosY, -playerDistanceBack);

    var player = Players[0];
    player.GetComponent<RigidBodyPlayerMovement>().ResetLastFramePosition();

    var playerRb = player.GetComponent<Rigidbody>();
    playerRb.MovePosition(playerPosition);
    player.transform.position = playerPosition;

    boardManager.LoadWave(CurrentStage.PuzzlesPerWave, CurrentWave.Width, CurrentWave.Depth);
    CurrentWaveBlockScaleUsed = 0;

    StartCoroutine(ActivateNextPuzzleAfterDelay(GameConsts.PostWaveLoadPause, false));
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

    // When squashhed, even forbidden cubes that fall off the end count towards filling up your block scale.
    if (cubeType != CubeType.Type.Forbidden || PlayerSquashed) {
      CurrentPuzzlePlayerMadeMistakes = true;

      CurrentWaveBlockScaleUsed += 1;

      if (CurrentWaveBlockScaleUsed >= CurrentWaveBlockScaleAvailable + 1) {
        CurrentWaveBlockScaleUsed = 0;
        boardManager.floorManager.DropLast(); // TODO: Need to make this idempotent?
      }
    }

    PossiblyLoadNextWaveOrStage(fallenCube);
  }

  void HandlePlayerSquashed(GameObject player) {
    PlayerSquashed = true;
  }

  void PossiblyLoadNextWaveOrStage(GameObject fallenOrDestroyedCube) {
    var playerIsFalling = Players[0].GetComponent<PlayerFallable>().PlayerFalling;

    if (playerIsFalling) {
      return;
    }

    if (boardManager.HasActivePuzzle() == false) {
      StartCoroutine(PostPuzzleDelayPhase(GameConsts.PostPuzzleScoringPause));
    }
  }

  IEnumerator PostPuzzleDelayPhase(float delayLength) {
    InPostPuzzlePause = true;
    yield return new WaitForSeconds(delayLength);

    bool playerWasSquashed = PlayerSquashed;

    if (PlayerSquashed) {
      Players[0].GetComponent<PlayerSquashable>().UnSquashPlayer();
      PlayerSquashed = false;
      PlayerSquashedForNPuzzles += 1;
    } else {
      PlayerSquashedForNPuzzles = 0;
    }

    Players[0].GetComponent<AdvantageMarkers>().ClearAllAdvantageMarkers();

    // Score based on rotations used to solve puzzle, vs. TRN
    if (CurrentPuzzlePlayerMadeMistakes == false) { // TODO: Double check these bonuses are only applied if player made no mistakes
      var justCompletedPuzzle = boardManager.CurrentPuzzleOrNextPuzzleUp();
      if (justCompletedPuzzle.RotationsSinceFirstCubeDestroyed < justCompletedPuzzle.TypicalRotationNumber) {
        CurrentStageScore += 10000;
        scoreboard.ShowAnnounce("True Genius!!", GameConsts.PostPuzzleScoreTextDuration); // under TRN
      } else if (justCompletedPuzzle.RotationsSinceFirstCubeDestroyed == justCompletedPuzzle.TypicalRotationNumber) {
        CurrentStageScore += 5000;
        scoreboard.ShowAnnounce("Brilliant!!", GameConsts.PostPuzzleScoreTextDuration); // on TRN
      } else if (justCompletedPuzzle.RotationsSinceFirstCubeDestroyed > justCompletedPuzzle.TypicalRotationNumber) {
        CurrentStageScore += 1000;
        scoreboard.ShowAnnounce("Perfect", GameConsts.PostPuzzleScoreTextDuration); // above TRN
      }

      boardManager.floorManager.Add(CurrentWave.Width, true);
    }

    // If the current wave has an already loaded puzzle...
    if (boardManager.CurrentWavePuzzleCount() > 0) {
      // Players only get one replay of a puzzle on a given wave of puzzles if they get squashed while playing it.
      var playerSquashedOnceOnPuzzleAndAbleToRetry = playerWasSquashed && (PlayerSquashedForNPuzzles == 1);

      StartCoroutine(ActivateNextPuzzleAfterDelay(GameConsts.PostWaveLoadPause, playerSquashedOnceOnPuzzleAndAbleToRetry));
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

  IEnumerator ActivateNextPuzzleAfterDelay(float afterDelay, bool playerSquashedOnceOnPuzzleAndAbleToRetry) {
    yield return new WaitForSeconds(afterDelay);

    cubeRotationMonitor.CubeScoredThisPuzzle = false;
    CurrentPuzzlePlayerMadeMistakes = false;
    InPostPuzzlePause = false;

    // TODO: Sometimes call this with true, i.e. when player is forced to play the previous puzzle again.
    boardManager.ActivateNextPuzzle(playerSquashedOnceOnPuzzleAndAbleToRetry);
  }

  public void DisablePlayerControlsAndWalkAnimation() {
    var player = Players[0];

    // TODO: Also destroy all player markers when squashed?
    player.GetComponent<PlayerMarker>().CurrentPlayerMarker.SetActive(false);
    player.GetComponent<AdvantageMarkers>().ClearAllAdvantageMarkers();

    GameManager.instance.GetPlayerControls(0).Disable();

    var playerAnimator = player.GetComponentInChildren<Animator>();
    playerAnimator.SetFloat("Speed", 0f);
  }

  public PlayerControls GetPlayerControls(int playerIndex) {
    var player = Players[playerIndex];
    return player.GetComponent<PlayerControls>();
  }

  public string CurrentPuzzleStateQueueStatus() {
    return boardManager.CurrentPuzzleOrNextPuzzleUp().StateQueueStatus();
  }

  public bool CameraFollowingPlayer() {
    if (PlayerSquashed) {
      return !CubesSpedUp();
    }

    if (InPostPuzzlePause) {
      return false;
    }

    if (boardManager.CurrentPuzzleOrNextPuzzleUp().PuzzleContainsOnlyForbiddenCubes()) {
      return !CubesSpedUp();
    }

    return true;
  }

  public float TumbleSpeedMultiplier() {
    return CubesSpedUp() ? GameConsts.SpedUpTumbleMultiplier : 1f;
  }

  bool CubesSpedUp() {
    var playerFalling = Players[0].GetComponent<PlayerFallable>().PlayerFalling;

    return (GameManager.instance.GetPlayerControls(0).isSpeedingUpCubes() && playerFalling == false) || (PlayerSquashed && playerFalling == false);
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
