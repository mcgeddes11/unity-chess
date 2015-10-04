using UnityEngine;
using System.Collections;

public class ChessPiece : MonoBehaviour {

	public int PieceType;
	public int RowRef;
	public int ColRef;
	public float YOffset;
	public int PieceOwner;
	public string PieceName;

	// Use this for initialization
	void Start () {
		
		// Adjust knight rotation
		string pName = gameObject.name.Replace("(Clone)","");
        if (pName.Contains("BlackKnight") && gameObject.transform.parent == null)
        {
            gameObject.transform.Rotate(new Vector3(0, 205, 0));
        } else if (pName.Contains("BlackKnight") && gameObject.transform.parent != null)
        {
            gameObject.transform.Rotate(new Vector3(0, 25, 0));
		} else if (pName.Contains ("WhiteKnight") && gameObject.transform.parent == null) {
			gameObject.transform.Rotate(new Vector3(0,25,0));
		} else if (pName.Contains ("WhiteKnight") && gameObject.transform.parent != null)
        {
            gameObject.transform.Rotate(new Vector3(0,205,0));
        }
		
		// Shift by Y-offset
		Vector3 pos = gameObject.transform.position;
		gameObject.transform.position = new Vector3(pos.x, pos.y + YOffset, pos.z);
		
		
		// Assign col/row ref
		ColRef = (int)gameObject.transform.position.x;
		RowRef = -(int)gameObject.transform.position.z;

        // Set IsOccupied on this square
        var go = GameObject.Find("GameController");
        if (go != null)
        {
            GameObject.Find("a" + RowRef.ToString() + ColRef.ToString()).GetComponent<SquareScript>().IsOccupied = true;
            GameObject.Find("a" + RowRef.ToString() + ColRef.ToString()).GetComponent<SquareScript>().PieceOnSquare = gameObject.GetComponent<ChessPiece>();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
