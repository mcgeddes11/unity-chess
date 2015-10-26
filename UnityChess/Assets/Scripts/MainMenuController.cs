using UnityEngine;
using System.Collections;

public class MainMenuController : MonoBehaviour {
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
    public GameObject Container;
    public GameObject TwoPlayerSetup;
    public GameObject MainMenu;

    public string PlayerOneName;
    public string PlayerTwoName;

    // Use this for initialization
    void Start () {
        InstantiateBoard();
        InstantiatePieces();
        DontDestroyOnLoad(GameObject.Find("GameConfigData"));
    }
	
	// Update is called once per frame
	void Update ()
    {
        Container.transform.RotateAround(Vector3.zero, Vector3.up, Time.deltaTime*5);

        if (Input.GetKeyUp(KeyCode.Escape) && !MainMenu.activeSelf)
        {
            MainMenu.SetActive(true);
        }

        if (TwoPlayerSetup.activeSelf)
        {
            GameObject OkButton = GameObject.Find("OK");
            if (OkButton.GetComponent<InstantGuiButton>().pressed)
            {
                GameConfigData configData = GameObject.Find("GameConfigData").GetComponent<GameConfigData>();
                configData.PlayerOneName = GameObject.Find("Player White").GetComponentInChildren<GUIText>().text;
                configData.PlayerTwoName = GameObject.Find("Player Black").GetComponentInChildren<GUIText>().text;
                configData.NumPlayers = 2;
                // Save data and load level
                Application.LoadLevel("MainGame");
            }
        }

        if (MainMenu.activeSelf && GameObject.Find("Quit").GetComponent<InstantGuiButton>().pressed)
        {
            Application.Quit();
        }

    }


    void InstantiateBoard()
    {
        GameObject board = GameObject.Find("Board");
        // Populate board squares
        for (int ii = 0; ii < 64; ii++)
        {
            GameObject go;
            // Col ref
            int colRef = (ii % 8);
            // Row ref
            int rowRef = (ii / 8);
            // If rowRef + colRef is odd, black, else white
            if ((rowRef + colRef) % 2 > 0)
            {
                go = Instantiate(BlackSquare).gameObject;
            }
            else
            {
                go = Instantiate(WhiteSquare).gameObject;
            }
            go.transform.parent = board.transform;
            go.transform.position = new Vector3(rowRef-3.5f, 0.2f, -colRef+3.5f);
            go.name = "a" + rowRef.ToString() + colRef.ToString();

        }


    }

    void InstantiatePieces()
    {

        // Black Pieces
        PopulateSquare(0, 0, BlackRook);
        PopulateSquare(0, 7, BlackRook);
        PopulateSquare(0, 1, BlackKnight);
        PopulateSquare(0, 6, BlackKnight);
        PopulateSquare(0, 2, BlackBishop);
        PopulateSquare(0, 5, BlackBishop);
        PopulateSquare(0, 4, BlackQueen);
        PopulateSquare(0, 3, BlackKing);
        for (int col = 0; col < 8; col++)
        {
            PopulateSquare(1, col, BlackPawn);
        }


        // White Pieces
        PopulateSquare(7, 0, WhiteRook);
        PopulateSquare(7, 7, WhiteRook);
        PopulateSquare(7, 1, WhiteKnight);
        PopulateSquare(7, 6, WhiteKnight);
        PopulateSquare(7, 2, WhiteBishop);
        PopulateSquare(7, 5, WhiteBishop);
        PopulateSquare(7, 4, WhiteQueen);
        PopulateSquare(7, 3, WhiteKing);
        for (int col = 0; col < 8; col++)
        {
            PopulateSquare(6, col, WhitePawn);
        }

    }

    void PopulateSquare(int row, int col, Rigidbody obj)
    {
        GameObject board = GameObject.Find("Board");
        Vector3 p = new Vector3(col-3.5f, 0, -row+3.5f);
        GameObject go = Instantiate(obj).gameObject;
        go.transform.parent = board.transform;
        go.transform.position = p;

    }

}



