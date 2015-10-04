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
}
