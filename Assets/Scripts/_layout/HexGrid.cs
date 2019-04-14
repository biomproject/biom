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
		if (Input.GetMouseButton(0)) {
			HandleInput();
		} else if (touchedCell) {
			touchedCell.setControlsStatuc(GameControlsStatus.NOTHING);
		}

		hexMesh.Triangulate(cells);
	}

	void HandleInput () {
		Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (Physics.Raycast(inputRay, out hit)) {
			TouchCell(hit.point);
		}
	}
	
	void TouchCell (Vector3 position) {
		position = transform.InverseTransformPoint(position);
		HexCoordinates coordinates = HexCoordinates.FromPosition(position);

		touchedCell = cells[getCellIndexByHexCoordinates(coordinates)];
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
