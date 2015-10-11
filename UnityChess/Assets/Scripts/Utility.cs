using UnityEngine;
using System.Collections;
using System.Text;

public class Utility {

	public static void ArrayToConsole(int[,] arr){
		int rowLength = arr.GetLength(0);
		int colLength = arr.GetLength(1);
		StringBuilder sb = new StringBuilder();
		
		for (int i = 0; i < rowLength; i++)
		{
			for (int j = 0; j < colLength; j++)
			{
				sb.Append(string.Format("{0} ", arr[i, j]));
			}
			sb.Append("\n" + "\n");
		}
		Debug.Log (sb.ToString ());
	}

    public string BoardToFen(int[,] boardLayout, int[,] ownerLayout, int playerTurn)
    {
        // Remember - this function will be passed AFTER the move is made, but BEFORE control is passed to the other player.
        // So we have to flip them (ie. player to go NEXT is white when playerTurn == 2 and black when playerTurn == 2)
        string turnString;
        if (playerTurn == 1)
        {
            turnString = "b";
        } else
        {
            turnString = "w";
        }

        StringBuilder fenBuilder = new StringBuilder();
        for (int r = 0; r < 7; r++)
        {
            for (int c=0; c < 7; c++)
            {
                // TO DO: castling rights not included yet...
                
            }
        }

        return "";
    }


}
