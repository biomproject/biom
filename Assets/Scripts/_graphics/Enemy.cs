using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {
    public HexCoordinates coordinates;
    public bool isEaten;
    public PlayerCell beingEatenBy;
    public int hp = 4;
    Animator anim;

    void Awake() {
        foreach (Transform child in transform) {
            if (child.name == "Face") {
                anim = child.GetComponent<Animator>();
            }
        }
    }

    public void ChangePosition(HexCoordinates position) {
        transform.position = HexCoordinates.ToPosition(position);
        coordinates = position;
    }

    public void SetHp(int newHp) {
        hp = newHp;
        if (newHp < 1) {
            anim.Play("eaten");
            return;
        }
        anim.Play("eaten" + newHp.ToString());
    }
}