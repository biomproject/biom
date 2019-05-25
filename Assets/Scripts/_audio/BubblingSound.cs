using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class BubblingSound : MonoBehaviour {
    [FMODUnity.EventRef]
    public string selectSound;

    public FMOD.Studio.EventInstance PlayBubblingSound;

    void Awake() {
        PlayBubblingSound = FMODUnity.RuntimeManager.CreateInstance(this.selectSound);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(PlayBubblingSound, GetComponent<Transform>(), GetComponent<Rigidbody>());
        PlayBubblingSound.start();
    }

    public void PlaySound() {
        PlayBubblingSound.start();
    }

    public void StopSound() {
        PlayBubblingSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}
