using UnityEngine;
using System.Collections;

public class SquareScript : MonoBehaviour {

	public bool IsOccupied;
	public ChessPiece PieceOnSquare;
	public int RowRef;
	public int ColRef;
	public GameController gameController;
	
	
	// Use this for initialization
	void Start () {
		IsOccupied = false;
		RowRef = (int)-gameObject.transform.position.z;
		ColRef = (int)gameObject.transform.position.x;
		gameObject.name = "a" + RowRef.ToString() + ColRef.ToString();
		gameController = GameObject.Find ("GameController").GetComponent<GameController>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnMouseUp(){

        if (gameController.PieceIsMoving)
        {
            return;
        }
        if (!gameController.InGameMenu.activeSelf)
        {
            // If we aren't mid-move, square is occupied by someone whose turn it is then:
            // - Highlight square and piece
            if (IsOccupied && !gameController.MoveInProgress && PieceOnSquare.PieceOwner == gameController.PlayerTurn)
            {
                // Set "selected" color

                PieceOnSquare.gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Mobile/Bumped Specular");
                gameController.MoveInProgress = true;
                gameController.SquareSelected = gameObject;
                
                int[,] moves = gameController.CalculateMoves(PieceOnSquare.RowRef, PieceOnSquare.ColRef, gameController.BoardLayout, gameController.OwnerLayout);
                // Remove the move to take the king
                if (gameController.PlayerTurn == 1)
                {
                    moves[GameObject.Find("BlackKing(Clone)").GetComponent<ChessPiece>().RowRef, GameObject.Find("BlackKing(Clone)").GetComponent<ChessPiece>().ColRef] = 0;
                }
                else
                {
                    moves[GameObject.Find("WhiteKing(Clone)").GetComponent<ChessPiece>().RowRef, GameObject.Find("WhiteKing(Clone)").GetComponent<ChessPiece>().ColRef] = 0;
                }
                gameController.CurrentPossibleMoves = moves;
                // Deselect this piece
            }
            else if (IsOccupied && gameController.MoveInProgress && PieceOnSquare.PieceOwner == gameController.PlayerTurn && gameObject == gameController.SquareSelected)
            {
                PieceOnSquare.gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
                gameController.MoveInProgress = false;
                gameController.SquareSelected = null;
                gameController.ClearMoves();
            }



            if (gameController.MoveInProgress && gameObject != gameController.SquareSelected)
            {
                if (gameController.CurrentPossibleMoves[RowRef, ColRef] == 1)
                {
                    gameController.TargetSquare = gameObject;
                    int rowFrom = gameController.SquareSelected.GetComponent<SquareScript>().RowRef;
                    int colFrom = gameController.SquareSelected.GetComponent<SquareScript>().ColRef;
                    int rowTo = gameController.TargetSquare.GetComponent<SquareScript>().RowRef;
                    int colTo = gameController.TargetSquare.GetComponent<SquareScript>().ColRef;
                    // Run checkSelf to determine whether it was a valid move (ie "Am I in check after this move?")
                    bool invalidMove = gameController.CheckSelf(rowFrom, colFrom, rowTo, colTo, gameController.BoardLayout, gameController.OwnerLayout);
                    if (!invalidMove)
                    {
                        // Run checkOpp to determine whether opponent is in check as a result of selected move
                        bool inCheck = gameController.CheckOpp(rowFrom, colFrom, rowTo, colTo, gameController.BoardLayout, gameController.OwnerLayout);
                        bool mate = false;
                        if (inCheck)
                        {
                            if (gameController.PlayerTurn == 1)
                            {
                                gameController.PlayerInCheck = 2;
                            }
                            else if (gameController.PlayerTurn == 2)
                            {
                                gameController.PlayerInCheck = 1;
                            }
                            mate = gameController.MateCheck(rowFrom, colFrom, rowTo, colTo, gameController.BoardLayout, gameController.OwnerLayout);
                        }
                        else
                        {
                            gameController.PlayerInCheck = 0;
                        }

                        if (mate)
                        {
                            gameController.GameOver = true;
                        }

                        // Move piece
                        gameController.MovePiece();


                    } else
                    {
                        gameController.TargetSquare = null;
                    }
                }

            }

        }		
	}
	
	
	
}
