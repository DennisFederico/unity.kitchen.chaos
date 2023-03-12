using UnityEngine;

public class CounterSelected : MonoBehaviour {

    [SerializeField] private ClearCounter clearCounter;
    [SerializeField] private GameObject selectedVisual;

    private void Start() {
        Player.Instance.OnSelectedCounterChanged += PlayerOnOnSelectedCounterChanged;
    }
    
    // private void OnEnable() {
    //     Player.Instance.OnSelectedCounterChanged += PlayerOnOnSelectedCounterChanged;
    // }

    // private void OnDisable() {
    //     Player.Instance.OnSelectedCounterChanged -= PlayerOnOnSelectedCounterChanged;
    // }

    private void PlayerOnOnSelectedCounterChanged(Player.SelectedCounter counterSelectedChange) {
        selectedVisual.SetActive(clearCounter == counterSelectedChange.ClearCounter);
    }

    // private void Show() {
    //     selectedVisual.SetActive(true);
    // }
    //
    // private void Hide() {
    //     selectedVisual.SetActive(false);
    // }
}