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
        wallAnim.Play("bubble_target");
        SpriteRenderer sr = shine.GetComponent<SpriteRenderer>();
        sr.transform.Translate(1000, 1000, 1000);
        SpriteRenderer sr2 = redBloodCells.GetComponent<SpriteRenderer>();
        sr2.transform.Translate(1000, 1000, 1000);
    }
    public void PlayCellWallAnim(PlayerCellWallCase wallCase) {
        Tuple<PlayerCellWallCase, int> archeTypeAndDegree = PlayerCellWall.GetArcheTypeAndDegree(wallCase);
        wallAnim.Play(archeTypeAndDegree.Item1.ToString());
        Quaternion target = Quaternion.AngleAxis((float)archeTypeAndDegree.Item2, Vector3.up);
        wall.eulerAngles =  new Vector3(
            transform.eulerAngles.x,
            archeTypeAndDegree.Item2,
            transform.eulerAngles.z
        );
    }

    public void PlayCellWallAnim(string wallCase) {
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
