using Unity.Netcode;
using UnityEngine;

namespace Player {
    public class PlayerAnimator : NetworkBehaviour {
        private Player _player;
    
        private Animator _animator;
        private const string IsWalkingAnimParamName = "IsWalking";
        private readonly int _isWalkingAnimParameter = Animator.StringToHash(IsWalkingAnimParamName);

        private void Awake() {
            _player = GetComponentInParent<Player>();
            _animator = GetComponent<Animator>();
        }

        private void Update() {
            if (!IsOwner) return;
            _animator.SetBool(_isWalkingAnimParameter, _player.IsWalking());
        }
    }
}