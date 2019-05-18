using System;
using System.Collections.Generic;
using UnityEngine;

public class HexGrid: MonoBehaviour {

	public int width = 15;
	public int height = 10;

	public HexCell cellPrefab;

	FirstLevel currentLevel;
	public HexCell[] cells;
	Canvas gridCanvas;
	HexMesh hexMesh;

	public HexCell touchedCell;
	public HexCell furthestCell;
	public HexCell hoveredCellForGraphics;

	void Awake () {
		currentLevel = GameObject.Find("First Level").GetComponent<FirstLevel>();
		hexMesh = GetComponentInChildren<HexMesh>();
		cells = new HexCell[height * width];

		for (int x = 0, i = 0; x < width; x++) {
			for (int z = 0; z < height; z++) {
				CreateCell(x, z, i++);
			}
		}

		for (int x = 0, i = 0; x < width; x++) {
			for (int z = 0; z < height; z++) {
				HexCell cell = cells[i];

				if (z > 0) {
					cell.SetNeighbor(HexDirection.W, cells[i - 1]);
				}
				if (x > 0) {
					if ((x & 1) == 0) {
						cell.SetNeighbor(HexDirection.SE, cells[i - height]);
						if (z > 0) {
							cell.SetNeighbor(HexDirection.SW, cells[i - height - 1]);
						}
					}
					else {
						cell.SetNeighbor(HexDirection.SW, cells[i - height]);
						if (z < height - 1) {
							cell.SetNeighbor(HexDirection.SE, cells[i - height + 1]);
						}
					}
				}
				i++;
			}
		}
	}

	void CreateCell (int x, int z, int i) {
		Vector3 position;
		position.x = x * HexMetrics.outerRadius * 1.5f;
		position.y = 0f;
		position.z = (z + (x * 0.5f) - (x / 2)) * (HexMetrics.innerRadius * 2f);

		HexCell cell = cells[i] = Instantiate<HexCell>(cellPrefab);
		cell.transform.SetParent(transform, false);
		cell.transform.localPosition = position;
		cell.coordinates = HexCoordinates.FromOffsetCoordinates(z, x);
	}

	void Start () {
		SetupCellStatuses();
	}

	void SetupCellStatuses() {
		for (int i = 0; i < currentLevel.playerCoordinates.Length; i++) {
			cells[getCellIndexByHexCoordinates(currentLevel.playerCoordinates[i])].setStatus(HexCellStatus.PLAYER);
		}
		for (int i = 0; i < currentLevel.wallCoordinates.Length; i++) {
			cells[getCellIndexByHexCoordinates(currentLevel.wallCoordinates[i])].setStatus(HexCellStatus.WALL);
		}
	}

	void Update() {
		HandleInput();

		hexMesh.Triangulate(cells);
	}

	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			HoverOverCell(hit.point);
			if (Input.GetMouseButton(0)) {
				TouchCell(hit.point);
			}
		}
		if (Input.GetMouseButton(0)) {
			if (touchedCell && Array.FindIndex(touchedCell.GetNeighbors(), neighbor => neighbor.status == HexCellStatus.PLAYER) == -1) {
				touchedCell = null;
			}
			if (touchedCell && touchedCell.status == HexCellStatus.PLAYER) {
				touchedCell = null;
			}
		}
	}

	void HoverOverCell(Vector3 position) {
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);
		if (!hoveredCellForGraphics || hoveredCellForGraphics.coordinates.ToString() != coordinates.ToString()) {
			hoveredCellForGraphics = cells[getCellIndexByHexCoordinates(coordinates)];
		}
	}
	
	void TouchCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);

		// only touch neighbor cells
		HexCell[] playerHexCells = getCellsByStatus(HexCellStatus.PLAYER);
		for (int i = 0; i < playerHexCells.Length; i++) {
			if (Array.IndexOf(playerHexCells[i].GetNeighbors(), cells[getCellIndexByHexCoordinates(coordinates)]) > -1) {
				touchedCell = cells[getCellIndexByHexCoordinates(coordinates)];
			}
		}

		// only move if there was a dragging motion
		if (touchedCell.status == HexCellStatus.PLAYER) {
			for (int i = 0; i < cells.Length; i++) {
				cells[i].movementStartedFromThis = false;
			}
			touchedCell.movementStartedFromThis = true;
		}
	}

	int getCellIndexByHexCoordinates(HexCoordinates coordinates) {
		return coordinates.X + coordinates.Z * height + coordinates.Z / 2;
	}

	public HexCell[] getCellsByStatus(HexCellStatus status) {
		return Array.FindAll(cells, c => c.status == status);
	}
	public HexCell[] getCellsByStatus(GameControlsStatus status) {
		return Array.FindAll(cells, c => c.controlsStatus == status);
	}

	public HexCell[] getCellsByNotStatus(HexCellStatus status) {
		return Array.FindAll(cells, c => c.status != status);
	}

}
