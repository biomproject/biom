using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour {
    public HexCoordinates coordinates;
    public bool isEaten;
    public PlayerCell beingEatenBy;
    public int hp = 1;
    public int movement = 0;
    Animator anim;


    void Awake() {
        foreach (Transform child in transform) {
            if (child.name == "Face") {
                anim = child.GetComponent<Animator>();
            }
        }
    }

    public void ChangePosition(HexCoordinates position) {
        if (hp < 1) {
            return;
        }
        coordinates = position;
        movement += 1;
        anim.Play("movement");
        Invoke("SetPosition", 0.15f);
    }

    private void SetPosition() {
        transform.position = HexCoordinates.ToPosition(coordinates);
    }

    public void SetHp(int newHp) {
        hp = newHp;
        if (newHp < 1) {
            anim.Play("nothing");
            return;
        }
        // anim.Play("eaten" + newHp.ToString());
    }
}