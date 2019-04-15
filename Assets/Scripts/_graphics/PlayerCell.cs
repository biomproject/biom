using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCell : MonoBehaviour {
    public HexCoordinates coordinates;
    private Animator anim;
    private Transform shine;

    void Awake() {
        anim = GetComponent<Animator>();
        foreach (Transform eachChild in transform) {
            if (eachChild.name == "Shine") {
                shine = eachChild;
            }
        }
    }

    public void PlayTargetingAnim() {
        anim.Play("bubble_target");
        SpriteRenderer sr = shine.GetComponent<SpriteRenderer>();
        sr.transform.Translate(1000, 1000, 1000);
    }
    public void PlayCellWallAnim(PlayerCellWallCase wallCase) {
        Tuple<PlayerCellWallCase, int> archeTypeAndDegree = PlayerCellWall.GetArcheTypeAndDegree(wallCase);
        anim.Play(archeTypeAndDegree.Item1.ToString());
        Quaternion target = Quaternion.Euler(90, (float)archeTypeAndDegree.Item2, 0);
        transform.eulerAngles =  new Vector3(
            transform.eulerAngles.x,
            archeTypeAndDegree.Item2,
            transform.eulerAngles.z
        );
    }

    public void PlayCellWallAnim(string wallCase) {
        Tuple<PlayerCellWallCase, int> archeTypeAndDegree = PlayerCellWall.GetArcheTypeAndDegree(wallCase);
        anim.Play(archeTypeAndDegree.Item1.ToString());
        Quaternion target = Quaternion.AngleAxis((float)archeTypeAndDegree.Item2, Vector3.up);
        transform.eulerAngles =  new Vector3(
            transform.eulerAngles.x,
            archeTypeAndDegree.Item2,
            transform.eulerAngles.z
        );
    }
}
