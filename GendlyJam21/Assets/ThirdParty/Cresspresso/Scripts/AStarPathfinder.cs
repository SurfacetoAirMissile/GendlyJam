///
/// This script was taken from a separate project Elijah Shadbolt worked on.
/// It was copied over 03/06/2021.
///

using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace Baiji.Pathfinding
{
	public sealed class AStarPathfinder<TNodeID, TCost, TExtraNodeData, TExtraEdgeData>
	{
		public Pathfinder<TNodeID, TCost, AStarPathNodeData, TExtraEdgeData> innerPathfinder { get; private set; }

		public AStarImplementation implementation => (AStarImplementation)innerPathfinder.implementation;

		public Dictionary<TNodeID, AStarImplementation.Node> exploredNodes => innerPathfinder.exploredNodes;
		public PriorityQueue<TCost, AStarImplementation.Node> openBoundary => innerPathfinder.boundary;

		public IPathfinderStepEvent latestStepEvent
		{
			get => innerPathfinder.latestStepEvent;
			set => innerPathfinder.latestStepEvent = value;
		}



		public AStarPathfinder(
			AStarImplementation implementation,
			AStarImplementation.AStarUserProvidedNode start)
		{
			this.innerPathfinder = new Pathfinder<TNodeID, TCost, AStarPathNodeData, TExtraEdgeData>(
				implementation,
				implementation.MakeInnerUserProvidedNode(start)
				);
			var startEvent = (AStarImplementation.ExploredNodeStepEvent)this.innerPathfinder.latestStepEvent;
			if (implementation.FoundGoal(startEvent.node, startEvent))
			{
				innerPathfinder.latestStepEvent = new AStarImplementation.FoundGoalStepEvent(startEvent);
			}
		}

		/// <summary>WARNING: Takes ownership of the pathfinder.</summary>
		public AStarPathfinder(Pathfinder<TNodeID, TCost, AStarPathNodeData, TExtraEdgeData> innerPathfinder)
		{
			if (!(innerPathfinder.implementation is AStarImplementation))
			{
				throw new ArgumentException(nameof(innerPathfinder) + "'s " + nameof(innerPathfinder.implementation) + " must be of type " + nameof(AStarImplementation), paramName: nameof(innerPathfinder));
			}

			this.innerPathfinder = innerPathfinder;
		}



		public IPathfinderStepEvent Step(AStarImplementation.Node current)
		{
			innerPathfinder.Step(current);

			if (latestStepEvent is AStarImplementation.INodePathfinderStepEvent nodeEvent)
			{
				if (nodeEvent is AStarImplementation.ExploredNodeStepEvent
					|| nodeEvent is AStarImplementation.ReplacedNodeStepEvent)
				{
					if (implementation.FoundGoal(nodeEvent.node, nodeEvent))
					{
						innerPathfinder.latestStepEvent = new AStarImplementation.FoundGoalStepEvent(nodeEvent);
						return innerPathfinder.latestStepEvent;
					}
				}
			}

			return latestStepEvent;
		}

		public IPathfinderStepEvent Step() => innerPathfinder.Step();

		public IPathfinderStepEvent StepUntilGoal()
		{
			while (!(latestStepEvent is ExhaustedBoundaryStepEvent)
				&& !(latestStepEvent is AStarImplementation.FoundGoalStepEvent))
			{
				Step();
			}
			return latestStepEvent;
		}

		public /*Nullable*/ AStarImplementation.IReadOnlyNode TryToFindPathToGoal()
		{
			if (StepUntilGoal() is AStarImplementation.FoundGoalStepEvent foundGoal)
			{
				return foundGoal.node;
			}
			else
			{
				return null;
			}
		}



		public sealed class AStarPathNodeData
		{
			public AStarPathNodeData(TCost pathCost, TCost heuristicCost, TExtraNodeData extraAStarData)
			{
				this.pathCost = pathCost;
				this.heuristicCost = heuristicCost;
				this.extraAStarData = extraAStarData;
			}

			public TCost pathCost { get; private set; }
			public TCost heuristicCost { get; private set; }
			public TExtraNodeData extraAStarData { get; private set; }
		}



		public abstract class AStarImplementation
			: Pathfinder<TNodeID, TCost, AStarPathNodeData, TExtraEdgeData>.Implementation
		{
			public abstract TCost Add(TCost pathCost, TCost heuristicCost);

			public virtual bool FoundGoal(IReadOnlyNode node, INodePathfinderStepEvent stepEvent)
				=> IsEqual(node.extraData.heuristicCost, default(TCost));

			public abstract IEnumerable<AStarUserProvidedEdge> AStarNeighbours(IReadOnlyNode parent);



			public class AStarUserProvidedEdge
			{
				public AStarUserProvidedEdge(
					AStarUserProvidedNode destination,
					TExtraEdgeData extraData)
				{
					if (destination == null)
						throw new ArgumentNullException(nameof(destination));

					this.destination = destination;
					this.extraAStarData = extraData;
				}

				public AStarUserProvidedNode destination { get; private set; }
				public TExtraEdgeData extraAStarData { get; private set; }
			}

			public class AStarUserProvidedNode
			{
				public AStarUserProvidedNode(
					TNodeID id,
					AStarPathNodeData data)
				{
					this.id = id;
					this.data = data;
				}

				public TNodeID id { get; private set; }
				public AStarPathNodeData data { get; private set; }
			}



			public sealed class FoundGoalStepEvent : IPathfinderStepEvent
			{
				public FoundGoalStepEvent(INodePathfinderStepEvent nodeEvent)
				{
					this.nodeEvent = nodeEvent;
				}

				public INodePathfinderStepEvent nodeEvent { get; private set; }
				public IReadOnlyNode node => nodeEvent.node;
			}



			public sealed override IEnumerable<UserProvidedEdge>
				Neighbours(IReadOnlyNode parent)
			{
				return AStarNeighbours(parent).Select(neighbour =>
				new UserProvidedEdge(
					MakeInnerUserProvidedNode(neighbour.destination),
					neighbour.extraAStarData
					)
				);
			}



			public UserProvidedNode MakeInnerUserProvidedNode(AStarUserProvidedNode node) =>
				new UserProvidedNode(
					node.id,
					Add(node.data.pathCost, node.data.heuristicCost),
					node.data
				);
		}
	}
}