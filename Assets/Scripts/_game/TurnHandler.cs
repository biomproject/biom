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
	public Tile tilePrefab;
	PlayerCell[] playerCells;
	PlayerCell hoveredPlayerCell;
	HexDirection hoveredCellOpensToThisDirection;

	void Awake () {
		scriptUsageTimeline = GameObject.Find("Music Player").GetComponent<ScriptUsageTimeline>();
		hexGrid = GameObject.Find("Hex Grid").GetComponent<HexGrid>();
		hexMesh = GameObject.Find("Hex Mesh").GetComponent<HexMesh>();
		currentLevel = GameObject.Find("First Level").GetComponent<FirstLevel>();
		playerCells = new PlayerCell[currentLevel.playerCoordinates.Length];
	}

	void Start () {
		InitPlayer();
		InitTiles();
	}

	void InitPlayer() {
		for(int i = 0; i < currentLevel.playerCoordinates.Length; i++) {
			playerCells[i] = Instantiate<PlayerCell>(playerCellPrefab);
			playerCells[i].transform.position = HexCoordinates.ToPosition(currentLevel.playerCoordinates[i], -1);
			playerCells[i].coordinates = currentLevel.playerCoordinates[i];
		}
	}

	void InitTiles() {
		HexCell[] emptyCells = hexGrid.getCellsByNotStatus(HexCellStatus.WALL);
		for (int i = 0; i < emptyCells.Length; i++) {
			Tile tile = Instantiate<Tile>(tilePrefab);
			tile.transform.position = HexCoordinates.ToPosition(emptyCells[i].coordinates, -5);
			tile.PlayRandomAnim();
		}
	}

	void Update () {
		ResetDraw();
		DrawNeighbors();
		DrawHoverAndFurthest();
		RedrawPlayer();

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

		// draw playerCells
		if (!hoveredPlayerCell) {
			hoveredPlayerCell = Instantiate<PlayerCell>(playerCellPrefab);
			hoveredPlayerCell.transform.position = HexCoordinates.ToPosition(hexGrid.touchedCell.coordinates, -1);
			hoveredPlayerCell.coordinates = hexGrid.touchedCell.coordinates;
			Tuple<int, HexDirection> foundTouchedDirection = PlayerCellWall.FindTouchedRotation(hexGrid.touchedCell);
			hoveredCellOpensToThisDirection = foundTouchedDirection.Item2;
			hoveredPlayerCell.transform.eulerAngles =  new Vector3(
				hoveredPlayerCell.transform.eulerAngles.x,
				foundTouchedDirection.Item1,
				hoveredPlayerCell.transform.eulerAngles.z
			);
			hoveredPlayerCell.PlayTargetingAnim();
			return;
		}

		if (hoveredPlayerCell && (hoveredPlayerCell.coordinates.ToString() != hexGrid.touchedCell.coordinates.ToString())) {
			Destroy(hoveredPlayerCell.transform.gameObject);

			hoveredPlayerCell = Instantiate<PlayerCell>(playerCellPrefab);
			hoveredPlayerCell.transform.position = HexCoordinates.ToPosition(hexGrid.touchedCell.coordinates, -1);
			hoveredPlayerCell.coordinates = hexGrid.touchedCell.coordinates;
			Tuple<int, HexDirection> foundTouchedDirection = PlayerCellWall.FindTouchedRotation(hexGrid.touchedCell);
			hoveredCellOpensToThisDirection = foundTouchedDirection.Item2;
			hoveredPlayerCell.transform.eulerAngles =  new Vector3(
				hoveredPlayerCell.transform.eulerAngles.x,
				foundTouchedDirection.Item1,
				hoveredPlayerCell.transform.eulerAngles.z
			);
			hoveredPlayerCell.PlayTargetingAnim();
		}
	}

	void DoTurn() {
		MovePlayer();
		RedrawPlayer();
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
			int removedPlayerCellIndex = Array.FindIndex(playerCells, cell => {
				return cell.coordinates.ToString() == furthestCell.coordinates.ToString();
			});
			Destroy(playerCells[removedPlayerCellIndex].transform.gameObject);
	
			playerCells[removedPlayerCellIndex] = Instantiate<PlayerCell>(playerCellPrefab);
			playerCells[removedPlayerCellIndex].transform.position = HexCoordinates.ToPosition(hoveredCell.coordinates, -1);
			playerCells[removedPlayerCellIndex].coordinates = hoveredCell.coordinates;

			if (hoveredPlayerCell) {
				Destroy(hoveredPlayerCell.transform.gameObject);
			}

			// set game state
			hoveredCell.setStatus(HexCellStatus.PLAYER);
			hoveredCell.setControlsStatuc(GameControlsStatus.NOTHING);
			furthestCell.setStatus(HexCellStatus.EMPTY);
			furthestCell.setControlsStatuc(GameControlsStatus.NOTHING);
		}
	}
	private void RedrawPlayer() {
		HexCell[] playerHexCells = hexGrid.getCellsByStatus(HexCellStatus.PLAYER);
		for (int i = 0; i < playerHexCells.Length; i++) {
			RedrawPlayerCellFromHexCell(playerHexCells[i]);
		}
	}

	private void RedrawPlayerCellFromHexCell(HexCell hexCell) {
		// TODO: move this finding to a util function
		PlayerCell playerCellToRedraw = playerCells[Array.FindIndex(playerCells, cell => {
			return cell.coordinates.ToString() == hexCell.coordinates.ToString();
		})];
		string playerCellWallCase = "";

		// order: [NE, E, SE, SW, W, NW]
		//		  E
		//	  SE	NE
		//	  SW	NW
		//		  W

		HexCell[] playerHexCellNeighbors = hexCell.GetNeighbors();
		if (
			(hexCell.GetNeighbor(HexDirection.NE).status == HexCellStatus.PLAYER)
			||
			(
				(hexCell.GetNeighbor(HexDirection.NE).controlsStatus == GameControlsStatus.HOVERED)
				&&
				hoveredCellOpensToThisDirection is Enum
				&&
				(HexDirectionExtensions.Opposite(hoveredCellOpensToThisDirection) == HexDirection.NE)
			)
		) {
			playerCellWallCase += "O";
		} else {
			playerCellWallCase += "I";
		}
		
		if (
			(hexCell.GetNeighbor(HexDirection.E).status == HexCellStatus.PLAYER)
			||
			(
				(hexCell.GetNeighbor(HexDirection.E).controlsStatus == GameControlsStatus.HOVERED)
				&&
				hoveredCellOpensToThisDirection is Enum
				&&
				(HexDirectionExtensions.Opposite(hoveredCellOpensToThisDirection) == HexDirection.E)
			)
		) {
			playerCellWallCase += "O";
		} else {
			playerCellWallCase += "I";
		}

		if (
			(hexCell.GetNeighbor(HexDirection.SE).status == HexCellStatus.PLAYER)
			||
			(
				(hexCell.GetNeighbor(HexDirection.SE).controlsStatus == GameControlsStatus.HOVERED)
				&&
				hoveredCellOpensToThisDirection is Enum
				&&
				(HexDirectionExtensions.Opposite(hoveredCellOpensToThisDirection) == HexDirection.SE)
			)
		) {
			playerCellWallCase += "O";
		} else {
			playerCellWallCase += "I";
		}

		if (
			(hexCell.GetNeighbor(HexDirection.SW).status == HexCellStatus.PLAYER)
			||
			(
				(hexCell.GetNeighbor(HexDirection.SW).controlsStatus == GameControlsStatus.HOVERED)
				&&
				hoveredCellOpensToThisDirection is Enum
				&&
				(HexDirectionExtensions.Opposite(hoveredCellOpensToThisDirection) == HexDirection.SW)
			)
		) {
			playerCellWallCase += "O";
		} else {
			playerCellWallCase += "I";
		}

		if (
			(hexCell.GetNeighbor(HexDirection.W).status == HexCellStatus.PLAYER)
			||
			(
				(hexCell.GetNeighbor(HexDirection.W).controlsStatus == GameControlsStatus.HOVERED)
				&&
				hoveredCellOpensToThisDirection is Enum
				&&
				(HexDirectionExtensions.Opposite(hoveredCellOpensToThisDirection) == HexDirection.W)
			)
		) {
			playerCellWallCase += "O";
		} else {
			playerCellWallCase += "I";
		}

		if (
			(hexCell.GetNeighbor(HexDirection.NW).status == HexCellStatus.PLAYER)
			||
			(
				(hexCell.GetNeighbor(HexDirection.NW).controlsStatus == GameControlsStatus.HOVERED)
				&&
				hoveredCellOpensToThisDirection is Enum
				&&
				(HexDirectionExtensions.Opposite(hoveredCellOpensToThisDirection) == HexDirection.NW)
			)
		) {
			playerCellWallCase += "O";
		} else {
			playerCellWallCase += "I";
		}

		playerCellToRedraw.PlayCellWallAnim(playerCellWallCase);
	}
	private void MoveEnemies() {}
	private void DoFight() {}
}
