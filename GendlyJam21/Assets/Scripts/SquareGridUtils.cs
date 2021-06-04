using UnityEngine;

public static class SquareGridUtils
{
    public static Vector2Int[] LocalNeighbourCoords() => new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(0, -1),
    };

    public static Vector2Int[] NeighbourCoords(Vector2Int cellCoord)
    {
        var neighbours = LocalNeighbourCoords();
        for (var i = 0; i < neighbours.Length; ++i)
        {
            neighbours[i] += cellCoord;
        }
        return neighbours;
    }
}
