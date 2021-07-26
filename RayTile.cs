using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RayTile : MonoBehaviour
{
    public TileScriptableObject tileScriptableObject;
    SpriteRenderer spriteRender;

    public int rowNumber;
    public int columnNumber;

    float tileSize;
    Vector3 mouseDownStart;

    private void Awake()
    {
        spriteRender = GetComponent<SpriteRenderer>();
        rowNumber = -1;
        columnNumber = -1;
        tileSize = GetComponent<BoxCollider2D>().size.x;
    }

    public enum TileType
    {
        red = 0,
        green = 1,
        blue = 2,
        yellow = 3,
        purple = 4
    }

    public TileType GetTileType() => tileScriptableObject.tileType;

    public void UpdateSprite()
    {
        spriteRender.sprite = tileScriptableObject.tileSprite;
    }

    public void MatchEvent()
    {
        RaycastGameManager.instance.UpdateScore(tileScriptableObject);
        // Play Particle Instance at this Transform.
        Destroy(gameObject);
    }

    private void OnMouseDown()
    {
        mouseDownStart = Camera.main.ScreenToWorldPoint(Input.mousePosition); ;
    }

    private void OnMouseUp()
    {
        if(RaycastGameManager.CurrentGameState != RaycastGameManager.GameTileState.AwaitPlayer)
        {
            return;
        }
        Vector3 mouseEnd = Camera.main.ScreenToWorldPoint(Input.mousePosition); ;
        var delta = (Vector2)mouseEnd - (Vector2)mouseDownStart;
        Debug.Log(delta);
        int x = 0;
        int y = 0;
        if(Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
        {
            x = (int)Mathf.Sign(delta.x);
        } else
        {
            y = (int)Mathf.Sign(delta.y);
        }
        Debug.Log($"X:{x} Y:{y}");
        var hit = Physics2D.RaycastAll(transform.position, new Vector2(x, y), 1.6f) ;
        if(hit.Length > 1 && hit[1].collider.TryGetComponent<RayTile>(out RayTile swapTile))
        {
            TileManager.instance.CallTileSwap(gameObject, hit[1].collider.gameObject);
        }
    }
}
