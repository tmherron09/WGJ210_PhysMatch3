using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager instance;
    private void Awake()
    {
        instance = this;
    }


    [Header("Grid Size")]
    public int columnCount;
    public int rowCount;

    [Header("Column/Row Casters")]
    public GameObject columnCaster;
    public GameObject rowCaster;

    [HideInInspector]
    public ColumnCaster[] columns;
    [HideInInspector]
    public RowRayCaster[] rows;

    bool sendGridValues;
    float tileSize;


    public void GridInitialize()
    {
        // Assume tiles are square
        tileSize = RaycastGameManager.instance.tile.GetComponent<BoxCollider2D>().size.x;

        CreateGridBottom(tileSize);
        InitializeColumns();
        InitializeRows();

        CreateInitialGrid();
    }

    private void InitializeRows()
    {
        rows = new RowRayCaster[rowCount];
        for (int i = 0; i < rowCount; i++)
        {
            var row = Instantiate(rowCaster);
            rows[i] = row.GetComponent<RowRayCaster>();
            row.transform.position = transform.position + (new Vector3(0, (i * tileSize) + tileSize / 2, 0));
        }
    }

    private void InitializeColumns()
    {
        columns = new ColumnCaster[columnCount];
        for (int i = 0; i < columnCount; i++)
        {
            var column = Instantiate(columnCaster);
            columns[i] = column.GetComponent<ColumnCaster>();
            columns[i].columnHeight = rowCount;
            column.transform.position = transform.position + (new Vector3((i * tileSize) + tileSize / 2, 0, 0));
        }
    }

    public void GridUpdate()
    {

        if (CheckTilesMoving())
        {
            TileManager.instance.canMove = false;
        }
        else
        {
            CheckAllColumns();
            if (!CheckTilesMoving())
            {
                TileManager.instance.canMove = true;
            }
        }

    }

    /// <summary>
    /// Signals all Columns to check for tile movement.
    /// </summary>
    /// <returns>True if tiles are still falling.</returns>
    public bool CheckTilesMoving()
    {
        for (int i = 0; i < columns.Length; i++)
        {
            if (columns[i].CheckDropComplete())
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Creates EdgeCollider2d for Bottom of the Grid.
    /// </summary>
    /// <param name="tileSize">Size of Tile Prefab for Edge Width</param>
    private void CreateGridBottom(float tileSize)
    {
        var bottom = new GameObject("GridBottom");
        bottom.transform.position = transform.position;
        var collider = bottom.AddComponent<EdgeCollider2D>();
        var points = new Vector2[2];
        points[0] = new Vector2(0, 0);
        points[1] = new Vector2((columnCount + 1) * tileSize, 0);
        collider.points = points;
    }


    /// <summary>
    /// Signals all Columns to do a tile check for count and refill.
    /// </summary>
    public bool CheckAllColumns()
    {
        bool reCheck = false;
        for (int i = 0; i < columnCount; i++)
        {
            reCheck = columns[i].CheckColumn();
        }
        return reCheck;
    }


    /// <summary>
    /// Create an Initial Tile Grid. Basic layout with no matching tiles touching. Will not create
    /// </summary>
    void CreateInitialGrid()
    {
        List<TileScriptableObject> lastColumn = new List<TileScriptableObject>(new TileScriptableObject[rowCount]);
        List<TileScriptableObject> availableTiles;

        for (int i = 0; i < columnCount; i++)
        {
            TileScriptableObject prevTile = null;
            for (int j = 0; j < rowCount; j++)
            {
                availableTiles = new List<TileScriptableObject>(RaycastGameManager.instance.levelTiles);
                availableTiles.Remove(lastColumn[j]);
                availableTiles.Remove(prevTile);

                var selectedTile = availableTiles[UnityEngine.Random.Range(0, availableTiles.Count)];
                columns[i].DropTile(j, selectedTile);

                lastColumn[j] = selectedTile;
                prevTile = selectedTile;
            }
        }
    }

    public bool CheckAllPossibleMatches()
    {
        var matchingTiles = new List<RayTile>();

        CheckAllRowsMatch(matchingTiles);
        CheckAllColumnsMatch(matchingTiles);

        int matchAmount = matchingTiles.Count;

        RaytileMatchEvents(matchingTiles);

        return matchAmount >= 3;
    }

    void CheckAllRowsMatch(List<RayTile> matchingTiles)
    {
        for (int i = 0; i < rowCount; i++)
        {
            CheckRowForMatches(rows[i], matchingTiles);
        }
    }

    void CheckRowForMatches(RowRayCaster row, List<RayTile> matchingTiles)
    {
        var raytiles = row.GetRowTiles();
        int count = 1;
        var prevTile = raytiles[0].GetTileType();
        var currentMatching = new List<RayTile>();
        currentMatching.Add(raytiles[0]);
        for (int i = 1; i < row.rowLength; i++)
        {
            if (raytiles[i].GetTileType() == prevTile)
            {
                count++;
                AddRaytileToList(currentMatching, raytiles[i]);
                if (i == row.rowLength - 1 && count >= 3)
                {
                    AddMatchingTiles(matchingTiles, currentMatching);
                    //currentMatching.Clear();
                }
            }
            else if (count >= 3)
            {
                AddMatchingTiles(matchingTiles, currentMatching);
                currentMatching.Clear();
            }
            else
            {
                currentMatching.Clear();
            }
        }
    }

    void CheckAllColumnsMatch(List<RayTile> matchingTiles)
    {
        for (int i = 0; i < columnCount; i++)
        {
            CheckColumnForMatches(columns[i], matchingTiles);
        }
    }

    void CheckColumnForMatches(ColumnCaster column, List<RayTile> matchingTiles)
    {
        var raytiles = column.GetColumnTiles();
        int count = 1;
        var prevTile = raytiles[0].GetTileType();
        var currentMatching = new List<RayTile>();
        currentMatching.Add(raytiles[0]);
        for (int i = 1; i < column.columnHeight; i++)
        {
            if (raytiles[i].GetTileType() == prevTile)
            {
                count++;
                AddRaytileToList(currentMatching, raytiles[i]);
                if (i == column.columnHeight - 1 && count >= 3)
                {
                    AddMatchingTiles(matchingTiles, currentMatching);
                    //currentMatching.Clear();
                }
            }
            else if (count >= 3)
            {
                AddMatchingTiles(matchingTiles, currentMatching);
                currentMatching.Clear();
            }
            else
            {
                currentMatching.Clear();
            }
        }
    }

    void AddRaytileToList(List<RayTile> raytiles, RayTile raytile)
    {
        if (!raytiles.Contains(raytile))
        {
            raytiles.Add(raytile);
        }
    }

    void AddMatchingTiles(List<RayTile> matchingTiles, List<RayTile> toAddRaytiles)
    {
        foreach (var raytile in toAddRaytiles)
        {
            AddRaytileToList(matchingTiles, raytile);
        }
    }

    void RaytileMatchEvents(List<RayTile> matchingTiles)
    {
        foreach (var tile in matchingTiles)
        {
            tile.MatchEvent();
        }
    }



}
