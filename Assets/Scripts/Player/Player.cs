using System;
using Counters;
using KitchenObjects;
using UnityEngine;

namespace Player {
    public class Player : MonoBehaviour, IKitchenObjectParent {

        public class SelectedCounter {
            public BaseCounter BaseCounter;
        }
    
        public static Player Instance { get; private set; }
        
        public event Action<SelectedCounter> OnSelectedCounterChanged;
        public event EventHandler PickedSomething;
    
        [SerializeField] private float moveSpeed = 7f;
        [SerializeField] private GameInput gameInput;
        [SerializeField] private LayerMask countersLayerMask;
        [SerializeField] private Transform kitchenObjectParentPoint;

        private bool _isWalking;
        private Vector3 _lastInteractDirection;
        private KitchenObject _kitchenObject;
        private BaseCounter _selectedCounter;

        private void Awake() {
            if (Instance != null) {
                Debug.Log("There is more than one player instance");
                Destroy(gameObject);
            }
            Instance = this;
        }

        private void OnEnable() {
            gameInput.OnInteractAction += GameInputOnInteractAction;
            gameInput.OnInteractAlternateAction += GameInputOnInteractAlternateAction;
        }

        private void OnDisable() {
            gameInput.OnInteractAction -= GameInputOnInteractAction;
            gameInput.OnInteractAlternateAction -= GameInputOnInteractAlternateAction;
        }

        private void Update() {
            HandleMovement();
            HandleSelectCounter();
        }

        private void HandleMovement() {
            var inputVector = gameInput.GetMovementNormalizedVector();
            var moveDirection= new Vector3(inputVector.x, 0f, inputVector.y);
            var moveDistance = moveSpeed * Time.deltaTime;
        
            float playerRadius = .65f;
            float moveDeadZone = .4f;
            //DebugExtension.DebugCapsule(transform.position + (transform.forward * moveDistance), (transform.position + (Vector3.up * 2)) + (transform.forward * moveDistance), Color.magenta, playerRadius);
            bool collided = Physics.CapsuleCast(transform.position, transform.position + (Vector3.up * 2), playerRadius, moveDirection, moveDistance);
            if (collided) {
                //Try move X and Z independently
                var moveX = new Vector3(moveDirection.x, 0, 0).normalized;
                var moveZ = new Vector3(0, 0, moveDirection.z).normalized;
                if (Mathf.Abs(moveDirection.x) >= moveDeadZone && !Physics.CapsuleCast(transform.position, transform.position + (Vector3.up * 2), playerRadius, moveX, moveDistance)) {
                    moveDirection = moveX;
                    collided = false;
                } else if (Mathf.Abs(moveDirection.z) >= moveDeadZone && !Physics.CapsuleCast(transform.position, transform.position + (Vector3.up * 2), playerRadius, moveZ, moveDistance)) {
                    moveDirection = moveZ;
                    collided = false;
                }
            }
            if (!collided) transform.position += moveDirection.normalized * moveDistance;
            _isWalking = !collided && moveDirection != Vector3.zero;
        
            float rotateSpeed = 10f;
            transform.forward = Vector3.Slerp(transform.forward, moveDirection,Time.deltaTime * rotateSpeed);
        }

        private void HandleSelectCounter() {
            var inputVector = gameInput.GetMovementNormalizedVector();
            var  moveDirection= new Vector3(inputVector.x, 0f, inputVector.y);
            float interactionDistance = 2f;

            if (moveDirection != Vector3.zero) {
                _lastInteractDirection = moveDirection;
            }

            if (Physics.Raycast(transform.position, _lastInteractDirection, out RaycastHit raycastHit, interactionDistance, countersLayerMask)) {
                if (raycastHit.transform.TryGetComponent<BaseCounter>(out var counter)) {
                    SetSelectedCounter(counter);
                } else {
                    SetSelectedCounter(null);         
                }
            } else {
                SetSelectedCounter(null);
            }
        }

        private void SetSelectedCounter(BaseCounter selectedCounter) {
            if (_selectedCounter != selectedCounter) {
                _selectedCounter = selectedCounter;
                OnSelectedCounterChanged?.Invoke(new SelectedCounter() {
                    BaseCounter = selectedCounter
                });
            }
        }
    
        private void GameInputOnInteractAction() {
            if (!GameManager.Instance.IsGamePlaying()) return;
            if (_selectedCounter != null) _selectedCounter.Interact(this);
        }
    
        private void GameInputOnInteractAlternateAction() {
            if (!GameManager.Instance.IsGamePlaying()) return;
            if (_selectedCounter != null) _selectedCounter.InteractAlternate(this);
        }
    
        public bool IsWalking() {
            return _isWalking;
        }

        public Transform GetKitchenObjectParentPoint() {
            return kitchenObjectParentPoint;
        }

        public KitchenObject GetKitchenObject() {
            return _kitchenObject;
        }

        public void SetKitchenObject(KitchenObject kitchenObject) {
            _kitchenObject = kitchenObject;
            if (kitchenObject) {
                PickedSomething?.Invoke(this, EventArgs.Empty);
            }
        }

        public void ClearKitchenObject() {
            _kitchenObject = null;
        }

        public bool HasKitchenObject() {
            return _kitchenObject != null;
        }
    }
}