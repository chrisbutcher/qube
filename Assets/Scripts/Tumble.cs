using System.Collections;
using UnityEngine;

public class Tumble : MonoBehaviour {
  public bool isMoving = false;
  public bool isFalling = false;

  public Vector3 PositionMovingTo;

  private Rigidbody rb;

  public static event SharedEvents.CubeFell OnCubeFell;

  void Start() {
    PositionMovingTo = transform.position;

    rb = GetComponent<Rigidbody>();

    DisablePhysics();
  }

  void Update() {
    if (!isFalling && !isMoving) {
      HandleInput();
    }
  }

  void HandleInput() {
    var direction = Vector3.zero;

    if (Input.GetKeyUp(KeyCode.W))
      direction = Vector3.forward;

    if (Input.GetKeyUp(KeyCode.S))
      direction = Vector3.back;

    if (Input.GetKeyUp(KeyCode.A))
      direction = Vector3.left;

    if (Input.GetKeyUp(KeyCode.D))
      direction = Vector3.right;

    TumbleInDirection(direction, GameConsts.TumbleDuration);
  }

  public void TumbleInDirection(Vector3 direction, float duration) {
    if (direction != Vector3.zero && !isMoving && gameObject.activeInHierarchy) {
      StartCoroutine(DoTumble(direction, duration));
    }
  }

  IEnumerator DoTumble(Vector3 direction, float duration) {
    isMoving = true;

    var rotationAxis = Vector3.Cross(Vector3.up, direction);
    var pivot = (transform.position + Vector3.down * (GameConsts.CubeSize / 2)) + direction * (GameConsts.CubeSize / 2);

    var startRotation = transform.rotation;
    var endRotation = Quaternion.AngleAxis(90.0f, rotationAxis) * startRotation;

    var startPosition = transform.position;
    PositionMovingTo = transform.position + direction;

    var rotationSpeed = 90.0f / duration;
    var progress = 0.0f;

    while (progress < duration) {
      var timeDelta = Time.deltaTime * GameManager.instance.TumbleSpeedMultiplier();
      progress += timeDelta;
      transform.RotateAround(pivot, rotationAxis, rotationSpeed * timeDelta);
      yield return null;
    }

    transform.rotation = endRotation;
    transform.position = PositionMovingTo;

    isMoving = false;

    StartFallingIfOffEdge();
  }

  void StartFallingIfOffEdge() {
    if (!GameManager.instance.boardManager.floorManager.IsFloorBelowVec3(transform.position)) {
      isFalling = true;

      EnablePhysics();
      OnCubeFell(this.gameObject);
    }
  }

  void EnablePhysics() {
    rb.detectCollisions = true;
    rb.useGravity = true;
    rb.isKinematic = false;
  }

  void DisablePhysics() {
    // rb.detectCollisions = false;
    rb.useGravity = false;
    rb.isKinematic = true;
  }
}
