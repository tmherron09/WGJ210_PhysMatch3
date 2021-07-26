using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastGameManager : MonoBehaviour
{
    public static RaycastGameManager instance;

    /// <summary>
    /// Tile Prefab
    /// </summary>
    public GameObject tile;
    /// <summary>
    /// All Tile Scriptable Objects available in scene.
    /// </summary>
    public List<TileScriptableObject> levelTiles;


    public Dictionary<RayTile.TileType, int> scores;

    private void Awake()
    {
        instance = this;
        CurrentGameState = GameTileState.Initialize;
    }

    public enum GameTileState
    {
        Initialize,
        CheckTiles,
        CheckingMovement,
        AwaitPlayer,
        CheckingSwap,
        CheckAllMatch,
        GameFinish
    }
    public static GameTileState CurrentGameState;


    void Start()
    {
        InitializeScores();
        GridManager.instance.GridInitialize();
        CurrentGameState = GameTileState.CheckTiles;
    }

    // Update is called once per frame
    void Update()
    {

        switch (CurrentGameState)
        {
            case GameTileState.CheckTiles:
                CheckTiles();
                break;
            case GameTileState.CheckingMovement:
                CheckingMovement();
                break;
            case GameTileState.CheckAllMatch:
                CheckAllMatch();
                break;
            case GameTileState.AwaitPlayer:
                break;
            default:
                CurrentGameState = GameTileState.CheckTiles;
                break;
        }
    }

    void CheckTiles()
    {
        if(!GridManager.instance.CheckAllColumns())
        {
            CurrentGameState = GameTileState.CheckingMovement;
        }
    }

    void CheckingMovement()
    {
        if (GridManager.instance.CheckTilesMoving())
        {
            return;
        }
        else
        {
            CurrentGameState = GameTileState.CheckAllMatch;
        }
    }

    void CheckAllMatch()
    {
        //Check All Tiles for match.
        if (GridManager.instance.CheckAllPossibleMatches())
        {
            CurrentGameState = GameTileState.CheckTiles;
        }
        else
        {
            CurrentGameState = GameTileState.AwaitPlayer;
        }
    }



    void InitializeScores()
    {
        scores = new Dictionary<RayTile.TileType, int>();
        foreach (var type in levelTiles)
        {
            scores.Add(type.tileType, 0);
        }
    }
    public void UpdateScore(TileScriptableObject tileScriptableObject)
    {
        scores[tileScriptableObject.tileType] += 1;
    }

}
