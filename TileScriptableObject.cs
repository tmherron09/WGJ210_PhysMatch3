using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TileType", menuName = "Scriptable Objects/Tile Type")]
public class TileScriptableObject : ScriptableObject
{

    public string tileName;
    public RayTile.TileType tileType;

    public Sprite tileSprite;

}
