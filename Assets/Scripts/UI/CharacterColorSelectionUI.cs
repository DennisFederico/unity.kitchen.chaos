using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace UI {
    public class CharacterColorSelectionUI : MonoBehaviour {
        [SerializeField] private GameObject colorSelectUIPrefab;
        private List<Color> _colors;
        private readonly List<CharacterColorSelectionSingleUI> _colorSelectionUIs = new();

        private void Start() {
            _colors = GameManagerMultiplayer.Instance.GetPlayerColorList();
            for (int index = 0; index < _colors.Count; index++) {
                var colorUIGameObject = Instantiate(colorSelectUIPrefab, transform);
                CharacterColorSelectionSingleUI colorSingleUI = colorUIGameObject.GetComponent<CharacterColorSelectionSingleUI>();
                colorSingleUI.SetColor(index, _colors[index]);
                colorSingleUI.UnMarkSelected();
                _colorSelectionUIs.Add(colorSingleUI);
            }

            UpdateSelected();

            GameManagerMultiplayer.Instance.LocalPlayerColorChanged += GameManagerMultiplayerOnLocalPlayerColorChanged;
        }

        private void GameManagerMultiplayerOnLocalPlayerColorChanged(int prev, int current) {
            _colorSelectionUIs[prev].UnMarkSelected();
            _colorSelectionUIs[current].MarkSelected();
        }

        private void OnDestroy() {
            GameManagerMultiplayer.Instance.LocalPlayerColorChanged -= GameManagerMultiplayerOnLocalPlayerColorChanged;
        }

        private void UpdateSelected() {
            if (GameManagerMultiplayer.Instance.TryGetPlayerDataForClientId(NetworkManager.Singleton.LocalClientId, out var playerData)) {
                _colorSelectionUIs[playerData.colorId].MarkSelected();
            }
        }
    }
}