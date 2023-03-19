using UnityEngine;

namespace Player {
    public class PlayerSoundFx : MonoBehaviour {
        private Player _player;
        private float _footStepsTimer;
        private const float FootStepsFrequency = .1f;

        private void Awake() {
            _player = GetComponent<Player>();
        }

        private void Update() {
            _footStepsTimer += Time.deltaTime;
            if (_footStepsTimer > FootStepsFrequency) {
                _footStepsTimer = 0f;
                if (_player.IsWalking()) {
                    AudioManager.Instance.PlayFootStepsSound(transform.position);    
                }
            }
        }
    }
}