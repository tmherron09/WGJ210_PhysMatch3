using Holoville.HOTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TileManager : MonoBehaviour
{
    public static TileManager instance;

    public bool startSwap;
    public GameObject tileA;
    public GameObject tileB;

    public LayerMask layerMask;

    public bool hasTileMovement;
    public bool canMove;

    //public GameObject tilePrefab;
    //public TileScriptableObject[] tileSO;

    private void Awake()
    {
        instance = this;
    }

    // Depreciated Editor Only Proto Update
    //public void Update()
    //{
    //    if (startSwap && canMove)
    //    {
    //        DoTileSwap();
    //        startSwap = false;
    //    }
    //}

    public void CallTileSwap(GameObject startTile, GameObject swapTile)
    {
        if (RaycastGameManager.CurrentGameState == RaycastGameManager.GameTileState.AwaitPlayer)
        {
            canMove = false;
            tileA = startTile;
            tileB = swapTile;
            DoTileSwap();
        }
    }

    void DoTileSwap()
    {
        Transform a = tileA.transform;
        Transform b = tileB.transform;

        Vector3 aStart = tileA.transform.position;
        Vector3 bStart = tileB.transform.position;
        DoSwapMotion(a, b, bStart, aStart);
        StartCoroutine(RaycastTileMatchVertical());
    }

    void DoSwapMotion(Transform a, Transform b, Vector3 aTarget, Vector3 bTarget)
    {
        a.DOMove(aTarget, .2f);
        b.DOMove(bTarget, .2f);
    }

    IEnumerator RaycastTileMatchVertical()
    {
        yield return new WaitForSeconds(.2f);

        var checkingTileType = tileA.GetComponent<RayTile>().GetTileType();

        List<RayTile> aVertTiles = new List<RayTile>();
        aVertTiles.Add(tileA.GetComponent<RayTile>());
        int aMatchCountVert = 1;
        aMatchCountVert += TileTypeRayCheck(tileA, Vector2.up, checkingTileType, aVertTiles);
        aMatchCountVert += TileTypeRayCheck(tileA, Vector2.down, checkingTileType, aVertTiles);

        List<RayTile> aHorzTiles = new List<RayTile>();
        aHorzTiles.Add(tileA.GetComponent<RayTile>());
        int aMatchCountHorz = 1;
        aMatchCountHorz += TileTypeRayCheck(tileA, Vector2.left, checkingTileType, aHorzTiles);
        aMatchCountHorz += TileTypeRayCheck(tileA, Vector2.right, checkingTileType, aHorzTiles);


        checkingTileType = tileB.GetComponent<RayTile>().GetTileType();

        List<RayTile> bVertTiles = new List<RayTile>();
        bVertTiles.Add(tileB.GetComponent<RayTile>());
        int bMatchCountVert = 1;
        bMatchCountVert += TileTypeRayCheck(tileB, Vector2.up, checkingTileType, bVertTiles);
        bMatchCountVert += TileTypeRayCheck(tileB, Vector2.down, checkingTileType, bVertTiles);

        int bMatchCountHorz = 1;
        List<RayTile> bHorzTiles = new List<RayTile>();
        bHorzTiles.Add(tileB.GetComponent<RayTile>());
        bMatchCountHorz += TileTypeRayCheck(tileB, Vector2.left, checkingTileType, bVertTiles);
        bMatchCountHorz += TileTypeRayCheck(tileB, Vector2.right, checkingTileType, bVertTiles);


        //Debug.Log($"Tile {tileA.GetComponent<RayTile>().GetTileType()} is {aMatchCountVert} vertically.");
        //Debug.Log($"Tile {checkingTileType} is {bMatchCountVert} vertically.");
        //Debug.Log($"Tile {tileA.GetComponent<RayTile>().GetTileType()} is {aMatchCountHorz} Horizontally.");
        //Debug.Log($"Tile {checkingTileType} is {bMatchCountHorz} Horizontally.");

        bool hasAMatch, hasBMatch;
        hasAMatch = MatchCheck(aHorzTiles, aVertTiles);
        hasBMatch = MatchCheck(bHorzTiles, bVertTiles);

        if (!hasAMatch && !hasBMatch)
        {
            DoTileSwap();
        }

        tileA = null;
        tileB = null;
    }

    int TileTypeRayCheck(GameObject a, Vector2 dir, RayTile.TileType tileType, List<RayTile> tiles)
    {
        int matchingCount = 0;
        var tileCheck = Physics2D.RaycastAll(a.transform.position, dir, Mathf.Infinity, layerMask);
        for (int i = 1; i < tileCheck.Length; i++)
        {
            if (tileCheck[i].collider.GetComponent<RayTile>().GetTileType() == tileType)
            {
                matchingCount++;
                tiles.Add(tileCheck[i].collider.GetComponent<RayTile>());
            }
            else
            {
                break;
            }
        }
        return matchingCount;
    }

    bool MatchCheck(List<RayTile> horzTiles, List<RayTile> vertTiles)
    {
        bool hasMatch = true;
        if (horzTiles.Count >= 3 && vertTiles.Count >= 3)
        {
            List<RayTile> twoAxis = new List<RayTile>(horzTiles);
            foreach (var tile in vertTiles)
            {
                if (!twoAxis.Contains(tile))
                {
                    twoAxis.Add(tile);
                }
            }
            foreach (var tile in twoAxis)
            {
                tile.MatchEvent();
            }
        }
        else if (horzTiles.Count >= 3)
        {
            foreach (var tile in horzTiles)
            {
                tile.MatchEvent();
            }
        }
        else if (vertTiles.Count >= 3)
        {
            foreach (var tile in vertTiles)
            {
                tile.MatchEvent();
            }
        } 
        else
        {
            hasMatch = false;
        }
        return hasMatch;
    }
}
