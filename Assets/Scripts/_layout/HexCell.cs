using UnityEngine;

public class HexCell : MonoBehaviour {
	public HexCoordinates coordinates;
	int distance;
	public Color color;
	public HexCellStatus status;
	public void setStatus(HexCellStatus _status) {
		status = _status;
		color = StatusColors.GetColor(_status);
	}

	public GameControlsStatus controlsStatus = GameControlsStatus.NOTHING;
	public void setControlsStatuc(GameControlsStatus cStatus) {
		if (cStatus == GameControlsStatus.HOVERED) {
			controlsStatus = GameControlsStatus.HOVERED;
			color = StatusColors.GetColor(controlsStatus);
		} else if (cStatus == GameControlsStatus.FURTHEST) {
			controlsStatus = GameControlsStatus.FURTHEST;
			color = StatusColors.GetColor(controlsStatus);
		} else if (cStatus == GameControlsStatus.NEIGHBOR) {
			controlsStatus = GameControlsStatus.NEIGHBOR;
			color = StatusColors.GetColor(controlsStatus);
		} else {
			controlsStatus = GameControlsStatus.NOTHING;
			/// fix this shit, 2 statuses are a fucking nightmare
			color = StatusColors.GetColor(status);
		}
	}

	public bool movementStartedFromThis = false;

	[SerializeField]
	HexCell[] neighbors;

	public int Distance {
		get {
			return distance;
		}
		set {
			distance = value;
		}
	}

	void Awake() {
		status = HexCellStatus.EMPTY;
	}

	public HexCell GetNeighbor (HexDirection direction) {
		return neighbors[(int)direction];
	}

	public void SetNeighbor (HexDirection direction, HexCell cell) {
		neighbors[(int)direction] = cell;
		cell.neighbors[(int)direction.Opposite()] = this;
	}

	public HexCell[] GetNeighbors() {
		return neighbors;
	}
}
