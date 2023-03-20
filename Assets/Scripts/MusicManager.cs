using UnityEngine;

public class MusicManager : MonoBehaviour {
    
    private const string ConstPlayerPrefsMusicVolume = "MusicVolume"; 
    public static MusicManager Instance { get; private set; }
    private AudioSource _musicAudioSource;
    private float _volume = .3f;

    private void Awake() {
        Instance = this;
        _volume = PlayerPrefs.GetFloat(ConstPlayerPrefsMusicVolume, .3f);
        _musicAudioSource = GetComponent<AudioSource>();
        _musicAudioSource.volume = _volume;
    }

    public void ChangeVolume() {
        _volume += 0.1f;
        if (_volume > 1.05f) {
            _volume = 0f;
        }
        _musicAudioSource.volume = _volume;
        
        PlayerPrefs.SetFloat(ConstPlayerPrefsMusicVolume, _volume);
        PlayerPrefs.Save();
    }

    public float GetVolume() {
        return _volume;
    }
}