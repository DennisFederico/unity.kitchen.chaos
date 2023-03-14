using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour {

    [SerializeField] private CuttingCounter cuttingCounter;
    private Animator _animator;
    private static readonly int CutTrigger = Animator.StringToHash("Cut");

    private void Awake() {
        _animator = GetComponent<Animator>();
    }

    private void Start() {
        cuttingCounter.OnCutAction += CuttingCounterOnCutAction;
        
    }

    private void CuttingCounterOnCutAction() {
        _animator.SetTrigger(CutTrigger);
    }
}