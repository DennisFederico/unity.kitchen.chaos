using UnityEngine;

public class Player : MonoBehaviour {

    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;

    private bool _isWalking;
    private void Update() {
        PerformMovement();
    }

    private void PerformMovement() {
        var inputVector = gameInput.GetMovementNormalizedVector();
        var moveDistance = moveSpeed * Time.deltaTime;
        var  moveDirection= new Vector3(inputVector.x, 0f, inputVector.y);
        
        float playerRadius = .65f;
        //DebugExtension.DebugCapsule(transform.position + (transform.forward * moveDistance), (transform.position + (Vector3.up * 2)) + (transform.forward * moveDistance), Color.magenta, playerRadius);
        bool collided = Physics.CapsuleCast(transform.position, transform.position + (Vector3.up * 2), playerRadius, moveDirection, moveDistance);
        if (collided) {
            //Try move X and Z independently
            var moveX = new Vector3(moveDirection.x, 0, 0).normalized;
            var moveZ = new Vector3(0, 0, moveDirection.z).normalized;
            if (!Physics.CapsuleCast(transform.position, transform.position + (Vector3.up * 2), playerRadius, moveX, moveDistance)) {
                moveDirection = moveX;
                collided = false;
            } else if (!Physics.CapsuleCast(transform.position, transform.position + (Vector3.up * 2), playerRadius, moveZ, moveDistance)) {
                moveDirection = moveZ;
                collided = false;
            }
        }
        if (!collided) transform.position += moveDirection.normalized * moveDistance;
        _isWalking = !collided && moveDirection != Vector3.zero;
        
        float rotateSpeed = 10f;
        transform.forward = Vector3.Slerp(transform.forward, moveDirection,Time.deltaTime * rotateSpeed);
    }

    public bool IsWalking() {
        return _isWalking;
    }
    
}