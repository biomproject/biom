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
	private int previousOffbeat = 0;

	public PlayerCell playerCellPrefab;
	public Tile tilePrefab;
	PlayerCell[] playerCells;
	PlayerCell hoveredPlayerCell;
	PlayerCell furthestPlayerCell;
	public CellCore cellCorePrefab;
	CellCore cellCore;
	public Enemy enemyPrefab;
	Enemy[] enemyCells;
	HexDirection hoveredCellOpensToThisDirection;
	HexDirection furthestCellOpensToThisDirection;
	CellBeats cellBeats;
	Tile[] tiles;

	void Awake () {
		scriptUsageTimeline = GameObject.Find("Music Player").GetComponent<ScriptUsageTimeline>();
		hexGrid = GameObject.Find("Hex Grid").GetComponent<HexGrid>();
		hexMesh = GameObject.Find("Hex Mesh").GetComponent<HexMesh>();
		currentLevel = GameObject.Find("First Level").GetComponent<FirstLevel>();
		playerCells = new PlayerCell[currentLevel.playerCoordinates.Length];
		enemyCells = new Enemy[currentLevel.enemyCoordinates.Length];
		cellBeats = GameObject.Find("Cell Beats").GetComponent<CellBeats>();
	}

	void Start () {
		InitPlayer();
		InitTiles();
		InitEnemies();
	}

	void InitPlayer() {
		for(int i = 0; i < currentLevel.playerCoordinates.Length; i++) {
			playerCells[i] = Instantiate<PlayerCell>(playerCellPrefab);
			playerCells[i].transform.position = HexCoordinates.ToPosition(currentLevel.playerCoordinates[i], -1);
			playerCells[i].coordinates = currentLevel.playerCoordinates[i];
		}
		cellCore = Instantiate<CellCore>(cellCorePrefab);
		cellCore.transform.position = HexCoordinates.ToPosition(Centering.FindCenter(playerCells), -2);
	}

	void InitTiles() {
		HexCell[] emptyCells = hexGrid.getCellsByNotStatus(HexCellStatus.WALL);
		tiles = new Tile[emptyCells.Length];
		for (int i = 0; i < emptyCells.Length; i++) {
			Tile tile = Instantiate<Tile>(tilePrefab);
			tile.transform.position = HexCoordinates.ToPosition(emptyCells[i].coordinates, -5);
			tile.PlayRandomAnim();
			tile.coordinates = emptyCells[i].coordinates;
			tiles[i] = tile;
		}
	}

	void InitEnemies() {
		for(int i = 0; i < currentLevel.enemyCoordinates.Length; i++) {
			enemyCells[i] = Instantiate<Enemy>(enemyPrefab);
			enemyCells[i].transform.position = HexCoordinates.ToPosition(currentLevel.enemyCoordinates[i], -1);
			enemyCells[i].coordinates = currentLevel.enemyCoordinates[i];
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
		if (previousOffbeat != scriptUsageTimeline.timelineInfo.currentOffbeatBar) {
			previousOffbeat = scriptUsageTimeline.timelineInfo.currentOffbeatBar;
			DoFight();
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
			if (hoveredPlayerCell) {
				Destroy(hoveredPlayerCell.transform.gameObject);
			}
			return;
		}

		// check if touchedCell is a legal move
		bool isLegal = false;
		HexCell[] playerHexCells = hexGrid.getCellsByStatus(HexCellStatus.PLAYER);
		for (int i = 0; i < playerHexCells.Length; i++) {
			if (Array.IndexOf(playerHexCells[i].GetNeighbors(), hexGrid.touchedCell) > -1) {
				isLegal = true;
			}
		}
		if(!isLegal) {
			return;
		}

		// draw hover and furthest
		if (hexGrid.touchedCell && Input.GetMouseButton(0) && hexGrid.touchedCell.status == HexCellStatus.EMPTY) {
			hexGrid.touchedCell.setControlsStatuc(GameControlsStatus.HOVERED);
			hexGrid.furthestCell = Distance.getFurthestPathCell(hexGrid.touchedCell, hexGrid.getCellsByStatus(HexCellStatus.PLAYER));
			hexGrid.furthestCell.setControlsStatuc(GameControlsStatus.FURTHEST);
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
			// return;

			// move cellcore
			cellCore.MoveCellCore(HexCoordinates.ToPosition(hexGrid.touchedCell.GetNeighbor(foundTouchedDirection.Item2).coordinates, -2));
			cellCore.PlayBreathInBoiAnim();
			cellCore.Rotate(foundTouchedDirection.Item1);
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

			// move cellcore
			cellCore.MoveCellCore(HexCoordinates.ToPosition(hexGrid.touchedCell.GetNeighbor(foundTouchedDirection.Item2).coordinates, -2));
			cellCore.PlayBreathInBoiAnim();
			cellCore.Rotate(foundTouchedDirection.Item1);
		}

		if (!furthestPlayerCell) {
			furthestPlayerCell = Array.Find(playerCells, playerCell => playerCell.coordinates.ToString() == hexGrid.furthestCell.coordinates.ToString());
			furthestPlayerCell.transform.position = HexCoordinates.ToPosition(hexGrid.furthestCell.coordinates, -1);
			furthestPlayerCell.coordinates = hexGrid.furthestCell.coordinates;
			Tuple<int, HexDirection> foundFurthestDirection = PlayerCellWall.FindFurthestRotation(hexGrid.furthestCell);
			furthestCellOpensToThisDirection = foundFurthestDirection.Item2;
			furthestPlayerCell.RotateWall(foundFurthestDirection.Item1);
			furthestPlayerCell.PlayDisappearingAnim();
		}
		if (furthestPlayerCell && (furthestPlayerCell.coordinates.ToString() != hexGrid.furthestCell.coordinates.ToString())) {
			furthestPlayerCell = Array.Find(playerCells, playerCell => playerCell.coordinates.ToString() == hexGrid.furthestCell.coordinates.ToString());
			furthestPlayerCell.transform.position = HexCoordinates.ToPosition(hexGrid.furthestCell.coordinates, -1);
			furthestPlayerCell.coordinates = hexGrid.furthestCell.coordinates;
			Tuple<int, HexDirection> foundFurthestDirection = PlayerCellWall.FindFurthestRotation(hexGrid.furthestCell);
			furthestCellOpensToThisDirection = foundFurthestDirection.Item2;
			furthestPlayerCell.RotateWall(foundFurthestDirection.Item1);
			furthestPlayerCell.PlayDisappearingAnim();
		}
	}

	void DoTurn() {
		MovePlayer();
		RedrawPlayer();
		MoveNotEatenEnemies();
	}

	private void MovePlayer() {
		HexCell[] hoveredCells = hexGrid.getCellsByStatus(GameControlsStatus.HOVERED);
		HexCell[] furthestCells = hexGrid.getCellsByStatus(GameControlsStatus.FURTHEST);
		if (hoveredCells.Length != 0 && furthestCells.Length != 0) {
			HexCell hoveredCell = hoveredCells[0];
			HexCell furthestCell = furthestCells[0];

			// create/destroy player and enemy objects
			int removedPlayerCellIndex = Array.FindIndex(playerCells, cell => {
				return cell.coordinates.ToString() == furthestCell.coordinates.ToString();
			});
			Enemy enemyToMove = playerCells[removedPlayerCellIndex].isEating;
			if (enemyToMove) {
				HexCell playerCellHex = Array.Find(hexGrid.cells, cell => cell.coordinates.ToString() == playerCells[removedPlayerCellIndex].coordinates.ToString());
				HexCell[] possiblePositions = playerCellHex.GetNeighbors();
				HexCell target = Array.Find(possiblePositions, cell => {
					return cell.status == HexCellStatus.PLAYER;
				});
				enemyToMove.ChangePosition(target.coordinates);
				enemyToMove.SetHp(enemyToMove.hp);
			}

			Destroy(playerCells[removedPlayerCellIndex].transform.gameObject);
	
			playerCells[removedPlayerCellIndex] = Instantiate<PlayerCell>(playerCellPrefab);
			playerCells[removedPlayerCellIndex].transform.position = HexCoordinates.ToPosition(hoveredCell.coordinates, -1);
			playerCells[removedPlayerCellIndex].coordinates = hoveredCell.coordinates;

			if (enemyToMove) {
				playerCells[removedPlayerCellIndex].isEating = enemyToMove;
				enemyToMove.beingEatenBy = playerCells[removedPlayerCellIndex];
			}

			if (hoveredPlayerCell) {
				Destroy(hoveredPlayerCell.transform.gameObject);
			}

			// set game state
			hoveredCell.setStatus(HexCellStatus.PLAYER);
			hoveredCell.setControlsStatuc(GameControlsStatus.NOTHING);
			furthestCell.setStatus(HexCellStatus.EMPTY);
			furthestCell.setControlsStatuc(GameControlsStatus.NOTHING);

			cellCore.PlayDefaultAnim();

			RedrawSpawningPlayerCellFromHexCell(hoveredCell);

			cellBeats.MoveWithCenter(hoveredCell.coordinates, tiles, hexGrid.getCellsByNotStatus(HexCellStatus.WALL));
		}
	}
	private void RedrawPlayer() {
		HexCell[] playerHexCells = hexGrid.getCellsByStatus(HexCellStatus.PLAYER);
		for (int i = 0; i < playerHexCells.Length; i++) {
			RedrawPlayerCellFromHexCell(playerHexCells[i]);
		}
	}

	private void RedrawSpawningPlayerCellFromHexCell(HexCell hexCell) {
		// TODO: move this finding to a util function
		PlayerCell playerCellToRedraw = playerCells[Array.FindIndex(playerCells, cell => {
			return cell.coordinates.ToString() == hexCell.coordinates.ToString();
		})];
		if (furthestPlayerCell && (playerCellToRedraw.coordinates.ToString() == furthestPlayerCell.coordinates.ToString())) {
			return;
		}

		string playerCellWallCase = FindPlayerCellWallCase(hexCell);
		playerCellToRedraw.PlaySpawningCellWallAnim(playerCellWallCase);
	}

	private void RedrawPlayerCellFromHexCell(HexCell hexCell) {
		// TODO: move this finding to a util function
		PlayerCell playerCellToRedraw = playerCells[Array.FindIndex(playerCells, cell => {
			return cell.coordinates.ToString() == hexCell.coordinates.ToString();
		})];
		if (furthestPlayerCell && (playerCellToRedraw.coordinates.ToString() == furthestPlayerCell.coordinates.ToString())) {
			return;
		}

		string playerCellWallCase = FindPlayerCellWallCase(hexCell);
		playerCellToRedraw.PlayCellWallAnim(playerCellWallCase);
	}

	private string FindPlayerCellWallCase(HexCell hexCell) {
		string playerCellWallCase = "";

		// order: [NE, E, SE, SW, W, NW]
		//		  E
		//	  SE	NE
		//	  SW	NW
		//		  W

		HexCell[] playerHexCellNeighbors = hexCell.GetNeighbors();
		if (
			!(furthestPlayerCell && (hexCell.GetNeighbor(HexDirection.NE).coordinates.ToString() == furthestPlayerCell.coordinates.ToString()) && (furthestCellOpensToThisDirection is Enum && HexDirectionExtensions.Opposite(furthestCellOpensToThisDirection) != HexDirection.NE))
			&&
			((hexCell.GetNeighbor(HexDirection.NE).status == HexCellStatus.PLAYER)
			||
			(
				(hexCell.GetNeighbor(HexDirection.NE).controlsStatus == GameControlsStatus.HOVERED)
				&&
				hoveredCellOpensToThisDirection is Enum
				&&
				(HexDirectionExtensions.Opposite(hoveredCellOpensToThisDirection) == HexDirection.NE)
			))
		) {
			playerCellWallCase += "O";
		} else {
			playerCellWallCase += "I";
		}
		
		if (
			!(furthestPlayerCell && (hexCell.GetNeighbor(HexDirection.E).coordinates.ToString() == furthestPlayerCell.coordinates.ToString()) && (furthestCellOpensToThisDirection is Enum && HexDirectionExtensions.Opposite(furthestCellOpensToThisDirection) != HexDirection.E))
			&&
			((hexCell.GetNeighbor(HexDirection.E).status == HexCellStatus.PLAYER)
			||
			(
				(hexCell.GetNeighbor(HexDirection.E).controlsStatus == GameControlsStatus.HOVERED)
				&&
				hoveredCellOpensToThisDirection is Enum
				&&
				(HexDirectionExtensions.Opposite(hoveredCellOpensToThisDirection) == HexDirection.E)
			))
		) {
			playerCellWallCase += "O";
		} else {
			playerCellWallCase += "I";
		}

		if (
			!(furthestPlayerCell && (hexCell.GetNeighbor(HexDirection.SE).coordinates.ToString() == furthestPlayerCell.coordinates.ToString()) && (furthestCellOpensToThisDirection is Enum && HexDirectionExtensions.Opposite(furthestCellOpensToThisDirection) != HexDirection.SE))
			&&
			((hexCell.GetNeighbor(HexDirection.SE).status == HexCellStatus.PLAYER)
			||
			(
				(hexCell.GetNeighbor(HexDirection.SE).controlsStatus == GameControlsStatus.HOVERED)
				&&
				hoveredCellOpensToThisDirection is Enum
				&&
				(HexDirectionExtensions.Opposite(hoveredCellOpensToThisDirection) == HexDirection.SE)
			))
		) {
			playerCellWallCase += "O";
		} else {
			playerCellWallCase += "I";
		}

		if (
			!(furthestPlayerCell && (hexCell.GetNeighbor(HexDirection.SW).coordinates.ToString() == furthestPlayerCell.coordinates.ToString()) && (furthestCellOpensToThisDirection is Enum && HexDirectionExtensions.Opposite(furthestCellOpensToThisDirection) != HexDirection.SW))
			&&
			((hexCell.GetNeighbor(HexDirection.SW).status == HexCellStatus.PLAYER)
			||
			(
				(hexCell.GetNeighbor(HexDirection.SW).controlsStatus == GameControlsStatus.HOVERED)
				&&
				hoveredCellOpensToThisDirection is Enum
				&&
				(HexDirectionExtensions.Opposite(hoveredCellOpensToThisDirection) == HexDirection.SW)
			))
		) {
			playerCellWallCase += "O";
		} else {
			playerCellWallCase += "I";
		}

		if (
			!(furthestPlayerCell && (hexCell.GetNeighbor(HexDirection.W).coordinates.ToString() == furthestPlayerCell.coordinates.ToString()) && (furthestCellOpensToThisDirection is Enum && HexDirectionExtensions.Opposite(furthestCellOpensToThisDirection) != HexDirection.W))
			&&
			((hexCell.GetNeighbor(HexDirection.W).status == HexCellStatus.PLAYER)
			||
			(
				(hexCell.GetNeighbor(HexDirection.W).controlsStatus == GameControlsStatus.HOVERED)
				&&
				hoveredCellOpensToThisDirection is Enum
				&&
				(HexDirectionExtensions.Opposite(hoveredCellOpensToThisDirection) == HexDirection.W)
			))
		) {
			playerCellWallCase += "O";
		} else {
			playerCellWallCase += "I";
		}

		if (
			!(furthestPlayerCell && (hexCell.GetNeighbor(HexDirection.NW).coordinates.ToString() == furthestPlayerCell.coordinates.ToString()) && (furthestCellOpensToThisDirection is Enum && HexDirectionExtensions.Opposite(furthestCellOpensToThisDirection) != HexDirection.NW))
			&&
			((hexCell.GetNeighbor(HexDirection.NW).status == HexCellStatus.PLAYER)
			||
			(
				(hexCell.GetNeighbor(HexDirection.NW).controlsStatus == GameControlsStatus.HOVERED)
				&&
				hoveredCellOpensToThisDirection is Enum
				&&
				(HexDirectionExtensions.Opposite(hoveredCellOpensToThisDirection) == HexDirection.NW)
			))
		) {
			playerCellWallCase += "O";
		} else {
			playerCellWallCase += "I";
		}

		return playerCellWallCase;
	}

	private void MoveNotEatenEnemies() {
		for(int i = 0; i < currentLevel.enemyCoordinates.Length; i++) {
			if (enemyCells[i].isEaten) {
				return;
			}
			HexCell currentEnemyCell = Array.Find(hexGrid.cells, cell => cell.coordinates.ToString() == enemyCells[i].coordinates.ToString());
			HexCoordinates target = currentEnemyCell.GetNeighbor(HexDirection.SW).coordinates;
			enemyCells[i].ChangePosition(target);
			enemyCells[i].coordinates = target;
		}
	}
	private void DoFight() {
		for(int i = 0; i < currentLevel.enemyCoordinates.Length; i++) {
			if (enemyCells[i].hp < 1) {
				return;
			}
			PlayerCell cellEating = Array.Find(playerCells, cell => cell.coordinates.ToString() == enemyCells[i].coordinates.ToString());
			if (!cellEating) {
				return;
			}
			enemyCells[i].isEaten = true;
			enemyCells[i].SetHp(enemyCells[i].hp - 1);
			enemyCells[i].beingEatenBy = cellEating;
			cellEating.isEating = enemyCells[i];
			Debug.Log(enemyCells[i].hp);
		}
	}
}
