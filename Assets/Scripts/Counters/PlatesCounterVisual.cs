using System.Collections.Generic;
using UnityEngine;

namespace Counters {
    public class PlatesCounterVisual : MonoBehaviour {

        [SerializeField] private PlatesCounter platesCounter;
        [SerializeField] private Transform counterTopPoint;
        [SerializeField] private Transform plateVisualPrefab;

        private readonly List<GameObject> _plateVisualsList = new List<GameObject>();
        private const float PlateOffsetY = 0.1f;

        private void Start() {
            platesCounter.PlateSpawned += PlatesCounterOnPlateSpawned;
            platesCounter.PlateGrabbed += PlatesCounterOnPlateGrabbed;
        }
        
        private void PlatesCounterOnPlateSpawned() {
            var plateVisual = Instantiate(plateVisualPrefab, counterTopPoint);
            plateVisual.localPosition = new Vector3(0, PlateOffsetY * _plateVisualsList.Count, 0);
            _plateVisualsList.Add(plateVisual.gameObject);
        }
        
        private void PlatesCounterOnPlateGrabbed() {
            var lastPlate = _plateVisualsList[^1];
            _plateVisualsList.Remove(lastPlate);
            Destroy(lastPlate);
        }
    }
}