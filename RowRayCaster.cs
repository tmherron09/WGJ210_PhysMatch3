using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RowRayCaster : MonoBehaviour
{
    GameObject[] rowObjects;

    public int rowLength = 6;
    public float rayLength;
    public string[] objectNames;

    LayerMask tileMask;

    private void Awake()
    {
        tileMask = LayerMask.GetMask("Tiles");
    }

    public List<RayTile> GetRowTiles()
    {
        var rowTiles = Physics2D.RaycastAll(transform.position, Vector2.right, rowLength * 1.1f, tileMask);
        var rowRayTiles = new List<RayTile>();
        for (int i = 0; i < rowTiles.Length; i++)
        {
            rowRayTiles.Add(rowTiles[i].collider.GetComponent<RayTile>());
        }
        return rowRayTiles;
    }


    private void RowUpdate()
    {
        Debug.DrawRay(transform.position, new Vector2(rayLength, 0), Color.red);
        var raycastHits = Physics2D.RaycastAll(transform.position, Vector2.right, rowLength * 1.1f, tileMask);
        //var raycastHits = Physics2D.RaycastAll(transform.position, Vector2.right, rowLength * 1.1f, 1 << LayerMask.NameToLayer("Tiles"));
        rowObjects = new GameObject[raycastHits.Length];
        for(int i = 0;i < raycastHits.Length; i++)
        {
            rowObjects[i] = raycastHits[i].collider.gameObject;
        }
        objectNames = new string[rowObjects.Length];
        for(int i = 0; i < rowObjects.Length; i++)
        {
            objectNames[i] = rowObjects[i].name;
        }
    }

    bool TestForFullRow()
    {
        var raycastHits = Physics2D.RaycastAll(transform.position, Vector2.right, rayLength);
        rowObjects = new GameObject[raycastHits.Length];
        for (int i = 0; i < raycastHits.Length; i++)
        {
            rowObjects[i] = raycastHits[i].collider.gameObject;
        }
        return true;
    }

}
