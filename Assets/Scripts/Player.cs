using System;
using UnityEngine;

public class Player : MonoBehaviour {

    public class SelectedCounter {
        public ClearCounter ClearCounter;
    }
    
    public static Player Instance { get; private set; }
    
    public event Action<SelectedCounter> OnSelectedCounterChanged;
    
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;

    private bool _isWalking;
    private Vector3 _lastInteractDirection;
    private ClearCounter _selectedCounter;

    private void Awake() {
        if (Instance != null) {
            Debug.Log("There is more than one player instance");
            Destroy(gameObject);
        }
        Instance = this;
    }

    private void OnEnable() {
        gameInput.OnInteractAction += GameInputOnInteractAction;
    }

    private void OnDisable() {
        gameInput.OnInteractAction -= GameInputOnInteractAction;
    }

    private void Update() {
        HandleMovement();
        HandleInteraction();
    }

    private void HandleMovement() {
        var inputVector = gameInput.GetMovementNormalizedVector();
        var moveDirection= new Vector3(inputVector.x, 0f, inputVector.y);
        var moveDistance = moveSpeed * Time.deltaTime;
        
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

    private void HandleInteraction() {
        var inputVector = gameInput.GetMovementNormalizedVector();
        var  moveDirection= new Vector3(inputVector.x, 0f, inputVector.y);
        float interactionDistance = 2f;

        if (moveDirection != Vector3.zero) {
            _lastInteractDirection = moveDirection;
        }

        if (Physics.Raycast(transform.position, _lastInteractDirection, out RaycastHit raycastHit, interactionDistance, countersLayerMask)) {
            if (raycastHit.transform.TryGetComponent<ClearCounter>(out var clearCounter)) {
                SetSelectedCounter(clearCounter);
            } else {
                SetSelectedCounter(null);         
            }
        } else {
            SetSelectedCounter(null);
        }
    }

    private void SetSelectedCounter(ClearCounter selectedCounter) {
        if (_selectedCounter != selectedCounter) {
            _selectedCounter = selectedCounter;
            OnSelectedCounterChanged?.Invoke(new SelectedCounter() {
                ClearCounter = selectedCounter
            });
        }
    }
    
    private void GameInputOnInteractAction() {
        if (_selectedCounter != null) _selectedCounter.Interact();
    }
    
    public bool IsWalking() {
        return _isWalking;
    }
}