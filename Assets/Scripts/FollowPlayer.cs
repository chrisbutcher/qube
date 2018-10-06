using UnityEngine;

public class FollowPlayer : MonoBehaviour {
  // ref https://www.youtube.com/watch?v=lnguV1v38z4

  public float Smooth;
  public float DistanceAway;
  public float DistanceUp; // TODO This changes when solving, 4 when solving, 2 when not
  private Transform follow;
  private Vector3 targetPosition;
  public float CameraLookZOffset = 1f;
  public bool ShowDebugVectors = false;

  private FloorManager floorManager;

  void Awake() {
    // follow = GameObject.FindWithTag("Player").transform;
    // floorManager = GameState.State.FloorManager;
  }

  void Start() {
    follow = GameObject.FindWithTag("Player").transform;
    UpdateCameraPosition(1000f, 0f);
  }

  void LateUpdate() {
    var followPercentageAcrossFloor = 0f; //floorManager.SideToSidePositionOnFloor(follow);
    UpdateCameraPosition(Smooth, followPercentageAcrossFloor);
  }

  void UpdateCameraPosition(float smooth, float followPercentageAcrossFloor) {
    var turnVector = Vector3.forward + Vector3.right * followPercentageAcrossFloor;
    targetPosition = follow.position + Vector3.up * DistanceUp - turnVector * DistanceAway;

    if (ShowDebugVectors) {
      Debug.DrawRay(follow.position, Vector3.up * DistanceUp, Color.red);
      Debug.DrawRay(follow.position, -1f * turnVector * DistanceAway, Color.blue);
      Debug.DrawLine(follow.position, targetPosition, Color.magenta);
    }

    transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smooth);
    SmoothLookAt(smooth);
  }

  void SmoothLookAt(float smooth) {
    Vector3 lookOffset = new Vector3(0f, 0f, CameraLookZOffset);
    Vector3 relPlayerPosition = (follow.position + lookOffset) - transform.position;

    Quaternion lookAtRotation = Quaternion.LookRotation(relPlayerPosition, Vector3.up);
    transform.rotation = Quaternion.Lerp(transform.rotation, lookAtRotation, smooth * Time.deltaTime);
  }
}
