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
