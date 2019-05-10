using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour {
    private Animator anim;
    private int animNo;
    public HexCoordinates coordinates;

    void Awake() {
        anim = GetComponent<Animator>();
    }

    public void PlayRandomAnim() {
        animNo = UnityEngine.Random.Range(1, 5); 
        anim.Play("tile_0" + animNo.ToString() + "_anim");
    }

    public void Nudge() {
        anim.Play("tile_0" + animNo.ToString() + "_anim" + "_nudge");
    }
}
