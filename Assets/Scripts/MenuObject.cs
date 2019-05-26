using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class MenuObject : MonoBehaviour {
    public bool isUp = true;
    bool isTitleUp = true;
    bool isTutorialUp = false;
    Animator anim;

    void Awake() {
        anim = GetComponent<Animator>();
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            if (isTitleUp) {
                isTitleUp = false;
                isTutorialUp = true;
                anim.Play("title_tut_anim");
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
