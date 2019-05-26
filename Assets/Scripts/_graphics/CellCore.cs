using UnityEngine;

public class CellCore: MonoBehaviour {

	public HexCoordinates coordinates;
    float journeyLength;
    float startTime;
    Vector3 startPos;
    Vector3 endPos;
    float speed = 100f;
	Animator anim;
	bool breathInPlaying = false;
	bool boiAnimPlaying = false;
	bool moveBoiAnimPlaying = false;
	SlidingSound slidingSound1;
	SlidingSound slidingSound2;
	SlidingSound slidingSound3;
	SlidingSound slidingSound4;
	SlidingSound slidingSound5;

	void Awake() {
		anim = GetComponent<Animator>();
		foreach (Transform child in transform) {
			if (child.name == "Sliding Sound 1") {
				slidingSound1 = child.GetComponent<SlidingSound>();
			} else if (child.name == "Sliding Sound 2") {
				slidingSound2 = child.GetComponent<SlidingSound>();
			} else if (child.name == "Sliding Sound 3") {
				slidingSound3 = child.GetComponent<SlidingSound>();
			} else if (child.name == "Sliding Sound 4") {
				slidingSound4 = child.GetComponent<SlidingSound>();
			} else if (child.name == "Sliding Sound 5") {
				slidingSound5 = child.GetComponent<SlidingSound>();
			}
		}
	}

	public void MoveCellCore(Vector3 end) {
		// Keep a note of the time the movement started.
		startTime = Time.time;
		startPos = transform.position;
        endPos = end;

		// Calculate the journey length.
		journeyLength = Vector3.Distance(startPos, endPos);
		if (journeyLength > 5f) {
			PlayMoveBoiAnim();
			PlayRandomSound();
		}
	}

	void PlayRandomSound() {
		int animNo = UnityEngine.Random.Range(1, 5);
		if (animNo == 1) {
			slidingSound1.PlaySound();
		} else if (animNo == 2) {
			slidingSound2.PlaySound();
		} else if (animNo == 3) {
			slidingSound3.PlaySound();
		} else if (animNo == 4) {
			slidingSound4.PlaySound();
		} else if (animNo == 5) {
			slidingSound5.PlaySound();
		}
	}

    void Update() {
		// distance
        if (endPos == new Vector3(0, 0, 0)) {
            return;
        }
	    // Distance moved = time * speed.
	    float distCovered = (Time.time - startTime) * speed;

	    // Fraction of journey completed = current distance divided by total distance.
	    float fracJourney = distCovered / journeyLength;

	    // transform.position = Vector3.Slerp(startPos, endPos, fracJourney);
	    transform.position = endPos;
    }

	public void PlayBreathInAnim() {
		if (!breathInPlaying) {
			anim.Play("breath_in");
			breathInPlaying = true;
			boiAnimPlaying = false;
		}
	}
	public void PlayMoveBoiAnim() {
		boiAnimPlaying = false;
		moveBoiAnimPlaying = true;
		anim.Play("move_boi");
		Invoke("EndMoveBoiAnim", 0.5f);
	}

	private void EndMoveBoiAnim() {
		moveBoiAnimPlaying = false;
	}

	public void PlayBoiAnim() {
		if (moveBoiAnimPlaying) {
			return;
		}
		boiAnimPlaying = true;
		anim.Play("boi");
	}

	public void PlayDefaultAnim() {
		boiAnimPlaying = false;
		breathInPlaying = false;
		anim.Play("happy_face");
	}
	public void Rotate(int degrees) {
		if (degrees == 120 || degrees == 60) {
			transform.eulerAngles = new Vector3(
				90,
				degrees - 120,
				transform.eulerAngles.z
			);
			return;
		}
		if (degrees == 180 || degrees == 240 || degrees == -60) {
			transform.eulerAngles = new Vector3(
				270,
				degrees - 60,
				transform.eulerAngles.z
			);
			return;
		}
		if (degrees == 0) {
			transform.eulerAngles = new Vector3(
				90,
				degrees - 60,
				transform.eulerAngles.z
			);
			return;
		}
	}

	public void ResetRotation() {
		if (boiAnimPlaying || breathInPlaying || moveBoiAnimPlaying) {
			return;
		}

		Rotate(120);
	}
}
