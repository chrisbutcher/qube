using System.Collections;
using UnityEngine;

public class Tumble : MonoBehaviour {
  public bool isMoving = false;
  public bool isFalling = false;

  public Vector3 PositionMovingTo;

  private Rigidbody rb;

  public static event SharedEvents.CubeFell OnCubeFell;
  public static event SharedEvents.PlayerSquashed OnPlayerSquashed;
  public static event SharedEvents.CubeStartedRotating OnCubeStartedRotating;
  public static event SharedEvents.CubeFinishedRotating OnCubeFinishedRotating;

  private float tumbleProgress;

  void Start() {
    PositionMovingTo = transform.position;

    rb = GetComponent<Rigidbody>();

    DisablePhysics();
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
    tumbleProgress = 0.0f;

    OnCubeStartedRotating(this.gameObject);

    while (tumbleProgress < duration) {
      var timeDelta = Time.deltaTime * GameManager.instance.TumbleSpeedMultiplier();
      tumbleProgress += timeDelta;
      transform.RotateAround(pivot, rotationAxis, rotationSpeed * timeDelta);

      yield return null;
    }

    OnCubeFinishedRotating(this.gameObject);

    transform.rotation = endRotation;
    transform.position = PositionMovingTo;

    isMoving = false;

    StartFallingIfOffEdge();
  }

  void OnCollisionEnter(Collision collision) {
    HandleCollisionEnterOrStay(collision);
  }

  void OnCollisionStay(Collision collision) {
    HandleCollisionEnterOrStay(collision);
  }

  void HandleCollisionEnterOrStay(Collision collision) {
    var collider = collision.collider;

    if (collider.tag == "Player" && tumbleProgress > GameConsts.TumbleDuration / 3f) {
      var player = collider.gameObject;

      var quantizedPlayerPosition = player.GetComponent<RigidBodyPlayerMovement>().QuantizedPlayerPosition;
      var quantizedCubePosition = Util.FloorVec3(PositionMovingTo);

      if (Util.Vec3sEqualXandZ(quantizedPlayerPosition, quantizedCubePosition)) {
        OnPlayerSquashed(player);
      }
    }
  }

  void StartFallingIfOffEdge() {
    var playerFalling = GameManager.instance.Players[0].GetComponent<PlayerFallable>().PlayerFalling;

    var playerFellFarAlready = GameManager.instance.Players[0].transform.position.y < -7f;
    var floorIsBelowTumblingCube = GameManager.instance.boardManager.floorManager.IsFloorBelowVec3(transform.position);

    // TODO
    // if (floorIsBelowTumblingCube == false && playerFalling == false) {
    // if (floorIsBelowTumblingCube == false && playerFellFarAlready == false) {
    if (floorIsBelowTumblingCube == false) {
      isFalling = true;

      EnablePhysics();

      GetComponent<Destroyable>().DestroyedByPlayerOrByFalling = true;
      OnCubeFell(this.gameObject);
    }
  }

  void EnablePhysics() {
    // TODO: Dry up this with FloorStack#Drop
    rb.detectCollisions = true;
    rb.useGravity = true;
    // rb.isKinematic = false; # ????
    rb.constraints = RigidbodyConstraints.None;

    var range = 3f;
    rb.AddRelativeTorque(Random.Range(-range, range), Random.Range(-range, range), Random.Range(-range, range));
  }

  void DisablePhysics() {
    // rb.detectCollisions = false;
    rb.useGravity = false;
    rb.isKinematic = true;
  }
}
