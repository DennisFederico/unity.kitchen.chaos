using UnityEngine;

public class LookAtCamera : MonoBehaviour {

    private enum LookMode {
        CameraForward,
        CameraForwardInverted,
        LookAt,
        LookAtInverted,
    }
    
    [SerializeField] private LookMode lookMode = LookMode.CameraForward;
    private void LateUpdate() {
        switch (lookMode) {
            case LookMode.CameraForward:
                transform.forward = Camera.main.transform.forward;
                break;
            case LookMode.CameraForwardInverted:
                transform.forward = -Camera.main.transform.forward;
                break;
            case LookMode.LookAt:
                transform.LookAt(Camera.main.transform);
                break;
            case LookMode.LookAtInverted:
                var dirFromCamera = transform.position - Camera.main.transform.position;
                transform.LookAt(transform.position + dirFromCamera);
                break;
        }
    }
}