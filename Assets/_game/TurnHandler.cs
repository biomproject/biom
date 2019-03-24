using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnHandler : MonoBehaviour {

	ScriptUsageTimeline scriptUsageTimeline;
	HexGrid hexGrid;
	HexMesh hexMesh;
	FirstLevel currentLevel;
	private int previousBeat = 0;

	public PlayerCell playerCellPrefab;
	PlayerCell[] playerCells;

	void Awake () {
		scriptUsageTimeline = GameObject.Find("Music Player").GetComponent<ScriptUsageTimeline>();
		hexGrid = GameObject.Find("Hex Grid").GetComponent<HexGrid>();
		hexMesh = GameObject.Find("Hex Mesh").GetComponent<HexMesh>();
		currentLevel = GameObject.Find("First Level").GetComponent<FirstLevel>();
		playerCells = new PlayerCell[currentLevel.playerCoordinates.Length];
	}

	void Start () {
		InitPlayer();
	}

	void InitPlayer() {
		for(int i = 0; i < currentLevel.playerCoordinates.Length; i++) {
			playerCells[i] = Instantiate<PlayerCell>(playerCellPrefab);
			playerCells[i].transform.position = HexCoordinates.ToPosition(currentLevel.playerCoordinates[i], -1);
			playerCells[i].hexCoordinates = currentLevel.playerCoordinates[i];
		}
	}

	void Update () {
		ResetDraw();
		DrawNeighbors();
		DrawHoverAndFurthest();

		if (previousBeat != scriptUsageTimeline.timelineInfo.currentMusicBar) {
			previousBeat = scriptUsageTimeline.timelineInfo.currentMusicBar;
			DoTurn();
		}
	}

	void ResetDraw() {
		// undraw GameControlsStatus things
		for (int i = 0; i < hexGrid.cells.Length; i++) {
			hexGrid.cells[i].setControlsStatuc(GameControlsStatus.NOTHING);
			hexGrid.cells[i].color = StatusColors.GetColor(hexGrid.cells[i].status);
		}
	}

	void DrawNeighbors() {
		// draw neighbors that could be moved to
		if (hexGrid.touchedCell && Input.GetMouseButton(0) && hexGrid.touchedCell.status == HexCellStatus.PLAYER) {
			HexCell[] neighbors = hexGrid.touchedCell.GetNeighbors();
			for (int i = 0; i < neighbors.Length; i++) {
				if (neighbors[i].status != HexCellStatus.PLAYER && neighbors[i].status != HexCellStatus.WALL) {
					neighbors[i].setControlsStatuc(GameControlsStatus.NEIGHBOR);
				}
			}
		}
	}

	void DrawHoverAndFurthest() {
		// check if there is a touched cell
		if (!hexGrid.touchedCell) {
			return;
		}

		// check if touchedCell is a legal move
		bool isLegal = false;
		HexCell[] playerCells = hexGrid.getCellsByStatus(HexCellStatus.PLAYER);
		for (int i = 0; i < playerCells.Length; i++) {
			if (Array.IndexOf(playerCells[i].GetNeighbors(), hexGrid.touchedCell) > -1) {
				isLegal = true;
			}
		}
		if(!isLegal) {
			return;
		}

		// draw hover and furthest
		if (hexGrid.touchedCell && Input.GetMouseButton(0) && hexGrid.touchedCell.status == HexCellStatus.EMPTY) {
			hexGrid.touchedCell.setControlsStatuc(GameControlsStatus.HOVERED);
			Distance.getFurthestPathCell(hexGrid.touchedCell, hexGrid.getCellsByStatus(HexCellStatus.PLAYER)).setControlsStatuc(GameControlsStatus.FURTHEST);
		}
	}

	void DoTurn() {
		MovePlayer();
		MoveEnemies();
		DoFight();
	}

	private void MovePlayer() {
		HexCell[] hoveredCells = hexGrid.getCellsByStatus(GameControlsStatus.HOVERED);
		HexCell[] furthestCells = hexGrid.getCellsByStatus(GameControlsStatus.FURTHEST);
		if (hoveredCells.Length != 0 && furthestCells.Length != 0) {
			HexCell hoveredCell = hoveredCells[0];
			HexCell furthestCell = furthestCells[0];

			// create/destroy player objects
			int removedPlayerCellIndex = Array.FindIndex(playerCells, cell => cell.hexCoordinates.ToString() == furthestCell.coordinates.ToString());
			Destroy(playerCells[removedPlayerCellIndex].transform.gameObject);
	
			playerCells[removedPlayerCellIndex] = Instantiate<PlayerCell>(playerCellPrefab);
			playerCells[removedPlayerCellIndex].transform.position = HexCoordinates.ToPosition(hoveredCell.coordinates, -1);
			playerCells[removedPlayerCellIndex].hexCoordinates = hoveredCell.coordinates;


			// set game state
			hoveredCell.setStatus(HexCellStatus.PLAYER);
			hoveredCell.setControlsStatuc(GameControlsStatus.NOTHING);
			furthestCell.setStatus(HexCellStatus.EMPTY);
			furthestCell.setControlsStatuc(GameControlsStatus.NOTHING);
		}
	}
	private void MoveEnemies() {}
	private void DoFight() {}
}
