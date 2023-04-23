using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;


[CreateAssetMenu(menuName = "ENV/Custom Tiles/Advanced Rule Tile")]
public class LavaRockTileRule : HexagonalRuleTile<LavaRockTileRule.Neighbor> {
    [Tooltip("Tiles to connect to")]
    public TileBase[] tilesToConnect;
    
    public class Neighbor : HexagonalRuleTile.TilingRule.Neighbor {
    }

    public override bool RuleMatch(int neighbor, TileBase tile) {
        switch (neighbor) {
            case Neighbor.This: return CheckThis(tile);
            case Neighbor.NotThis: return CheckNotThis(tile);
        }
        Debug.Log($"Tile: {neighbor}, {tile.name}");
        return base.RuleMatch(neighbor, tile);
    }

    private bool CheckThis(TileBase tile)
    {
        return tilesToConnect.Contains(tile); //' || tile == this;
    }
    
    private bool CheckNotThis(TileBase tile)
    {
        return !tilesToConnect.Contains(tile); // && tile != this;
    }
}