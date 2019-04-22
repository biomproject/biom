using UnityEngine;
using System;
using System.Collections.Generic;

public static class Centering {
	public static PlayerCell FindCenterCell(PlayerCell[] playerCells) {
		float tX = 0;
		float tZ = 0;
		for (int i = 0; i < playerCells.Length; i++) {
			tX += HexCoordinates.ToPosition(playerCells[i].coordinates).x;
			tZ += HexCoordinates.ToPosition(playerCells[i].coordinates).z;
		}

		HexCoordinates centerCoordinates = HexCoordinates.FromPosition(
			new Vector3((int)Math.Round((double)(tX / playerCells.Length), 0), 0, (int)Math.Round((double)(tZ / playerCells.Length), 0))
		);
		List<PlayerCell> orderedPlayerCells = new List<PlayerCell>(playerCells);
		orderedPlayerCells.Sort((a, b) => Distance.AbsDistanceTimes100(a.coordinates, centerCoordinates) - Distance.AbsDistanceTimes100(b.coordinates, centerCoordinates));

		return orderedPlayerCells[0];
	}
    public static HexCoordinates FindCenter(PlayerCell[] playerCells) {
		return Centering.FindCenterCell(playerCells).coordinates;
    }
}