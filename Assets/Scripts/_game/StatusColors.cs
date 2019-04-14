using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

static class StatusColors {
    public static Color GetColor(HexCellStatus status) {
        if (status == HexCellStatus.EMPTY) {
            return Color.white;
        } else if (status == HexCellStatus.PLAYER) {
            return Color.blue;
        } else if (status == HexCellStatus.WALL) {
            return Color.black;
        }
        return Color.red;
    }

    public static Color GetColor(GameControlsStatus status) {
        if (status == GameControlsStatus.HOVERED) {
            return Color.yellow;
        } else if (status == GameControlsStatus.FURTHEST) {
            return Color.red;
        } else if (status == GameControlsStatus.NEIGHBOR) {
            return Color.cyan;
        }
        return Color.black;
    }

}