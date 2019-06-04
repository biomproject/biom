using System;
using System.Collections;
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
	PlayerCell spawningPlayerCell;
	PlayerCell hoveredPlayerCellForGraphics;
	public CellCore cellCorePrefab;
	CellCore cellCore;
	public Enemy enemyPrefab;
	Enemy[] enemyCells;
	HexDirection hoveredCellOpensToThisDirection;
	HexDirection furthestCellOpensToThisDirection;
	HexDirection spawningCellOpensToThisDirection;
	CellBeats cellBeats;
	Tile[] tiles;
	MenuObject menuObject;

	void Awake () {
		scriptUsageTimeline = GameObject.Find("Music Player").GetComponent<ScriptUsageTimeline>();
		hexGrid = GameObject.Find("Hex Grid").GetComponent<HexGrid>();
		hexMesh = GameObject.Find("Hex Mesh").GetComponent<HexMesh>();
		currentLevel = GameObject.Find("First Level").GetComponent<FirstLevel>();
		playerCells = new PlayerCell[currentLevel.playerCoordinates.Length];
		enemyCells = new Enemy[currentLevel.enemyCoordinates.Length];
		cellBeats = GameObject.Find("Cell Beats").GetComponent<CellBeats>();
		menuObject = GameObject.Find("Menu").GetComponent<MenuObject>();
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
		if (Input.GetKey ("escape")) {
            Application.Quit();
        }

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
		AnimateCellCoreBreath();

		// check if there is a touched cell
		if (!hexGrid.touchedCell) {
			if (hoveredPlayerCell) {
				Destroy(hoveredPlayerCell.transform.gameObject);
			}
			hexGrid.furthestCell = null;
			return;
		}

		// only move if there was a dragging motion
		// HexCell movementStartedHere = Array.Find(hexGrid.getCellsByStatus(HexCellStatus.PLAYER), cell => cell.movementStartedFromThis);
		if (Array.FindIndex(hexGrid.touchedCell.GetNeighbors(), cell => cell.movementStartedFromThis) == -1) {
			return;
		}

		// draw hover and furthest
		if (hexGrid.touchedCell && hexGrid.touchedCell.status == HexCellStatus.EMPTY) {
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

			Tile touchedTile = Array.Find(tiles, tile => tile.coordinates.ToString() == hexGrid.touchedCell.coordinates.ToString());
			touchedTile.FastNudge();

			// only move cellcore if its a player cell 
			HexCell targetCell = hexGrid.touchedCell.GetNeighbor(foundTouchedDirection.Item2);
			if (targetCell.status != HexCellStatus.PLAYER) {
				return;
			}
			// move cellcore
			cellCore.MoveCellCore(HexCoordinates.ToPosition(targetCell.coordinates, -2));
			cellCore.PlayBoiAnim();
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

			Tile touchedTile = Array.Find(tiles, tile => tile.coordinates.ToString() == hexGrid.touchedCell.coordinates.ToString());
			touchedTile.FastNudge();

			// only move cellcore if its a player cell 
			HexCell targetCell = hexGrid.touchedCell.GetNeighbor(foundTouchedDirection.Item2);
			if (targetCell.status != HexCellStatus.PLAYER) {
				return;
			}
			// move cellcore
			cellCore.MoveCellCore(HexCoordinates.ToPosition(targetCell.coordinates, -2));
			cellCore.PlayBoiAnim();
			cellCore.Rotate(foundTouchedDirection.Item1);
		}

		if (!furthestPlayerCell) {
			furthestPlayerCell = Array.Find(playerCells, playerCell => playerCell.coordinates.ToString() == hexGrid.furthestCell.coordinates.ToString());
			furthestPlayerCell.transform.position = HexCoordinates.ToPosition(hexGrid.furthestCell.coordinates, -1);
			furthestPlayerCell.coordinates = hexGrid.furthestCell.coordinates;
			Tuple<int, HexDirection> foundFurthestDirection = PlayerCellWall.FindFurthestRotation(hexGrid.furthestCell);
			furthestCellOpensToThisDirection = foundFurthestDirection.Item2;
			string playerCellWallCase = FindPlayerCellWallCase(hexGrid.furthestCell);
			furthestPlayerCell.PlayDisappearingAnim(foundFurthestDirection.Item1, playerCellWallCase);
		}
		if (furthestPlayerCell && (furthestPlayerCell.coordinates.ToString() != hexGrid.furthestCell.coordinates.ToString())) {
			furthestPlayerCell = Array.Find(playerCells, playerCell => playerCell.coordinates.ToString() == hexGrid.furthestCell.coordinates.ToString());
			furthestPlayerCell.transform.position = HexCoordinates.ToPosition(hexGrid.furthestCell.coordinates, -1);
			furthestPlayerCell.coordinates = hexGrid.furthestCell.coordinates;
			Tuple<int, HexDirection> foundFurthestDirection = PlayerCellWall.FindFurthestRotation(hexGrid.furthestCell);
			furthestCellOpensToThisDirection = foundFurthestDirection.Item2;
			string playerCellWallCase = FindPlayerCellWallCase(hexGrid.furthestCell);
			furthestPlayerCell.PlayDisappearingAnim(foundFurthestDirection.Item1, playerCellWallCase);
		}
	}

	private void AnimateCellCoreBreath() {
		cellCore.ResetRotation();

		if (Input.GetMouseButton(0)) {
			cellCore.PlayBreathInAnim();
		} else if (hexGrid.touchedCell && Array.FindIndex(hexGrid.touchedCell.GetNeighbors(), cell => cell.movementStartedFromThis) != -1) {
			cellCore.PlayBoiAnim();
		} else {
			cellCore.PlayDefaultAnim();
		}
	}

	void DoTurn() {
		MovePlayer();
		RedrawPlayer();
		MoveNotEatenEnemies();

		hexGrid.touchedCell = null;
		hexGrid.furthestCell = null;
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

			for (int i = 0; i < hexGrid.cells.Length; i++) {
				hexGrid.cells[i].movementStartedFromThis = false;
			}
		}
	}
	private void RedrawPlayer() {
		HexCell[] playerHexCells = hexGrid.getCellsByStatus(HexCellStatus.PLAYER);
		for (int i = 0; i < playerHexCells.Length; i++) {
			RedrawPlayerCellFromHexCell(playerHexCells[i]);
		}

		if (hexGrid.hoveredCellForGraphics) {
			if (hexGrid.hoveredCellForGraphics.controlsStatus == GameControlsStatus.FURTHEST) {
				return;
			}

			int index = Array.FindIndex(playerCells, cell => {
				return cell.coordinates.ToString() == hexGrid.hoveredCellForGraphics.coordinates.ToString();
			});

			if (index < 0) {
				hoveredPlayerCellForGraphics = null;
				return;
			}
			if (!hoveredPlayerCellForGraphics || playerCells[index].coordinates.ToString() != hoveredPlayerCellForGraphics.coordinates.ToString()) {
				hoveredPlayerCellForGraphics = playerCells[index];
				hoveredPlayerCellForGraphics.PlayHoveringAnim();
			}
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
		Tuple<int, HexDirection> foundTouchedDirection = PlayerCellWall.FindTouchedRotation(hexGrid.touchedCell);
		spawningCellOpensToThisDirection = foundTouchedDirection.Item2;
		spawningPlayerCell = playerCellToRedraw;

		playerCellToRedraw.PlaySpawningCellWallAnim(playerCellWallCase, foundTouchedDirection.Item1);

		// Until the anim is playing, the spawning cell is not considered a player cell
		hexCell.status = HexCellStatus.SPAWNING;

		StartCoroutine(SetHexCellStatusToPlayer(hexCell, playerCellToRedraw, 0.2f));
	}

	private IEnumerator SetHexCellStatusToPlayer(HexCell hexCell, PlayerCell playerCell, float delay) {
		yield return new WaitForSeconds(delay);
		hexCell.status = HexCellStatus.PLAYER;
		spawningPlayerCell = null;
		playerCell.CloseSpawningCellAnim();
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
		HexCell[] playerHexCellNeighbors = hexCell.GetNeighbors();

		// order: [NE, E, SE, SW, W, NW]
		//		  E
		//	  SE	NE
		//	  SW	NW
		//		  W

		HexDirection[] hexDirectionInOrder = { HexDirection.NE, HexDirection.E, HexDirection.SE, HexDirection.SW, HexDirection.W, HexDirection.NW };

		for (int i = 0; i < hexDirectionInOrder.Length; i++) {
			HexDirection currentHexDirection = hexDirectionInOrder[i];
			if (
				IsNeighborPlayer(currentHexDirection, hexCell) ||
				IsNeighborSpawningAndOpenedInThisDirection(currentHexDirection, hexCell) ||
				// IsNeighborFurthestAndOpenedInThisDirection(currentHexDirection, hexCell) ||
				IsNeighborHoveredAndOpenedInThisDirection(currentHexDirection, hexCell)
			) {
				playerCellWallCase += "O";
			} else {
				playerCellWallCase += "I";
			}
		}

		return playerCellWallCase;
	}

	private bool IsNeighborSpawningAndOpenedInThisDirection(HexDirection currentHexDirection, HexCell hexCell) {
		return spawningPlayerCell &&
			spawningCellOpensToThisDirection is Enum &&
			(hexCell.GetNeighbor(currentHexDirection).coordinates.ToString() == spawningPlayerCell.coordinates.ToString()) &&
			(HexDirectionExtensions.Opposite(spawningCellOpensToThisDirection) == currentHexDirection);
	}

	private bool IsNeighborFurthestAndOpenedInThisDirection(HexDirection currentHexDirection, HexCell hexCell) {
		return furthestPlayerCell &&
			furthestCellOpensToThisDirection is Enum &&
			(hexCell.GetNeighbor(currentHexDirection).coordinates.ToString() == furthestPlayerCell.coordinates.ToString()) &&
			(HexDirectionExtensions.Opposite(furthestCellOpensToThisDirection) == currentHexDirection);
	}

	private bool IsNeighborHoveredAndOpenedInThisDirection(HexDirection currentHexDirection, HexCell hexCell) {
		return hoveredCellOpensToThisDirection is Enum &&
			(hexCell.GetNeighbor(currentHexDirection).controlsStatus == GameControlsStatus.HOVERED) &&
			(HexDirectionExtensions.Opposite(hoveredCellOpensToThisDirection) == currentHexDirection);
	}

	private bool IsNeighborPlayer(HexDirection currentHexDirection, HexCell hexCell) {
		return hexCell.GetNeighbor(currentHexDirection).status == HexCellStatus.PLAYER;
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
