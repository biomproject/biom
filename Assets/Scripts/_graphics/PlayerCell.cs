using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class PlayerCell : MonoBehaviour {
    public HexCoordinates coordinates;
    public Enemy isEating;
    private Animator wallAnim;
    private Transform shine;
    private Transform redBloodCells;
    private Transform wall;
    bool movedByTargeting = false;
    private string archetype;
    bool hoverAnimIsPlaying = false;
    bool spawningAnimIsPlaying = false;
    BubblingSound bubblingSound;

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
            if (child.name == "Bubbling Sound") {
                bubblingSound = child.GetComponent<BubblingSound>();
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

        bubblingSound.PlaySound();
    }

    public void PlayDisappearingAnim() {
        wallAnim.Play("bubble_furthest");
        Animator anim = shine.GetComponent<Animator>();
        anim.Play("wiggle_light");
        SpriteRenderer sr2 = redBloodCells.GetComponent<SpriteRenderer>();
        sr2.transform.Translate(1000, 1000, 1000);
        movedByTargeting = true;

        // bubblingSound.StopSound();
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
        if (hoverAnimIsPlaying || spawningAnimIsPlaying) {
            return;
        }
        if (movedByTargeting) {
            movedByTargeting = false;
            SpriteRenderer sr = shine.GetComponent<SpriteRenderer>();
            sr.transform.Translate(-1000, -1000, -1000);
            SpriteRenderer sr2 = redBloodCells.GetComponent<SpriteRenderer>();
            sr2.transform.Translate(-1000, -1000, -1000);
        }

        Tuple<PlayerCellWallCase, int> archeTypeAndDegree = PlayerCellWall.GetArcheTypeAndDegree(wallCase);
        archetype = archeTypeAndDegree.Item1.ToString();
        wallAnim.Play(archeTypeAndDegree.Item1.ToString());
        Quaternion target = Quaternion.AngleAxis((float)archeTypeAndDegree.Item2, Vector3.up);
        wall.eulerAngles =  new Vector3(
            transform.eulerAngles.x,
            archeTypeAndDegree.Item2,
            transform.eulerAngles.z
        );

        // bubblingSound.StopSound();
    }

    public void PlaySpawningCellWallAnim(string wallCase, int rotateDegress) {
        spawningAnimIsPlaying = true;
        SpriteRenderer sr = shine.GetComponent<SpriteRenderer>();
        sr.transform.Translate(-1000, -1000, -1000);
        if (movedByTargeting) {
            movedByTargeting = false;
            SpriteRenderer sr2 = redBloodCells.GetComponent<SpriteRenderer>();
            sr2.transform.Translate(-1000, -1000, -1000);
        }
        Tuple<PlayerCellWallCase, int> archeTypeAndDegree = PlayerCellWall.GetArcheTypeAndDegree(wallCase);
        archetype = archeTypeAndDegree.Item1.ToString();
        wallAnim.Play("bubble_spawn_" + archeTypeAndDegree.Item1.ToString());
        Quaternion target = Quaternion.AngleAxis((float)archeTypeAndDegree.Item2, Vector3.up);
        wall.eulerAngles =  new Vector3(
            transform.eulerAngles.x,
            rotateDegress,
            transform.eulerAngles.z
        );

        // bubblingSound.StopSound();
    }

    public void CloseSpawningCellAnim() {
        spawningAnimIsPlaying = false;
        SpriteRenderer sr = shine.GetComponent<SpriteRenderer>();
        sr.transform.Translate(1000, 1000, 1000);

        // bubblingSound.StopSound();
    }

    public void PlayHoveringAnim() {
        if (hoverAnimIsPlaying || spawningAnimIsPlaying) {
            return;
        }
        hoverAnimIsPlaying = true;
        wallAnim.Play("hover_" + archetype);
        Invoke("HoverAnimStopped", 0.1f);
    }

    private void HoverAnimStopped() {
        hoverAnimIsPlaying = false;
    }
}
