using UnityEngine;
using System.Collections;

public class Cursoring : MonoBehaviour
{
    public Texture2D cursorTexture;
    public Texture2D holdCursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;

    void Awake() {
        // newcursorTexture = Instantiate (cursorTexture);
	    // newholdCursorTexture = Instantiate (holdCursorTexture);
	    // TextureScale.Bilinear(newcursorTexture, newcursorTexture.width*2, newcursorTexture.height*2);
	    // TextureScale.Bilinear(newholdCursorTexture, newholdCursorTexture.width*2, newholdCursorTexture.height*2);
        SetNormalCursor();
    }

    void Update() {
        if (Input.GetMouseButton(0)) {
            SetHoldCursor();
        } else {
            SetNormalCursor();
        }
    }

    void SetNormalCursor()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }
    void SetHoldCursor()
    {
        Cursor.SetCursor(holdCursorTexture, hotSpot, cursorMode);
    }

}
