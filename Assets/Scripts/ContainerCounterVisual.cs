using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour {

    [SerializeField] private ContainerCounter containerCounter;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Animator _animator;
    private static readonly int OpenClose = Animator.StringToHash("OpenClose");

    private void Awake() {
        _animator = GetComponent<Animator>();
        spriteRenderer.sprite = containerCounter.GetContainerSprite();
    }

    private void Start() {
        containerCounter.OnGrabObjectFromContainer += ContainerCounter_OnGrabObjectFromContainer;
    }

    private void ContainerCounter_OnGrabObjectFromContainer() {
        _animator.SetTrigger(OpenClose);
    }
}
