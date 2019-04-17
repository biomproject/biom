using System.Collections.Generic;
using UnityEngine;

public class FirstLevel : MonoBehaviour {
    // this init means nothing, it's set in unity
    public HexCoordinates[] playerCoordinates = { new HexCoordinates(1, 1), new HexCoordinates(1, 2), new HexCoordinates(2, 2) };
    // wallElement 3-27: actual walls, a többi ilyen akadály
    public HexCoordinates[] wallCoordinates = { new HexCoordinates(3, 3), new HexCoordinates(2, 3), new HexCoordinates(3, 4) };

    public HexCoordinates[] enemyCoordinates = {};
}
