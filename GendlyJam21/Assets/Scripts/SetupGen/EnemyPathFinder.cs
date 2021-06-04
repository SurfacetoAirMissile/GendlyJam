using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using Baiji.Pathfinding;
using AStarPathfinder = Baiji.Pathfinding.AStarPathfinder<UnityEngine.Vector2Int, float, Voidlike, Voidlike>;

public static class EnemyPathFinder
{
    public static IEnumerable<Vector2Int> FindPath(Vector2Int start, Vector2Int goal)
    {
        Debug.Log("start: " + start);
        Debug.Log("Goal: " + goal);
        var impl = new EnemyPathFinderImpl(
            goal,
            TilemapSingleton.Instance,
            GameManager.Instance
            );
        var startData = new AStarPathfinder.AStarPathNodeData(0.0f, impl.HeuristicCost(start), default);
        var startNode = new EnemyPathFinderImpl.AStarUserProvidedNode(start, startData);
        var pathfinder = new AStarPathfinder(impl, startNode);
        //var step = pathfinder.Step();
        //Debug.Log(step);
        //while (!(step is ExhaustedBoundaryStepEvent))
        //{
        //    step = pathfinder.Step();
        //    Debug.Log(step);
        //}
        var finalStep = pathfinder.StepUntilGoal();
        if (finalStep is EnemyPathFinderImpl.FoundGoalStepEvent foundGoal)
        {
            return foundGoal.node.GetPath().Select(node => node.id);
        }
        else
        {
            Debug.LogError("EnemyPathFinder failed to find a path to castle at " + goal);
            return new List<Vector2Int>();
        }
    }
}

internal class EnemyPathFinderImpl : AStarPathfinder.AStarImplementation
{
    public EnemyPathFinderImpl(
        Vector2Int goal,
        TilemapSingleton tilemapSingleton,
        GameManager gameManager
        )
    {
        this.gameManager = gameManager;
        this.tilemapSingleton = tilemapSingleton;
        this.goal = goal;
    }

    public GameManager gameManager { get; private set; }
    public TilemapSingleton tilemapSingleton { get; private set; }
    public Vector2Int goal { get; private set; }

    public override float Add(float pathCost, float heuristicCost) => pathCost + heuristicCost;

    public float HeuristicCost(Vector2Int from)
    {
        return Vector2.Distance((Vector2)from, (Vector2)goal);
    }

    public override bool FoundGoal(IReadOnlyNode node, INodePathfinderStepEvent stepEvent)
    {
        Debug.Log("Compare: " + node.id);
        return node.id == goal;
    }

    public override IEnumerable<AStarUserProvidedEdge> AStarNeighbours(IReadOnlyNode parent)
    {
        var parentCell = parent.id;
        foreach (var neighbourCell in SquareGridUtils.NeighbourCoords(parentCell))
        {
            if (neighbourCell != goal)
            {
                var tileSprite = tilemapSingleton.tilemap.GetSprite((Vector3Int)neighbourCell);
                var tileData = gameManager.TryGetTileData(tileSprite);
                if (tileData is null || !tileData.canEnemiesWalk)
                {
                    // do not include this as a valid neighbour to search.
                    continue;
                }
            }

            var edgeCost = 1.0f;
            var pathCost = parent.extraData.pathCost + edgeCost;
            var heuristicCost = Vector2.Distance((Vector2)neighbourCell, (Vector2)goal);
            Voidlike extraNodeData = default;
            Voidlike extraEdgeData = default;
            var astarNode = new AStarPathfinder.AStarPathNodeData(pathCost, heuristicCost, extraNodeData);
            var node = new AStarUserProvidedNode(neighbourCell, astarNode);
            var edge = new AStarUserProvidedEdge(node, extraEdgeData);
            yield return edge;
        }
    }
}
