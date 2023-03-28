using System;
using System.Collections.Generic;
using Counters;
using KitchenObjects;
using Unity.Netcode;
using UnityEngine;

namespace Player {
    public class Player : NetworkBehaviour, IKitchenObjectParent {

        public class SelectedCounter {
            public BaseCounter baseCounter;
        }

        //TODO confirm if this should really be static
        public static EventHandler localPlayerSpawned;
        public static EventHandler anyonePickedSomething;
        public static void ResetStaticEventHandler() {
            localPlayerSpawned = null;
            anyonePickedSomething = null;
        }
    
        public static Player LocalInstance { get; private set; }
        
        public event Action<SelectedCounter> OnSelectedCounterChanged;
        public event EventHandler PickedSomething;
    
        [SerializeField] private float moveSpeed = 7f;
        [SerializeField] private LayerMask countersLayerMask;
        [SerializeField] private LayerMask collisionLayerMask;
        [SerializeField] private Transform kitchenObjectParentPoint;
        [SerializeField] private List<Vector3> spawnPositions;
        [SerializeField] private PlayerVisual playerVisual;
        
        private bool _isWalking;
        private Vector3 _lastInteractDirection;
        private KitchenObject _kitchenObject;
        private BaseCounter _selectedCounter;

        private void Start() {
            GameInput.Instance.OnInteractAction += GameInputOnInteractAction;
            GameInput.Instance.OnInteractAlternateAction += GameInputOnInteractAlternateAction;

            if (GameManagerMultiplayer.Instance.TryGetPlayerDataForClientId(OwnerClientId, out var playerData)) {
                playerVisual.SetPlayerColor(GameManagerMultiplayer.Instance.GetPlayerColorByColorId(playerData.colorId));
            }
        }

        public override void OnNetworkSpawn() {
            if (IsServer) {
                NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManagerOnClientDisconnectCallback;
            }
            
            if (!IsOwner) return;
            LocalInstance = this;
            
            transform.position = spawnPositions[GameManagerMultiplayer.Instance.GetPlayerDataIndexForClientId(OwnerClientId)];
            localPlayerSpawned?.Invoke(this, EventArgs.Empty);
        }

        private void NetworkManagerOnClientDisconnectCallback(ulong clientId) {
            if (clientId == OwnerClientId && HasKitchenObject()) {
                KitchenObject.DestroyKitchenObject(GetKitchenObject());
            }
        }
        
        private void Update() {
            if (!IsOwner) return;
            HandleMovement();
            HandleSelectCounter();
        }

        private void HandleMovement() {
            var inputVector = GameInput.Instance.GetMovementNormalizedVector();
            var moveDirection= new Vector3(inputVector.x, 0f, inputVector.y);
            var moveDistance = moveSpeed * Time.deltaTime;
        
            float playerRadius = .65f;
            float moveDeadZone = .4f;
            //DebugExtension.DebugCapsule(transform.position + (transform.forward * moveDistance), (transform.position + (Vector3.up * 2)) + (transform.forward * moveDistance), Color.magenta, playerRadius);
            //bool collided = Physics.CapsuleCast(transform.position, transform.position + (Vector3.up * 2), playerRadius, moveDirection, moveDistance, collisionLayerMask);
            bool collided = Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveDirection, Quaternion.identity, moveDistance, collisionLayerMask);
            if (collided) {
                //Try move X and Z independently
                var moveX = new Vector3(moveDirection.x, 0, 0).normalized;
                var moveZ = new Vector3(0, 0, moveDirection.z).normalized;
                 if (Mathf.Abs(moveDirection.x) >= moveDeadZone && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveX, Quaternion.identity, moveDistance, collisionLayerMask)) {
                     moveDirection = moveX;
                    collided = false;
                 } else if (Mathf.Abs(moveDirection.z) >= moveDeadZone && !Physics.BoxCast(transform.position, Vector3.one * playerRadius, moveZ, Quaternion.identity, moveDistance,collisionLayerMask)) {
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
            var inputVector = GameInput.Instance.GetMovementNormalizedVector();
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
                    baseCounter = selectedCounter
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
                anyonePickedSomething?.Invoke(this, EventArgs.Empty);
            }
        }

        public void ClearKitchenObject() {
            _kitchenObject = null;
        }

        public bool HasKitchenObject() {
            return _kitchenObject != null;
        }

        public NetworkObject GetNetworkObject() {
            return NetworkObject;
        }
    }
}