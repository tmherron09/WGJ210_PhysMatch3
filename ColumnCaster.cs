using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColumnCaster : MonoBehaviour
{

    GameObject[] columnTiles;
    [Header("Tile Prefab")]
    public GameObject Tile;
    [Header("Column Tile Drop Location")]
    public GameObject columnSpawnTarget;
    Vector3 tileSpawnOrigin;

    //[Header("Tile Scriptable Objects")]
    List<TileScriptableObject> tileScriptableObjects;

    [Header("Tile Stored Values")]
    public int ColumnNumber;
    public int RowNumber;
    public int columnHeight = 3;
    public LayerMask tileMask;
    public float rayLength;
    public string[] objectNames;

    [Header("Misc.")]
    public bool hasMovement;


    private void Start()
    {
        tileScriptableObjects = RaycastGameManager.instance.levelTiles;

        if (columnSpawnTarget == null)
        {
            tileSpawnOrigin = transform.position + new Vector3(0f, 7f, 0f);
        }
        else
        {
            tileSpawnOrigin = columnSpawnTarget.transform.position;
        }
    }

    public bool CheckColumn()
    {
        bool reCheck = false;

        if (Application.isEditor)
        {
            Debug.DrawRay(transform.position, new Vector2(0, rayLength), Color.green, 3f);
            var raycastHits = Physics2D.RaycastAll(transform.position, Vector2.up, (columnHeight - 1) + .5f, tileMask.value);
            columnTiles = new GameObject[raycastHits.Length];
            for (int i = 0; i < raycastHits.Length; i++)
            {
                columnTiles[i] = raycastHits[i].collider.gameObject;
            }
            objectNames = new string[columnTiles.Length];
            for (int i = 0; i < columnTiles.Length; i++)
            {
                objectNames[i] = columnTiles[i].name;
            }
        }


        var tileCheck = Physics2D.RaycastAll(transform.position, Vector2.up, Mathf.Infinity, tileMask);
        if (tileCheck.Length != columnHeight)
        {
            reCheck = true;
            Debug.DrawRay(transform.position, Vector2.down, Color.red, 1f);
            if (tileCheck.Length > columnHeight)
            {
                for (int i = columnHeight - 1; i < tileCheck.Length; i++)
                {
                    Destroy(tileCheck[i].collider.gameObject);
                }
            }
            else if (tileCheck.Length < columnHeight)
            {
                //Vector3 tileSpawn = tileSpawnOrigin;
                for (int i = 0; i < columnHeight - tileCheck.Length; i++)
                {
                    DropTile(i);
                }
            }
        }
        return reCheck;
    }

    public void DropTile(int position = 0, TileScriptableObject selectedTileSO = null)
    {
        var tileGameObject = Instantiate(Tile);
        tileGameObject.transform.position = tileSpawnOrigin + new Vector3(0f, 1.65f * position, 0f);
        if (selectedTileSO == null)
        {
            selectedTileSO = tileScriptableObjects[UnityEngine.Random.Range(0, tileScriptableObjects.Count)];
        }
        var tile = tileGameObject.GetComponent<RayTile>();
        tile.tileScriptableObject = selectedTileSO;
        tile.columnNumber = ColumnNumber;
        tile.UpdateSprite();
        if (Application.isEditor)
        {
            tileGameObject.name = $"Column: {ColumnNumber} Type:{selectedTileSO.tileType}";
        }
    }

    public List<RayTile> GetColumnTiles()
    {
        var columnTiles = Physics2D.RaycastAll(transform.position, Vector2.up, columnHeight * 2f, tileMask);
        var columnRaytiles = new List<RayTile>();
        for (int i = 0; i < columnTiles.Length; i++)
        {
            columnRaytiles.Add(columnTiles[i].collider.GetComponent<RayTile>());
        }
        return columnRaytiles;
    }



    public bool CheckDropComplete()
    {
        var tileCheck = Physics2D.RaycastAll(transform.position, Vector2.up, Mathf.Infinity, tileMask);
        foreach (var tile in tileCheck)
        {
            if (!tile.collider.attachedRigidbody.IsSleeping()) return false;
        }
        return true;
    }
    IEnumerator CreateTile(Vector3 tileSpawn, int i)
    {
        var tileGameObject = Instantiate(Tile);
        yield return new WaitForSeconds(0.1f);
        tileGameObject.transform.position = tileSpawn + new Vector3(0f, 1.1f * i, 0f);
        var tileSO = tileScriptableObjects[UnityEngine.Random.Range(0, tileScriptableObjects.Count - 1)];
        var tile = tileGameObject.GetComponent<RayTile>();
        tile.tileScriptableObject = tileSO;
        tile.UpdateSprite();

        tileGameObject.name = $"{ColumnNumber}- Tile";
    }

}
