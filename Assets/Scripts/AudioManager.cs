using System;
using System.Collections.Generic;
using Counters;
using ScriptableObjects;
using UnityEngine;
using Random = UnityEngine.Random;

public class AudioManager : MonoBehaviour {

    private const string ConstPlayerPrefsSoundFxVolume = "SoundFxVolume"; 
    public static AudioManager Instance { private set; get; }
    [SerializeField] private AudioClipReferencesScriptable audioClipReferences;

    private float _volume = 1f;
    
    private void Awake() {
        Instance = this;
        _volume = PlayerPrefs.GetFloat(ConstPlayerPrefsSoundFxVolume, 1f);
    }

    private void Start() {
        Player.Player.Instance.PickedSomething += PlayerOnPickedSomething;
        BaseCounter.AnyItemPlaced += BaseCounterOnAnyItemPlaced;
        DeliveryManager.Instance.FailedOrder += DeliveryManagerOnFailedOrder;
        DeliveryManager.Instance.SuccessfulOrder += DeliveryManagerOnSuccessfulOrder;
        CuttingCounter.OnAnyCut += CuttingCounterOnAnyCut;
        TrashCounter.ObjectTrashed += TrashCounterOnObjectTrashed;
    }

    public void PlayFootStepsSound(Vector3 position, float volume = 1f) {
        PlayRandomSound(audioClipReferences.footstep, position, volume);
    }
    
    public void PlayCountDownSound() {
        PlayRandomSound(audioClipReferences.warning, Vector3.zero);
    }
    
    private void TrashCounterOnObjectTrashed(object sender, EventArgs e) {
        var counter = (BaseCounter) sender;
        PlayRandomSound(audioClipReferences.trashObject, counter.transform.position);
    }

    private void BaseCounterOnAnyItemPlaced(object sender, EventArgs e) {
        var counter = (BaseCounter) sender;
        PlayRandomSound(audioClipReferences.objectDrop, counter.transform.position);
    }

    private void PlayerOnPickedSomething(object sender, EventArgs e) {
        var player = (Player.Player) sender;
        PlayRandomSound(audioClipReferences.objectPickup, player.transform.position);
    }

    private void CuttingCounterOnAnyCut(object sender, EventArgs e) {
        var counter = (BaseCounter) sender;
        PlayRandomSound(audioClipReferences.chop, counter.transform.position);
    }

    private void DeliveryManagerOnSuccessfulOrder(object sender, EventArgs e) {
        var counter = (BaseCounter) sender;
        PlayRandomSound(audioClipReferences.deliverySuccess, counter.transform.position);
    }

    private void DeliveryManagerOnFailedOrder(object sender, EventArgs e) {
        var counter = (BaseCounter) sender;
        PlayRandomSound(audioClipReferences.deliveryFail, counter.transform.position);
    }

    private void PlayRandomSound(IReadOnlyList<AudioClip> audioClipArray, Vector3 position, float volume = 1f) {
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Count)], position, volume);
    }
    
    private void PlaySound(AudioClip audioClip, Vector3 position, float volumeMultiplier = 1f) {
        AudioSource.PlayClipAtPoint(audioClip, position, volumeMultiplier * _volume);
    }

    public void ChangeVolume() {
        _volume += 0.1f;
        if (_volume > 1.05f) {
            _volume = 0f;
        }

        PlayerPrefs.SetFloat(ConstPlayerPrefsSoundFxVolume, _volume);
        PlayerPrefs.Save();
    }

    public float GetVolume() {
        return _volume;
    }
}