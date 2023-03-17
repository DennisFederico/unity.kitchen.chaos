using ScriptableObjects;
using UnityEngine;
using UnityEngine.UI;

public class PlateIconSingleUI : MonoBehaviour {

    [SerializeField] private Image icon;

    public void SetKitchenObjectScriptable(KitchenObjectScriptable kitchenObjectScriptable) {
        icon.sprite = kitchenObjectScriptable.sprite;
    }

}