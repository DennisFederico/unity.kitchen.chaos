using System;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour {
    [SerializeField] private CuttingCounter cuttingCounter;
    [SerializeField] private Image barImage;

    private void Start() {
        cuttingCounter.OnProgressChange += CuttingCounterOnProgressChange;
        barImage.fillAmount = 0f;
        Hide();
    }

    private void CuttingCounterOnProgressChange(float amount) {
        barImage.fillAmount = amount;
        if (amount == 0f || Math.Abs(amount - 1f) < 0.1f) {
            Hide();
        } else {
            Show();
        }
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void Show() {
        gameObject.SetActive(true);
    }
}