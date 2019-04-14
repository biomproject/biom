using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCell : MonoBehaviour {
    public HexCoordinates coordinates;
    private Animator anim;

    void Awake() {
        anim = GetComponent<Animator>();
    }

    public void PlayTargetingAnim() {
        anim.Play("bubble_target");
    }
    public void PlayDefaultAnim() {
        anim.Play("bubble_anim");
    }
}
