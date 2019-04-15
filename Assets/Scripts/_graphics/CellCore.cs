using UnityEngine;

public class CellCore: MonoBehaviour {

    float journeyLength;
    float startTime;
    Vector3 startPos;
    Vector3 endPos;
    float speed = 100f;

	public void MoveCellCore(Vector3 end) {
		// Keep a note of the time the movement started.
		startTime = Time.time;
		startPos = transform.position;
        endPos = end;

		// Calculate the journey length.
		journeyLength = Vector3.Distance(startPos, endPos);
	}
    void Update() {
        if (endPos == new Vector3(0, 0, 0)) {
            return;
        }
	    // Distance moved = time * speed.
	    float distCovered = (Time.time - startTime) * speed;

	    // Fraction of journey completed = current distance divided by total distance.
	    float fracJourney = distCovered / journeyLength;

	    transform.position = Vector3.Slerp(startPos, endPos, fracJourney);
    }
}
