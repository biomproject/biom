using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class PlayerCellWall {
    public static Tuple<PlayerCellWallCase, int> GetArcheTypeAndDegree(PlayerCellWallCase realCase) {
        return PlayerCellWall.GetArcheTypeAndDegree(realCase.ToString());
    }
    public static Tuple<PlayerCellWallCase, int> GetArcheTypeAndDegree(string realCase) {
        PlayerCellWallCase result = PlayerCellWallCase.IIOIII;
        int rotateDegree = 0;

        for (int i = 0; i < 6; i++) {
            bool isContained = Enum.GetNames(typeof(PlayerCellWallCase)).Contains(realCase);
            Debug.Log(realCase + " " + i);

            if (isContained) {
                Enum.TryParse(realCase, out PlayerCellWallCase wallCase);
                result = wallCase;

                // TODO: add correction bc i cant find anything else
                if (result == PlayerCellWallCase.IIIOOI) {
                    rotateDegree = i * 60;
                } else if (result == PlayerCellWallCase.IIOIII) {
                    rotateDegree = (i + 2) * 60;
                } else if (result == PlayerCellWallCase.IIOOOI) {
                    rotateDegree = (i + 1) * 60;
                } else if (result == PlayerCellWallCase.OIIOII) {
                    rotateDegree = (i + 1) * 60;
                } else if (result == PlayerCellWallCase.OIOIII) {
                    rotateDegree = (i - 1) * 60;
                } else if (result == PlayerCellWallCase.OIOIOI) {
                    rotateDegree = (i + 1) * 60;
                } else if (result == PlayerCellWallCase.OIOOII) {
                    rotateDegree = (i + 1) * 60;
                } else if (result == PlayerCellWallCase.OIIOOI) {
                    rotateDegree = (i + 1) * 60;
                } else {
                    rotateDegree = i * 60;
                }
                break;
            } else {
                realCase = PlayerCellWall.RotateString(realCase);
            }
        }

        return Tuple.Create(result, rotateDegree);
    }

    public static string RotateString(string realCase) {
        return realCase[5] + realCase.Substring(0, 5);
    }
}
