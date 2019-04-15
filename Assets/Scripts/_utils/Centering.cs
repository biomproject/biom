using UnityEngine;

public static class Centering {
    public static HexCoordinates FindCenter(PlayerCell[] playerCells) {
        return playerCells[UnityEngine.Random.Range(0, playerCells.Length)].coordinates;
    }
}