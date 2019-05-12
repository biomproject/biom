using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;

public class CellBeats: MonoBehaviour {

    public void MoveWithCenter(HexCoordinates coordinates, Tile[] tiles, HexCell[] emptyCells) {
        StartCoroutine(MoveWithCenterCo(coordinates, tiles, emptyCells));
    }
    public IEnumerator MoveWithCenterCo(HexCoordinates coordinates, Tile[] tiles, HexCell[] emptyCells) {
        // does this fuck up the Distance calcs? Yes it does but why
        for (int i = 0; i < emptyCells.Length; i++) {
			emptyCells[i].Distance = int.MaxValue;
		}
        int highestDistance = 0;

        HexCell centerCell = Array.Find(emptyCells, cell => cell.coordinates.ToString() == coordinates.ToString());
        centerCell.Distance = 0;

        Queue<HexCell> frontier = new Queue<HexCell>();
		frontier.Enqueue(centerCell);
		while (frontier.Count > 0) {
			HexCell current = frontier.Dequeue();

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor(d);
				if (neighbor != null && neighbor.Distance == int.MaxValue) {
					neighbor.Distance = current.Distance + 1;
                    if (neighbor.Distance <= 10) {
					    frontier.Enqueue(neighbor);
                    }

                    if (neighbor.Distance > highestDistance) {
                        highestDistance = neighbor.Distance;
                    }
				}
			}
		}

        for (int i = 0; i <= highestDistance; i++) {
            if (i != 0) {
                yield return new WaitForSeconds(0.13f);
            }
            HexCell[] currentCells = Array.FindAll(emptyCells, cell => cell.Distance == i);

            for (int j = 0; j < currentCells.Length; j++) {
                Tile currentTile = Array.Find(tiles, tile => tile.coordinates.ToString() == currentCells[j].coordinates.ToString());
                currentTile.Nudge();
            }
        }
    }
}