using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Baiji.Pathfinding;
using AStarPathfinder = Baiji.Pathfinding.AStarPathfinder<UnityEngine.Vector3Int, float, Voidlike, Voidlike>;

//public class EnemyPathFinderImpl : AStarPathfinder.AStarImplementation
//{
//    public override float Add(float pathCost, float heuristicCost) => pathCost + heuristicCost;

//    public override IEnumerable<AStarUserProvidedEdge> AStarNeighbours(IReadOnlyNode parent)
//    {
//    }
//}

//public static class EnemyPathFinder
//{
//    public static List<Vector3Int> FindPath(Vector3Int enemySpawnCell, Vector3Int castleCell)
//    {
//        var pathfinder = new AStarPathfinder()
//    }
//}
