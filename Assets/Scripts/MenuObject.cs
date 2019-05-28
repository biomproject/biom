using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MenuObject : MonoBehaviour {
    public bool isUp = true;
    bool isTitleUp = true;
    bool isTutorialUp = false;
    Animator anim;
    Animator loaderAnim;
    ScriptUsageTimeline scriptUsageTimeline;
    bool waitingForBeat = false;
    int previousBeat = 0;

    GameObject loader;

    void Awake() {
        anim = GetComponent<Animator>();
        scriptUsageTimeline = GameObject.Find("Music Player").GetComponent<ScriptUsageTimeline>();
        loader = GameObject.Find("Loader");
        loaderAnim = loader.GetComponent<Animator>();
    }

    void Update() {
        if (waitingForBeat) {
		    if (previousBeat != scriptUsageTimeline.timelineInfo.currentMusicBar) {
                loader.transform.Translate(1000, 1000, 1000);
                waitingForBeat = false;
                isTitleUp = false;
                isTutorialUp = true;
                anim.Play("title_tut_anim");
                scriptUsageTimeline.MakeBeatLoud();
            }
            return;
        }

        if (Input.GetMouseButtonDown(0)) {
            if (isTitleUp) {
                previousBeat = scriptUsageTimeline.timelineInfo.currentMusicBar;
                waitingForBeat = true;
                loaderAnim.Play("loading");
                return;
            }

            if (isTutorialUp) {
                isTutorialUp = false;
                isUp = false;
			    transform.eulerAngles = new Vector3(
			    	0,
			    	transform.eulerAngles.y,
			    	transform.eulerAngles.z
			    );
            }
        }
    }
}
