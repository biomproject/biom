using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class SlidingSound : MonoBehaviour {
    [FMODUnity.EventRef]
    public string selectSound;

    public FMOD.Studio.EventInstance PlaySlidingSound;

    void Awake() {
        PlaySlidingSound = FMODUnity.RuntimeManager.CreateInstance(this.selectSound);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(PlaySlidingSound, GetComponent<Transform>(), GetComponent<Rigidbody>());
    }

    public void PlaySound() {
        PlaySlidingSound.start();
    }

    public void StopSound() {
        // PlaySlidingSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        // PlaySlidingSound.release();
    }

    void OnDestroy() {
        StopSound();
    }
}
