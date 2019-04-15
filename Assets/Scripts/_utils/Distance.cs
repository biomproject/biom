using System.Collections.Generic;
using System;

public static class Distance {
    public static HexCell getFurthestCell(HexCell origin, HexCell[] cells) {
		int furthestDistance = 0;
		HexCell furthestCell = null;
		for (int i = 0; i < cells.Length; i++) {
			int distance = cells[i].coordinates.DistanceTo(origin.coordinates);
			if (distance > furthestDistance) {
				furthestDistance = distance;
				furthestCell = cells[i];
			}
		}
		return furthestCell;
	}

	public static HexCell getFurthestPathCell(HexCell origin, HexCell[] cells) {
		for (int i = 0; i < cells.Length; i++) {
			cells[i].Distance = int.MaxValue;
		}

		Queue<HexCell> frontier = new Queue<HexCell>();
		frontier.Enqueue(origin);
		while (frontier.Count > 0) {
			HexCell current = frontier.Dequeue();

			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++) {
				HexCell neighbor = current.GetNeighbor(d);
				if (neighbor != null && neighbor.Distance == int.MaxValue) {
					neighbor.Distance = current.Distance + 1;
					frontier.Enqueue(neighbor);
				}
			}
		}

		int furthestDistance = 0;
		HexCell furthestCell = null;
		for (int i = 0; i < cells.Length; i++) {
			if (cells[i].Distance > furthestDistance) {
				furthestDistance = cells[i].Distance;
				furthestCell = cells[i];
			}
		}
		return furthestCell;
	}

	public static int AbsDistanceTimes100 (HexCoordinates a, HexCoordinates b) {
		return (int) (Math.Sqrt(Math.Pow((a.X - b.X), 2) + Math.Pow((a.Z - b.Z), 2)) * 100);
	}
}
