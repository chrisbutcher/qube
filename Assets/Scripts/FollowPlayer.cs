using UnityEngine;

public class FollowPlayer : MonoBehaviour { // TODO: Rename this script since it doesn't only follow the player, it also zooms out to see the board
  // ref https://www.youtube.com/watch?v=lnguV1v38z4

  public float DistanceAway;
  public float DistanceUp; // TODO This changes when solving, 4 when solving, 2 when not
  public float CameraLookZOffset;

  public float Smooth;

  GameObject playerToFollow;
  Vector3 follow;
  Vector3 targetPosition;
  public bool ShowDebugVectors = false;

  GameManager gameManager;
  FloorManager floorManager;

  // This ensures that we don't keep updating the corner cube to follow when in bird's eye mode,
  // so when floor cubes get added or drop off, we don't follow them.
  bool lastFrameCameraWasFollowingPlayer;

  void Awake() {
    gameManager = GameManager.instance;
    floorManager = gameManager.boardManager.floorManager;
  }

  void Start() {
    playerToFollow = GameObject.FindWithTag("Player");
    follow = playerToFollow.transform.position;
    SetCameraDistanceAndLookZOffset();
    UpdateCameraPosition(1000f, 0f);
  }

  void SetCameraDistanceAndLookZOffset() {
    if (gameManager.CameraFollowingPlayer()) {
      DistanceAway = GameConsts.CameraFollowPlayuerDistanceAway;
      DistanceUp = GameConsts.CameraFollowPlayuerDistanceUp; // TODO This changes when solving, 4 when solving, 2 when not
      CameraLookZOffset = GameConsts.CameraFollowPlayuerCameraLookZOffset;
    } else {
      // TODO: Constantize
      DistanceAway = 4.56f;
      DistanceUp = 4.6f;
      CameraLookZOffset = 1.38f;
    }
  }

  void Update() {
    SetCameraDistanceAndLookZOffset();

    if (gameManager.CameraFollowingPlayer()) {
      follow = GameObject.FindWithTag("Player").transform.position;
      lastFrameCameraWasFollowingPlayer = true;
    } else {
      var floorManager = GameManager.instance.boardManager.floorManager;

      if (lastFrameCameraWasFollowingPlayer) {
        follow = floorManager.GetNearestRightMostFloorCube().transform.position;
      }

      lastFrameCameraWasFollowingPlayer = false;
    }
  }

  void LateUpdate() {
    if (gameManager.CameraFollowingPlayer()) {
      follow = playerToFollow.transform.position;
    }
    var followPercentageAcrossFloor = floorManager.SideToSidePositionOnFloor(follow);
    UpdateCameraPosition(Smooth, followPercentageAcrossFloor);
  }

  void UpdateCameraPosition(float smooth, float followPercentageAcrossFloor) {
    var turnVector = Vector3.forward + Vector3.right * (followPercentageAcrossFloor / 2);
    targetPosition = follow + Vector3.up * DistanceUp - turnVector * DistanceAway;

    if (ShowDebugVectors) {
      Debug.DrawRay(follow, Vector3.up * DistanceUp, Color.red);
      Debug.DrawRay(follow, -1f * turnVector * DistanceAway, Color.blue);
      Debug.DrawLine(follow, targetPosition, Color.magenta);
    }

    transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smooth);
    SmoothLookAt(smooth);
  }

  void SmoothLookAt(float smooth) {
    Vector3 lookOffset = new Vector3(0f, 0f, CameraLookZOffset);
    Vector3 relPlayerPosition = (follow + lookOffset) - transform.position;

    Quaternion lookAtRotation = Quaternion.LookRotation(relPlayerPosition, Vector3.up);
    transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, smooth * Time.deltaTime);
  }
}
