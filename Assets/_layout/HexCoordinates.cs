using UnityEngine;

[System.Serializable]
public struct HexCoordinates {

	[SerializeField]
	private int x, z;

	public int X {
		get {
			return x;
		}
	}

	public int Z {
		get {
			return z;
		}
	}

	public HexCoordinates (int x, int z) {
		this.x = x;
		this.z = z;
	}

	public static HexCoordinates FromOffsetCoordinates (int x, int z) {
		return new HexCoordinates(x - z / 2, z);
	}

	public int Y {
		get {
			return -X - Z;
		}
	}

	public override string ToString () {
		return "(" + X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ")";
	}

	public string ToStringOnSeparateLines () {
		return X.ToString() + "\n" + Y.ToString() + "\n" + Z.ToString();
	}

	public static HexCoordinates FromPosition (Vector3 position) {
		float z = position.z / (HexMetrics.innerRadius * 2f);
		float y = -z;

		float offset = position.x / (HexMetrics.outerRadius * 3f);
		z -= offset;
		y -= offset;

		int iX = Mathf.RoundToInt(-z -y);
		int iY = Mathf.RoundToInt(y);
		int iZ = Mathf.RoundToInt(z);

		if (iX + iY + iZ != 0) {
			float dZ = Mathf.Abs(z - iZ);
			float dY = Mathf.Abs(y - iY);
			float dX = Mathf.Abs(-z -y - iX);

			if (dZ > dY && dZ > dX) {
				iZ = -iY - iX;
			}
			else if (dX > dY) {
				iX = -iZ - iY;
			}
		}

		return new HexCoordinates(iZ, iX);
	}

	public static Vector3 ToPosition (HexCoordinates hexCoordinates, float y = 5) {
		float verticalHexCenterDistance = HexMetrics.innerRadius * 2f;
		float horizontalHexCenterDistance = HexMetrics.outerRadius * 1.5f;
		float pZ = (hexCoordinates.Z * verticalHexCenterDistance) +
			(hexCoordinates.X * HexMetrics.innerRadius);
		float pX = hexCoordinates.X * horizontalHexCenterDistance;

		return new Vector3(pX, y, pZ);
	}

	public int DistanceTo (HexCoordinates other) {
		return
			((x < other.x ? other.x - x : x - other.x) +
			(Y < other.Y ? other.Y - Y : Y - other.Y) +
			(z < other.z ? other.z - z : z - other.z)) / 2;
	}

}
