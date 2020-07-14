﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Dynamic;
using System.IO;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour {
  // TODO: Implement rest of gameplay! Reference:
  // https://gamefaqs.gamespot.com/ps/197636-intelligent-qube/faqs/40016
  // https://www.youtube.com/watch?v=BZM9kTGFeko&t=101s

  // TODO Gameplay:
  // * [ ] Calculate and show IQ: http://blog.airesoft.co.uk/2015/08/how-to-be-a-genius-intelligent-qubes-iq-algorithm/

  // BUGS: 
  // * [ ] When a large number of rows fall off at once, the floor that should crumble sometimes hangs around longer than expected.

  // Perf:
  // * [ ] Maybe disable floor cube colliders until they are close to end of floor, able to drop and be able to be interacted with?

  public BoardManager boardManager;
  public CubeRotationMonitor cubeRotationMonitor;
  public Scoreboard scoreboard;

  GameObject stageTransition = null;

  public GameObject PlayerPrefab;
  public List<GameObject> Players;

  public StageDefinitions stageDefinitions = null;

  public int CurrentStageIndex = 0;
  public Stage CurrentStage;
  public int CurrentWaveIndex = 0;
  public Wave CurrentWave;

  public int CurrentWaveBlockScaleUsed = 0;
  public int CurrentWaveBlockScaleAvailable = 3;

  public bool CurrentPuzzlePlayerMadeMistakes = false; // Mistake == capturing a forbidden cube, or dropping a non-forbidden cube

  public int TotalScore;
  public int CurrentStageScore;

  public bool PlayerSquashed = false;
  public float PlayerSquashedFor;
  int PlayerSquashedForNPuzzles = 0;

  const float PlayerStartingPosY = -0.5f;

  bool InPostPuzzlePause = false;

  bool GameActive = false; // Used when pausing the world during stage transitions etc. Not the same as player pausing.
  public bool PlayerPaused = false; // Player pressed pause.

  public bool isGameActive() {
    return this.GameActive;
  }

  public void ActivateGame() {
    this.GameActive = true;
  }

  public void DeactivateGame() {
    this.GameActive = false;
  }

  public static GameManager GameManagerInstance() {
    return GameObject.FindGameObjectWithTag("Camera").GetComponent<GameManager>();
  }

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
    boardManager = GetComponent<BoardManager>();
  }

  void Start() {
    LoadStageDefinitions();

    cubeRotationMonitor = GetComponent<CubeRotationMonitor>();
    scoreboard = GameObject.FindGameObjectWithTag("UI").GetComponent<Scoreboard>();
    scoreboard.HideAnnounce();

    stageTransition = GameObject.FindGameObjectWithTag("StageTransition");

    var player = (GameObject)Instantiate(PlayerPrefab, new Vector3(1.5f, PlayerStartingPosY, -7.5f), Quaternion.identity); // TODO: Do not hard code initial player position
    player.name = "Player";
    Players.Add(player);

    SetAndLoadStage(PersistentState.SelectedStage);

    ActivateGame();

    stageTransition.GetComponentInChildren<Canvas>().GetComponent<Animator>().SetTrigger("FadeIn");
  }

  void Update() {
    if (GameManager.GameManagerInstance().GetPlayerControls(0).isPausing()) {
      if (PlayerPaused) {
        PlayerPaused = false;
        ActivateGame();

        GameObject.FindGameObjectWithTag("PauseMenu").GetComponent<Canvas>().enabled = false;
      } else {
        PlayerPaused = true;
        DeactivateGame();

        GameObject.FindGameObjectWithTag("PauseMenu").GetComponent<Canvas>().enabled = true;
      }
    }

    if (!GameManager.GameManagerInstance().isGameActive()) {
      return;
    }

    if (PlayerSquashed) {
      PlayerSquashedFor += Time.deltaTime;
    } else {
      PlayerSquashedFor = 0f;
    }
  }

  void SetAndLoadStage(int stageIndex) {
    CurrentStageIndex = stageIndex;

    CurrentStage = stageDefinitions.Stages[CurrentStageIndex];
    CurrentWaveBlockScaleAvailable = CurrentStage.BlockScaleSize;

    CurrentWave = CurrentStage.Waves[CurrentWaveIndex];

    // NOTE: Floor depth = ((stage starting wave depth x stage puzzles per wave) * 2) + 3.
    // e.g. ((2 * 3) * 2) + 3 = 15 for stage 1
    //      ((5 * 3) * 2) + 3 = 33 for stage 2
    //      ((4 * 3) * 2) + 3 = 27 for stage 3 ...
    var stageInitialFloorDepth = ((CurrentWave.Depth * CurrentStage.PuzzlesPerWave) * 2) + 3;

    boardManager.LoadStage(CurrentWave.Width, stageInitialFloorDepth);

    LoadWaveAndPerformSideEffects(CurrentStage.PuzzlesPerWave, CurrentWave.Width, CurrentWave.Depth, GameConsts.StageLoadPuzzleActivationPause);

    // TODO: Attempting to smoothly set camera behind player when loading stage. WIP.
    var playerRb = Players[0].GetComponent<Rigidbody>();
    var camera = GameObject.FindGameObjectWithTag("Camera");

    playerRb.rotation = Quaternion.Euler(0f, 0f, 0f);
    camera.transform.position = playerRb.transform.position + Vector3.back * 3;
  }

  public SoundManager GetSoundManager() {
    return GameObject.FindGameObjectWithTag("Camera").GetComponent<SoundManager>();
  }

  public void RestartStageAfterGameOver(int playerIndex) {
    var endOfStageText = string.Format("Game over. Restarting stage.");
    scoreboard.ShowAnnounce(endOfStageText, 3f);

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

  void LoadWaveAndPerformSideEffects(int numPuzzles, int width, int depth, float puzzleActivationDelay) {
    float playerDistanceBack = (numPuzzles * depth) + 2.5f;
    var playerPosition = new Vector3(1.5f, PlayerStartingPosY, -playerDistanceBack);

    var player = Players[0];
    player.GetComponent<RigidBodyPlayerMovement>().ResetLastFramePosition();

    var playerRb = player.GetComponent<Rigidbody>();
    playerRb.MovePosition(playerPosition);
    player.transform.position = playerPosition;

    boardManager.LoadWave(CurrentStage.PuzzlesPerWave, CurrentWave.Width, CurrentWave.Depth);
    CurrentWaveBlockScaleUsed = 0;

    StartCoroutine(ActivateNextPuzzleAfterDelay(puzzleActivationDelay, false));
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
    if (!isGameActive()) {
      return;
    }

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
        scoreboard.ShowAnnounce("True Genius!! 10,000 bonus points", GameConsts.PostPuzzleScoreTextDuration); // under TRN
        GameManager.GameManagerInstance().GetSoundManager().PlayGreatScore();
      } else if (justCompletedPuzzle.RotationsSinceFirstCubeDestroyed == justCompletedPuzzle.TypicalRotationNumber) {
        CurrentStageScore += 5000;
        scoreboard.ShowAnnounce("Brilliant!! 5,000 bonus points", GameConsts.PostPuzzleScoreTextDuration); // on TRN
        GameManager.GameManagerInstance().GetSoundManager().PlayGreatScore();
      } else if (justCompletedPuzzle.RotationsSinceFirstCubeDestroyed > justCompletedPuzzle.TypicalRotationNumber) {
        CurrentStageScore += 1000;
        scoreboard.ShowAnnounce("Perfect! 1,000 bonus points", GameConsts.PostPuzzleScoreTextDuration); // above TRN
        GameManager.GameManagerInstance().GetSoundManager().PlayGreatScore();
      }

      boardManager.floorManager.Add(CurrentWave.Width, true);
    }

    // If the current wave has an already loaded puzzle...
    if (boardManager.CurrentWavePuzzleCount() > 0) {
      // Players only get one replay of a puzzle on a given wave of puzzles if they get squashed while playing it.
      var playerSquashedOnceOnPuzzleAndAbleToRetry = playerWasSquashed && (PlayerSquashedForNPuzzles == 1);

      StartCoroutine(ActivateNextPuzzleAfterDelay(GameConsts.PostWaveLoadPause, playerSquashedOnceOnPuzzleAndAbleToRetry));
    } else {
      if (CurrentWaveIndex + 1 < CurrentStage.Waves.Count) {
        CurrentWaveIndex += 1;
        CurrentWave = CurrentStage.Waves[CurrentWaveIndex];
        LoadWaveAndPerformSideEffects(CurrentStage.PuzzlesPerWave, CurrentWave.Width, CurrentWave.Depth, GameConsts.PostWaveLoadPause);
      } else {
        yield return new WaitForSeconds(3f); // Give extra time for the gained floor row to be added to the world.

        int bonusScore = Mathf.Min(boardManager.floorManager.FloorRowCount() * 1000, CurrentStage.MaxEndStageBonus);

        TotalScore += CurrentStageScore;
        TotalScore += bonusScore;

        var endOfStageText = string.Format("Stage score: {0}\nStage bonus: {1}\nTotal game score: {2}", CurrentStageScore, bonusScore, TotalScore);
        scoreboard.ShowAnnounce(endOfStageText, 5f);
        yield return new WaitForSeconds(5f);

        var player = Players[0];
        player.GetComponent<PlayerMarker>().CurrentPlayerMarker.SetActive(false);
        DeactivateGame();
        StartCoroutine(EndOfStageSummary(GameConsts.PostPuzzleScoreTextDuration));
      }
    }
  }

  IEnumerator EndOfStageSummary(float delay) {
    yield return new WaitForSeconds(delay);

    stageTransition.GetComponentInChildren<Canvas>().GetComponent<Animator>().SetTrigger("FadeOut");

    StartCoroutine(FadeInNextStage(1.2f, 3f, 1f));
  }

  IEnumerator FadeInNextStage(float delayBeforeLoadingNextStage, float delayedBeforeFadingInAfterLoad, float delayBeforeActivatingGameAfterFadeIn) {
    yield return new WaitForSeconds(delayBeforeLoadingNextStage);

    if (CurrentStageIndex < stageDefinitions.Stages.Count - 1) {
      SetAndLoadStage(CurrentStageIndex + 1);
      yield return new WaitForSeconds(delayedBeforeFadingInAfterLoad);

      CurrentStageScore = 0;

      stageTransition.GetComponentInChildren<Canvas>().GetComponent<Animator>().SetTrigger("FadeIn");
      ActivateGame();
    } else {
      scoreboard.ShowAnnounce("GAME OVER. ", 10f);
      yield return new WaitForSeconds(10f);

      TotalScore = 0;

      SetAndLoadStage(0);
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

    GameManager.GameManagerInstance().GetPlayerControls(0).Disable();

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
    if (PlayerSquashedFor > 2f) {
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

    return (GameManager.GameManagerInstance().GetPlayerControls(0).isSpeedingUpCubes() && playerFalling == false) || (PlayerSquashed && playerFalling == false);
  }

  void LoadStageDefinitions() {
    this.stageDefinitions = PersistentState.stageDefinitions;
  }
}
