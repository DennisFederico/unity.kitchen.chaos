using UnityEngine;

public class PlayerAnimator : MonoBehaviour {
    [SerializeField] private Player player;
    
    private Animator _animator;
    private const string IsWalkingAnimParamName = "IsWalking";
    private readonly int _isWalkingAnimParameter = Animator.StringToHash(IsWalkingAnimParamName);

    private void Awake() {
        player = GetComponentInParent<Player>();
        _animator = GetComponent<Animator>();
    }

    private void Update() {
        _animator.SetBool(_isWalkingAnimParameter, player.IsWalking());
    }
}