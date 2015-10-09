using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public class GameController : MonoBehaviour {
	public Rigidbody WhiteSquare;
	public Rigidbody BlackSquare;

	public Rigidbody BlackPawn;
	public Rigidbody BlackRook;
	public Rigidbody BlackKnight;
	public Rigidbody BlackBishop;
	public Rigidbody BlackQueen;
	public Rigidbody BlackKing;
	public Rigidbody WhitePawn;
	public Rigidbody WhiteRook;
	public Rigidbody WhiteKnight;
	public Rigidbody WhiteBishop;
	public Rigidbody WhiteQueen;
	public Rigidbody WhiteKing;
	

	public int[,] BoardLayout;
    public int[,] OwnerLayout;
	public int PlayerTurn;	
	public bool MoveInProgress;
	public int[,] CurrentPossibleMoves;
	public GameObject SquareSelected;
	public GameObject TargetSquare;
    public int PlayerInCheck;
    public bool GameOver;
    public int MoveCounter1;
    public int MoveCounter2;

    public GameObject InGameMenu;
    public GameConfigData GameConfig;

	public Ray MouseOverRay;
	public RaycastHit MouseOverRayHit;
	public bool DidMouseOverOccurThisFrame;
	public GameObject HighlightedSquare;

	// Use this for initialization
	void Start () {
		// Instantiate board
		InitializeDefaults();
		InstantiateBoard ();
		InstantiatePieces();
		ClearMoves();
		PlayerTurn = 1;
		MoveInProgress = false;
        GameConfig = GameObject.Find("GameConfigData").GetComponent<GameConfigData>();
    }
	
	// Update is called once per frame
	void Update () {
        // If in game menu is active, don't do anything
        if (!InGameMenu.activeSelf)
        {
            // Do the square/piece highlighting each frame as mouse moves
            MouseOverRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(MouseOverRay, out MouseOverRayHit))
            {
                if (MouseOverRayHit.collider.gameObject != HighlightedSquare)
                {
                    HighlightedSquare.GetComponent<Renderer>().material.shader = Shader.Find("Standard");
                    HighlightedSquare = MouseOverRayHit.collider.gameObject;
                    MouseOverRayHit.collider.gameObject.GetComponent<Renderer>().material.shader = Shader.Find("Mobile/Bumped Specular");
                }
            }

            // Open main menu if esc is pressed
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                InGameMenu.SetActive(true);
            }

        }
        else
        {

            // Respond to buttons on in-game menu
            GameObject mainMenuButton = GameObject.Find("Main Menu");
            GameObject quitButton = GameObject.Find("Quit to Desktop");
            // GameObject saveButton = GameObject.Find("SaveGame");

            if (mainMenuButton.GetComponent<InstantGuiButton>().pressed)
            {
                Application.LoadLevel("MainMenu");
            }

            if (quitButton.GetComponent<InstantGuiButton>().pressed)
            {
                Application.Quit();
            }
            // Close main menu if escape is pressed
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                InGameMenu.SetActive(false);
            }

        }

    }

	
	
	void InstantiateBoard(){
		// Populate board squares
		for (int ii = 0; ii < 64; ii++){
			// Col ref
			int colRef = (ii % 8);
			// Row ref
			int rowRef = (ii / 8);
			// If rowRef + colRef is odd, black, else white
			if ((rowRef + colRef) % 2 > 0) {
				Instantiate (BlackSquare, new Vector3(rowRef,0.2f,-colRef), this.transform.rotation);
			} else {
				Instantiate (WhiteSquare, new Vector3(rowRef,0.2f,-colRef), this.transform.rotation);
			}
		}
		
	
	}
	
	void InitializeDefaults(){
		// Set default value for board layout
		BoardLayout = new int[8,8];
        OwnerLayout = new int[8, 8];
        GameOver = false;
	}
	
	void InstantiatePieces(){
	

		// Black Pieces
		PopulateSquare(0,0, BlackRook);
		PopulateSquare(0,7, BlackRook);
		PopulateSquare(0,1, BlackKnight);
		PopulateSquare(0,6, BlackKnight);
		PopulateSquare(0,2, BlackBishop);
		PopulateSquare(0,5, BlackBishop);
		PopulateSquare(0,3, BlackQueen);
		PopulateSquare(0,4, BlackKing);
		for (int col = 0; col < 8; col++){
			PopulateSquare(1,col, BlackPawn);
		}
		
		
		// White Pieces
		PopulateSquare(7,0, WhiteRook);
		PopulateSquare(7,7, WhiteRook);
		PopulateSquare(7,1, WhiteKnight);
		PopulateSquare(7,6, WhiteKnight);
		PopulateSquare(7,2, WhiteBishop);
		PopulateSquare(7,5, WhiteBishop);
		PopulateSquare(7,3, WhiteQueen);
		PopulateSquare(7,4, WhiteKing);
		for (int col = 0; col < 8; col++){
			PopulateSquare(6,col, WhitePawn);
		}

	}
	
	void PopulateSquare(int row, int col, Rigidbody obj){
		Vector3 p = new Vector3(col, 0, -row);
		Instantiate(obj, p, GameObject.Find ("Board").transform.rotation);
		BoardLayout[row,col] = obj.gameObject.GetComponent<ChessPiece>().PieceType;
        OwnerLayout[row, col] = obj.gameObject.GetComponent<ChessPiece>().PieceOwner;
    }
	
	public int[,] CalculateMoves(int row, int col, int[,] boardLayout, int[,] ownerLayout){
        int[,] moves = new int[8,8];
        int pieceType = boardLayout[row, col];
        switch (pieceType){
			case 1:
			    moves = PawnMoves (row, col, boardLayout, ownerLayout);
				break;
			case 2:
                moves = RookMoves(row, col, boardLayout, ownerLayout);
				break;
			case 3:
                moves = KnightMoves(row, col, boardLayout, ownerLayout);
				break;
			case 4:
                moves = BishopMoves(row, col, boardLayout, ownerLayout);
				break;
			case 5:
                moves = QueenMoves(row, col, boardLayout, ownerLayout);
				break;
			case 6:
                moves = KingMoves(row, col, boardLayout, ownerLayout);
				break;
		
		}
        return moves;
	}
	
	public int[,] PawnMoves(int row, int col, int[,] boardLayout, int[,] ownerLayout)
    {
        int[,] moves = new int[8,8];
        int pieceOwner = ownerLayout[row, col];
        // White
        if (pieceOwner == 1){
			// Standard and start moves
			if (boardLayout[row-1, col] == 0){
				moves[row-1,col] = 1;
			}
			if (row == 6 && boardLayout[row-1, col] == 0 && boardLayout[row-2, col] == 0){
				moves[row-2,col] = 1;
			}
			// Capturing
			if (col != 7){
				if (boardLayout[row-1,col+1] != 0 && ownerLayout[row-1,col+1] != pieceOwner){
					moves[row-1,col+1] = 1;
				}
			}
			
			if (col !=0) {
				if (boardLayout[row-1,col-1] != 0 && ownerLayout[row-1,col-1] != pieceOwner){
					moves[row-1,col-1] = 1;
				}
			}			
			
			// Black
		} else {
			// Standard and start moves
			if (boardLayout[row+1, col] == 0){
				moves[row+1,col] = 1;
			}
			if (row == 1 && boardLayout[row+1, col] == 0 && boardLayout[row+2, col] == 0){
				moves[row+2,col] = 1;
			}
			// Capturing
			if (col != 7){
				if (boardLayout[row+1,col+1] != 0 && ownerLayout[row+1,col+1] != pieceOwner){
					moves[row+1,col+1] = 1;
				}
			}
			
			if (col !=0) {
				if (boardLayout[row+1,col-1] != 0 && ownerLayout[row+1,col-1]!= pieceOwner){
					moves[row+1,col-1] = 1;
				}
			}
			
		}
        return moves;
	}
	
	public int[,] BishopMoves(int row, int col, int[,] boardLayout, int[,] ownerLayout)
    {
        int[,] moves = new int[8,8];
        int pieceOwner = ownerLayout[row, col];
        int iRow;
		int iCol;
		// Up-right
		iRow = row - 1;
		iCol = col + 1;
		while( iRow < 8 && iRow >= 0 && iCol < 8 && iCol >= 0 && (boardLayout[iRow,iCol] == 0 || (boardLayout[iRow,iCol] != 0 && ownerLayout[iRow,iCol] != pieceOwner))){
			moves[iRow,iCol] = 1;
			if (boardLayout[iRow,iCol] != 0 && ownerLayout[iRow,iCol] != pieceOwner){
				break;
			}
			iRow--;
			iCol++;
		}
		
		// Up-left
		iRow = row - 1;
		iCol = col - 1;
		while( iRow < 8 && iRow >= 0 && iCol < 8 && iCol >= 0 && (boardLayout[iRow,iCol] == 0 || (boardLayout[iRow,iCol] != 0 && ownerLayout[iRow,iCol] != pieceOwner))){
			moves[iRow,iCol] = 1;
			if (boardLayout[iRow,iCol] != 0 && ownerLayout[iRow,iCol] != pieceOwner)
            {
				break;
			}
			iRow--;
			iCol--;
		}
		
		// Down-Right
		iRow = row + 1;
		iCol = col + 1;
		while( iRow < 8 && iRow >= 0 && iCol < 8 && iCol >= 0 && (boardLayout[iRow,iCol] == 0 || (boardLayout[iRow,iCol] != 0 && ownerLayout[iRow,iCol] != pieceOwner))){
			moves[iRow,iCol] = 1;
			if (boardLayout[iRow,iCol] != 0 && ownerLayout[iRow,iCol] != pieceOwner)
            {
				break;
			}
			iRow++;
			iCol++;
		}
		
		// Down-Left
		iRow = row + 1;
		iCol = col - 1;
		while( iRow < 8 && iRow >= 0 && iCol < 8 && iCol >= 0 && (boardLayout[iRow,iCol] == 0 || (boardLayout[iRow,iCol] != 0 && ownerLayout[iRow,iCol] != pieceOwner))){
			moves[iRow,iCol] = 1;
			if (boardLayout[iRow,iCol] != 0 && ownerLayout[iRow,iCol] != pieceOwner)
            {
				break;
			}
			iRow++;
			iCol--;
		}

        return moves;
	}
	
	public int[,] KnightMoves(int row, int col, int[,] boardLayout, int[,] ownerLayout)
    {
        int[,] moves = new int[8,8];
        int pieceOwner = ownerLayout[row, col];
        for (int ii=0; ii < 8; ii++){
			// Enumerate eight possible moves
			for (int jj=0; jj < 8; jj++) {
				if ((ii == row + 2 && jj == col-1 && ((boardLayout[row + 2,col-1] != 0 && ownerLayout[row + 2,col-1] != pieceOwner) || (boardLayout[row+2, col-1] == 0) )) ||
				    (ii == row + 2 && jj == col+1 && ((boardLayout[row + 2,col+1] != 0 && ownerLayout[row + 2,col+1] != pieceOwner) || (boardLayout[row+2, col+1] == 0) )) ||
				    (ii == row - 2 && jj == col-1 && ((boardLayout[row - 2,col-1] != 0 && ownerLayout[row - 2,col-1] != pieceOwner) || (boardLayout[row-2, col-1] == 0) )) ||
				    (ii == row - 2 && jj == col+1 && ((boardLayout[row - 2,col+1] != 0 && ownerLayout[row - 2,col+1] != pieceOwner) || (boardLayout[row-2, col+1] == 0) )) ||
				    (ii == row + 1 && jj == col-2 && ((boardLayout[row + 1,col-2] != 0 && ownerLayout[row + 1,col-2] != pieceOwner) || (boardLayout[row+1, col-2] == 0) )) ||
				    (ii == row + 1 && jj == col+2 && ((boardLayout[row + 1,col+2] != 0 && ownerLayout[row + 1,col+2] != pieceOwner) || (boardLayout[row+1, col+2] == 0) )) ||
				    (ii == row - 1 && jj == col-2 && ((boardLayout[row - 1,col-2] != 0 && ownerLayout[row - 1,col-2] != pieceOwner) || (boardLayout[row-1, col-2] == 0) )) ||
				    (ii == row - 1 && jj == col+2 && ((boardLayout[row - 1,col+1] != 0 && ownerLayout[row - 1,col+1] != pieceOwner) || (boardLayout[row-1, col+2] == 0) )))
				    {
						moves[ii,jj] = 1;
				    }
			}
			
		}
        return moves;
		

	}
	
	public int[,] RookMoves(int row, int col, int[,] boardLayout, int[,] ownerLayout)
    {
        int[,] moves = new int[8,8];
        int pieceOwner = ownerLayout[row, col];
        int iRow;
		int iCol;
		// Up
		iRow = row - 1;
		while( iRow < 8 && iRow >= 0 && (boardLayout[iRow,col] == 0 || (boardLayout[iRow,col] != 0 && ownerLayout[iRow,col] != pieceOwner))){
			moves[iRow,col] = 1;
			if (boardLayout[iRow,col] != 0 && ownerLayout[iRow,col] != pieceOwner)
            {
				break;
			}
			iRow--;
		}
		
		// Down
		iRow = row + 1;
		while( iRow < 8 && iRow >= 0 && (boardLayout[iRow,col] == 0 || (boardLayout[iRow,col] != 0 && ownerLayout[iRow,col] != pieceOwner))){
			moves[iRow,col] = 1;
			if (boardLayout[iRow,col] != 0 && ownerLayout[iRow,col] != pieceOwner)
            {
				break;
			}
			iRow++;
		}
		
		// Left
		iCol = col - 1;
		while( iCol < 8 && iCol >= 0 && (boardLayout[row,iCol] == 0 || (boardLayout[row,iCol] != 0 && ownerLayout[row,iCol] != pieceOwner))){
			moves[row,iCol] = 1;
			if (boardLayout[row,iCol] != 0 && ownerLayout[row,iCol] != pieceOwner)
            {
				break;
			}
			iCol--;
		}		
		
		// Right
		iCol = col + 1;
		while( iCol < 8 && iCol >= 0 && (boardLayout[row,iCol] == 0 || (boardLayout[row,iCol] != 0 && ownerLayout[row,iCol] != pieceOwner))){
			moves[row,iCol] = 1;
			if (boardLayout[row,iCol] != 0 && OwnerLayout[row,iCol] != pieceOwner)
            {
				break;
			}
			iCol++;
		}
        return moves;
	}
	
	public int[,] QueenMoves(int row, int col, int[,] boardLayout, int[,] ownerLayout)
    {
        int[,] moves;
	// Combine rook and bishop
		int[,] bMoves = BishopMoves(row, col, boardLayout, ownerLayout);
		int[,] rMoves = RookMoves (row, col, boardLayout, ownerLayout);
        moves = ElementwiseIf(bMoves, rMoves);
        return moves;
	}
	
	public int[,] KingMoves(int row, int col, int[,] boardLayout, int[,] ownerLayout)
    {
        int[,] moves = new int[8, 8];
        // Start with queen, remove any further than 1 sqaure
        moves = QueenMoves (row, col, boardLayout, ownerLayout);
		for (int iRow = 0; iRow < 8; iRow++) {
			for (int iCol = 0; iCol < 8; iCol++){
				if (iRow > row + 1 || iCol > col + 1){
					moves[iRow,iCol] = 0;
				}
			}
		}
        return moves;
	}
	
	public void ClearMoves(){
		// Set default value for moves
		CurrentPossibleMoves = new int[8,8];
		for (int ii = 0; ii < 8; ii++){
			for (int jj = 0; jj < 8; jj++){
				CurrentPossibleMoves[ii,jj] = 0;
			}
		}	
	}	
	
	public void MovePiece(){
		ChessPiece pieceToMove = SquareSelected.GetComponent<SquareScript>().PieceOnSquare;
		Vector3 origin = pieceToMove.transform.position;
		Vector3 destination = TargetSquare.transform.position;
		pieceToMove.transform.position = new Vector3(destination.x, pieceToMove.YOffset, destination.z);
		// Deal with taking the other piece here
		if (TargetSquare.GetComponent<SquareScript>().IsOccupied){
			Destroy(TargetSquare.GetComponent<SquareScript>().PieceOnSquare.gameObject);
		}
		// Pawn advancement
		if (pieceToMove.PieceName == "Pawn"){
			// White
			if (pieceToMove.PieceOwner == 1 && TargetSquare.GetComponent<SquareScript>().RowRef == 0){
				BoardLayout[pieceToMove.RowRef,pieceToMove.ColRef] = 5;
				Destroy (pieceToMove.gameObject);
				Instantiate (WhiteQueen, new Vector3(destination.x, 0, destination.z), GameObject.Find ("Board").transform.rotation);
			// Black
			} else if (pieceToMove.PieceOwner == 2 && TargetSquare.GetComponent<SquareScript>().RowRef == 7) {
				BoardLayout[pieceToMove.RowRef,pieceToMove.ColRef] = 5;
				Destroy (pieceToMove.gameObject);
				Instantiate (BlackQueen, new Vector3(destination.x, 0, destination.z), GameObject.Find ("Board").transform.rotation);
			}
			
			
		}
		
		// Update the board layout and owner layout
		BoardLayout[TargetSquare.GetComponent<SquareScript>().RowRef,TargetSquare.GetComponent<SquareScript>().ColRef] = BoardLayout[pieceToMove.RowRef,pieceToMove.ColRef];
		BoardLayout[pieceToMove.RowRef,pieceToMove.ColRef] = 0;
        OwnerLayout[TargetSquare.GetComponent<SquareScript>().RowRef, TargetSquare.GetComponent<SquareScript>().ColRef] = OwnerLayout[pieceToMove.RowRef, pieceToMove.ColRef];
        OwnerLayout[pieceToMove.RowRef, pieceToMove.ColRef] = 0;

    }
	
	public void ChangeTurn(){
		if (PlayerTurn == 1){
			PlayerTurn = 2;
		} else if (PlayerTurn == 2) {
			PlayerTurn = 1;
		}
	}
	
	public bool CheckSelf(int rowFrom, int colFrom, int rowTo, int colTo, int[,] boardLayout, int[,] ownerLayout){
        // This player
        int thisPlayer = ownerLayout[rowFrom, colFrom];
        // Find my king
        List<int> kingCoords = FindPlayerKing(ownerLayout[rowFrom, colFrom], boardLayout, ownerLayout);

        // Make copies of the layout inputs so we aren't changing persistent variables
        int[,] bLayout = new int[8, 8];
        Buffer.BlockCopy(boardLayout, 0, bLayout, 0, boardLayout.Length * sizeof(int));
        // Get current owner layout
        int[,] oLayout = new int[8, 8];
        Buffer.BlockCopy(ownerLayout, 0, oLayout, 0, ownerLayout.Length * sizeof(int));

        // Simulate move
        bLayout[rowTo, colTo] = bLayout[rowFrom, colFrom];
        bLayout[rowFrom, colFrom] = 0;
        oLayout[rowTo, colTo] = oLayout[rowFrom, colFrom];
        oLayout[rowFrom, colFrom] = 0;

        // Iterate over opponent's moves to see whether the opponent can now take my King

        int[,] moves;
        int[,] allMoves = new int[8,8];
		for (int ii=0; ii < 8; ii++){
			for (int jj=0; jj < 8; jj++){
				if (oLayout[ii,jj] != thisPlayer){
                    moves = CalculateMoves(ii, jj,bLayout,oLayout);
                    allMoves = ElementwiseIf(allMoves, moves);
				}
			}
		}

        // special case for king movement
        if (kingCoords[0] == rowFrom && kingCoords[1] == colFrom)
        {
            kingCoords[0] = rowTo;
            kingCoords[1] = colTo;
        }
        // Check to see if any of the moves take my king
        if (allMoves[kingCoords[0],kingCoords[1]] == 1)
        {
            return true;
        } else {
            return false;
        }
	}
	
	public bool CheckOpp(int rowFrom, int colFrom, int rowTo, int colTo, int[,] boardLayout, int[,] ownerLayout)
    {
        // Find opposite player
        int otherPlayer = 0;
        if (ownerLayout[rowFrom, colFrom] == 1)
        {
            otherPlayer = 2;
        } else if (ownerLayout[rowFrom,colFrom] == 2)
        {
            otherPlayer = 1;
        }

        // Make copies of the layout inputs so we aren't changing persistent variables
        int[,] bLayout = new int[8, 8];
        Buffer.BlockCopy(boardLayout, 0, bLayout, 0, boardLayout.Length * sizeof(int));
        // Get current owner layout
        int[,] oLayout = new int[8, 8];
        Buffer.BlockCopy(ownerLayout, 0, oLayout, 0, ownerLayout.Length * sizeof(int));

        // This player
        int thisPlayer = oLayout[rowFrom, colFrom];
        // Find other player's king
        List<int> kingCoords = FindPlayerKing(otherPlayer, boardLayout, oLayout);

        // Simulate move
        bLayout[rowTo, colTo] = bLayout[rowFrom, colFrom];
        bLayout[rowFrom, colFrom] = 0;
        oLayout[rowTo, colTo] = oLayout[rowFrom, colFrom];
        oLayout[rowFrom, colFrom] = 0;

        // Iterate over my moves to see whether I can now take their King
        int[,] moves;
        int[,] allMoves = new int[8, 8];
        for (int ii = 0; ii < 8; ii++)
        {
            for (int jj = 0; jj < 8; jj++)
            {
                if (oLayout[ii, jj] == thisPlayer)
                {
                    moves = CalculateMoves(ii, jj, bLayout, oLayout);
                    allMoves = ElementwiseIf(allMoves, moves);
                }
            }
        }
        // Check to see if any of my moves take their king
        if (allMoves[kingCoords[0], kingCoords[1]] == 1)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public bool MateCheck(int rowFrom, int colFrom, int rowTo, int colTo, int[,] boardLayout, int[,] ownerLayout)
    {

        // Make copies of the layout inputs so we aren't changing persistent variables
        int[,] bLayout = new int[8, 8];
        Buffer.BlockCopy(boardLayout, 0, bLayout, 0, boardLayout.Length * sizeof(int));
        // Get current owner layout
        int[,] oLayout = new int[8, 8];
        Buffer.BlockCopy(ownerLayout, 0, oLayout, 0, ownerLayout.Length * sizeof(int));

        // This player
        int thisPlayer = oLayout[rowFrom, colFrom];

        // Simulate move
        bLayout[rowTo, colTo] = bLayout[rowFrom, colFrom];
        bLayout[rowFrom, colFrom] = 0;
        oLayout[rowTo, colTo] = oLayout[rowFrom, colFrom];
        oLayout[rowFrom, colFrom] = 0;

        // Iterate over their moves to see whether any result in not being in check
        int[,] moves;
        List<bool> mate = new List<bool>();
        for (int ii = 0; ii < 8; ii++)
        {
            for (int jj = 0; jj < 8; jj++)
            {
                if (oLayout[ii, jj] != thisPlayer && oLayout[ii,jj] != 0)
                {
                    moves = CalculateMoves(ii, jj, bLayout, oLayout);
                    for (int iMove = 0; iMove < 8; iMove++)
                    {
                        for (int jMove=0; jMove < 8; jMove++)
                        {
                            if (moves[iMove,jMove] == 1)
                            {
                                mate.Add(CheckSelf(ii, jj, iMove, jMove, bLayout, oLayout));
                            }
                        }
                    }
                }
            }
        }
        return mate.All(x => x == true);
	}
	

    public void EndGame()
    {
        string winner = this.PlayerInCheck == 1 ? "White" : "Black";
        Debug.Log("Game over!  " + winner + " player wins!");
        // Do some sort of winning thing here, then drop back to title screen
        Application.LoadLevel("MainMenu");
    }



    // Utility methods:
	public int[,] ElementwiseIf(int[,] a1, int[,] a2){
		int[,] arrayOut = new int[8,8];
		for (int ii=0; ii < 8; ii++){
			for (int jj=0; jj < 8; jj++){
				if (a1[ii,jj] != 0 || a2[ii,jj] != 0){
					arrayOut[ii,jj] = 1;
				}
			}
		}
		return arrayOut;
	}

    public List<int> FindPlayerKing(int playerOwner, int[,] boardLayout, int[,] ownerLayout)
    {
        List < int > rowCol = new List<int>();
        for (int ii = 0; ii < 8; ii++)
        {
            for (int jj = 0; jj < 8; jj++)
            {
                if (boardLayout[ii,jj] == 6 && ownerLayout[ii, jj] == playerOwner)
                {
                    rowCol.Add(ii);
                    rowCol.Add(jj);
                }
            }
        }
        return rowCol;
    }

}

