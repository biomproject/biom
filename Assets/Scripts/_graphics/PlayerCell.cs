using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCell : MonoBehaviour {
    public HexCoordinates coordinates;
    public Enemy isEating;
    private Animator wallAnim;
    private Transform shine;
    private Transform redBloodCells;
    private Transform wall;
    bool movedByTargeting = false;

    void Awake() {
        foreach (Transform child in transform) {
            if (child.name == "Shine") {
                shine = child;
            }
            if (child.name == "RedBloodCells") {
                redBloodCells = child;
            }
            if (child.name == "Wall") {
                wall = child;
                wallAnim = child.GetComponent<Animator>();
            }
        }
    }

    public void PlayTargetingAnim() {
        wallAnim.Play("bubble_target_2");
        SpriteRenderer sr = shine.GetComponent<SpriteRenderer>();
        sr.transform.Translate(1000, 1000, 1000);
        SpriteRenderer sr2 = redBloodCells.GetComponent<SpriteRenderer>();
        sr2.transform.Translate(1000, 1000, 1000);
        movedByTargeting = true;
    }

    public void PlayDisappearingAnim() {
        wallAnim.Play("bubble_furthest");
        Animator anim = shine.GetComponent<Animator>();
        anim.Play("wiggle_light");
        SpriteRenderer sr2 = redBloodCells.GetComponent<SpriteRenderer>();
        sr2.transform.Translate(1000, 1000, 1000);
        movedByTargeting = true;
    }
    public void RotateWall(int degree) {
        wall.eulerAngles =  new Vector3(
            wall.eulerAngles.x,
            degree,
            wall.eulerAngles.z
        );
    }
    public void PlayCellWallAnim(PlayerCellWallCase wallCase) {
        PlayCellWallAnim(wallCase.ToString());
    }

    public void PlayCellWallAnim(string wallCase) {
        if (movedByTargeting) {
            movedByTargeting = false;
            SpriteRenderer sr = shine.GetComponent<SpriteRenderer>();
            sr.transform.Translate(-1000, -1000, -1000);
            SpriteRenderer sr2 = redBloodCells.GetComponent<SpriteRenderer>();
            sr2.transform.Translate(-1000, -1000, -1000);
        }

        Tuple<PlayerCellWallCase, int> archeTypeAndDegree = PlayerCellWall.GetArcheTypeAndDegree(wallCase);
        wallAnim.Play(archeTypeAndDegree.Item1.ToString());
        Quaternion target = Quaternion.AngleAxis((float)archeTypeAndDegree.Item2, Vector3.up);
        wall.eulerAngles =  new Vector3(
            transform.eulerAngles.x,
            archeTypeAndDegree.Item2,
            transform.eulerAngles.z
        );
    }
}
