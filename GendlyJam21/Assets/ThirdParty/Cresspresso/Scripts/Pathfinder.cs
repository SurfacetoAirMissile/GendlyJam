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
	public interface IPathfinderStepEvent { }
	public sealed class ExhaustedBoundaryStepEvent : IPathfinderStepEvent { }



	public sealed class Pathfinder<TNodeID, TCost, TExtraNodeData, TExtraEdgeData>
	{
		public Implementation implementation { get; private set; }
		public Dictionary<TNodeID, Implementation.Node> exploredNodes { get; private set; }
		public PriorityQueue<TCost, Implementation.Node> boundary { get; private set; }

		public IPathfinderStepEvent latestStepEvent
		{
			get => m_latestStepEvent;
			set => m_latestStepEvent = value != null ? value : throw new ArgumentNullException(nameof(value));
		}
		private IPathfinderStepEvent m_latestStepEvent;

		public Pathfinder(Implementation implementation, Implementation.UserProvidedNode start)
		{
			this.implementation = implementation;
			this.exploredNodes = new Dictionary<TNodeID, Implementation.Node>(implementation.nodeIDComparer);
			this.boundary = new PriorityQueue<TCost, Implementation.Node>(implementation.costComparer);

			var startNode = Implementation.MakeStartNode(start);
			exploredNodes.Add(startNode.id, startNode);
			ProcessNeighbours(startNode);
			m_latestStepEvent = new Implementation.ExploredNodeStepEvent(startNode);
		}

		/// <summary>WARNING: Takes ownership of the collections.</summary>
		public Pathfinder(
			Implementation implementation,
			Dictionary<TNodeID, Implementation.Node> exploredNodes,
			PriorityQueue<TCost, Implementation.Node> boundary,
			/* not-nullable */ IPathfinderStepEvent latestStepEvent)
		{
			if (exploredNodes.Comparer != implementation.nodeIDComparer)
			{
				throw new ArgumentException(nameof(exploredNodes) + " collection should share implementation's " + nameof(implementation.nodeIDComparer), nameof(exploredNodes));
			}
			if (boundary.Comparer != implementation.costComparer)
			{
				throw new ArgumentException(nameof(boundary) + " collection should share implementation's " + nameof(implementation.costComparer), nameof(boundary));
			}
			if (latestStepEvent == null)
			{
				throw new ArgumentNullException(nameof(latestStepEvent));
			}

			this.implementation = implementation;
			this.exploredNodes = exploredNodes;
			this.boundary = boundary;
			this.latestStepEvent = latestStepEvent;
		}



		public IPathfinderStepEvent Step(Implementation.Node current)
		{
			if (exploredNodes.TryGetValue(current.id, out var existing))
			{
				if (implementation.IsLessThan(current.totalCost, existing.totalCost))
				{
					var previousState = existing.state;
					existing.state = current.state;
					ProcessNeighbours(existing);
					latestStepEvent = new Implementation.ReplacedNodeStepEvent(existing, previousState);
					return latestStepEvent;
				}
				else
				{
					latestStepEvent = new Implementation.DidNotReplaceNodeStepEvent(existing, ignoredNewNode: current);
					return latestStepEvent;
				}
			}
			else
			{
				exploredNodes.Add(current.id, current);
				ProcessNeighbours(current);
				latestStepEvent = new Implementation.ExploredNodeStepEvent(current);
				return latestStepEvent;
			}
		}

		public IPathfinderStepEvent Step()
		{
			if (boundary.TryDequeue(out var current))
			{
				return Step(current);
			}
			else
			{
				latestStepEvent = new ExhaustedBoundaryStepEvent();
				return latestStepEvent;
			}
		}

		public void ProcessNeighbours(Implementation.Node parent)
		{
			Implementation.IReadOnlyNode readonlyParent = parent;

			var passed = new HashSet<TNodeID>(implementation.nodeIDComparer);

			foreach (var neighbour in implementation.Neighbours(readonlyParent))
			{
				var to = neighbour.destination.id;

				if (implementation.IsEqual(to, parent.id))
				{
					implementation.HandleNeighbourException(new Implementation.SameAsParentNeighbourException(), readonlyParent, neighbour);
				}
				else if (passed.Contains(to))
				{
					implementation.HandleNeighbourException(new Implementation.DuplicateNeighbourException(), readonlyParent, neighbour);
				}
				else
				{
					passed.Add(to);
					var node = Implementation.MakeNodeFromValidNeighbour(parent, neighbour);
					boundary.Add(node.totalCost, node);
				}
			}
		}



		public IPathfinderStepEvent StepUntilBoundaryExhausted()
		{
			while (!(latestStepEvent is ExhaustedBoundaryStepEvent))
			{
				Step();
			}
			return latestStepEvent;
		}



		public abstract class Implementation
		{
			public abstract IEnumerable<UserProvidedEdge> Neighbours(IReadOnlyNode parent);

			public virtual void HandleNeighbourException(NeighbourException neighbourException, IReadOnlyNode parent, UserProvidedEdge neighbour)
				=> throw neighbourException;



			public Implementation(
				IEqualityComparer<TNodeID> nodeIDComparer,
				IComparer<TCost> costComparer
			)
			{
				this.nodeIDComparer = nodeIDComparer;
				this.costComparer = costComparer;
			}

			public Implementation() : this(
				EqualityComparer<TNodeID>.Default,
				Comparer<TCost>.Default
			)
			{ }



			public IEqualityComparer<TNodeID> nodeIDComparer { get; private set; }
			public IComparer<TCost> costComparer { get; private set; }



			public bool IsEqual(TNodeID a, TNodeID b) => nodeIDComparer.Equals(a, b);
			public bool IsLessThan(TCost a, TCost b) => costComparer.Compare(a, b) < 0;
			public bool IsEqual(TCost a, TCost b) => costComparer.Compare(a, b) == 0;



			#region Types



			// can be constructed by user
			public class UserProvidedEdge
			{
				public UserProvidedEdge(
					UserProvidedNode destination,
					TExtraEdgeData extraData)
				{
					if (destination == null)
						throw new ArgumentNullException(nameof(destination));

					this.destination = destination;
					this.extraData = extraData;
				}

				public UserProvidedNode destination { get; private set; }
				public TExtraEdgeData extraData { get; private set; }
			}

			// can be constructed by user
			public class UserProvidedNode
			{
				public UserProvidedNode(
					TNodeID id,
					TCost totalCost,
					TExtraNodeData extraData)
				{
					this.id = id;
					this.totalCost = totalCost;
					this.extraData = extraData;
				}

				public TNodeID id { get; private set; }
				public TCost totalCost { get; private set; }
				public TExtraNodeData extraData { get; private set; }
			}



			public class NeighbourException : Exception
			{
				public NeighbourException(string message, Exception innerException)
					: base(message, innerException)
				{ }

				public NeighbourException(string message)
					: base(message)
				{ }

				public NeighbourException()
					: this(nameof(NeighbourException))
				{ }
			}

			public class SameAsParentNeighbourException : NeighbourException
			{
				public SameAsParentNeighbourException(string message, Exception innerException)
					: base(message, innerException)
				{ }

				public SameAsParentNeighbourException(string message)
					: base(message)
				{ }

				public SameAsParentNeighbourException()
					: base(nameof(SameAsParentNeighbourException))
				{ }
			}

			public class DuplicateNeighbourException : NeighbourException
			{
				public DuplicateNeighbourException(string message, Exception innerException)
					: base(message, innerException)
				{ }

				public DuplicateNeighbourException(string message)
					: base(message)
				{ }

				public DuplicateNeighbourException()
					: base(nameof(SameAsParentNeighbourException))
				{ }
			}



			public interface INodePathfinderStepEvent : IPathfinderStepEvent
			{
				IReadOnlyNode node { get; }
			}

			public sealed class ReplacedNodeStepEvent : INodePathfinderStepEvent
			{
				public ReplacedNodeStepEvent(
					IReadOnlyNode node,
					IReadOnlyNodeState previousState)
				{
					this.node = node;
					this.previousState = previousState;
				}

				public IReadOnlyNode node { get; private set; }
				public IReadOnlyNodeState previousState { get; private set; }
			}

			public sealed class DidNotReplaceNodeStepEvent : INodePathfinderStepEvent
			{
				public DidNotReplaceNodeStepEvent(
					IReadOnlyNode node,
					IReadOnlyNode ignoredNewNode)
				{
					this.node = node;
					this.ignoredNewNode = ignoredNewNode;
				}

				public IReadOnlyNode node { get; private set; }
				public IReadOnlyNode ignoredNewNode { get; private set; }
			}

			public sealed class ExploredNodeStepEvent : INodePathfinderStepEvent
			{
				public ExploredNodeStepEvent(IReadOnlyNode node)
				{
					this.node = node;
				}

				public IReadOnlyNode node { get; private set; }
			}



			public interface IReadOnlyEdge
			{
				IReadOnlyNode parent { get; }
				TExtraEdgeData extraData { get; }
			}

			/// <summary>
			///		<para>Immutable value type. Each <see cref="Node"/> owns a unique instance of this class.</para>
			///		<para>WARNING: Not all value type operations are implemented.</para>
			/// </summary>
			public sealed class Edge : IReadOnlyEdge
			{
				public Edge(Node parent, TExtraEdgeData extraData)
				{
					this.parent = parent;
					this.extraData = extraData;
				}

				public Node parent { get; private set; }
				IReadOnlyNode IReadOnlyEdge.parent => parent;

				public TExtraEdgeData extraData { get; private set; }
			}



			public interface IReadOnlyNodeState
			{
				/*Nullable*/
				IReadOnlyEdge edge { get; }
				TCost totalCost { get; }
				TExtraNodeData extraData { get; }
			}

			public sealed class NodeState : IReadOnlyNodeState
			{
				public NodeState(/*Nullable*/ Edge edge, TCost totalCost, TExtraNodeData extraData)
				{
					this.edge = edge;
					this.totalCost = totalCost;
					this.extraData = extraData;
				}

				public /*Nullable*/ Edge edge { get; private set; }
				/*Nullable*/
				IReadOnlyEdge IReadOnlyNodeState.edge => edge;

				public TCost totalCost { get; private set; }
				public TExtraNodeData extraData { get; private set; }
			}



			public interface IReadOnlyNode
			{
				TNodeID id { get; }

				IReadOnlyNodeState state { get; }
				TCost totalCost { get; } // should be an extension property but cannot due to language limitations
				TExtraNodeData extraData { get; } // should be an extension property but cannot due to language limitations

				/*Nullable*/
				IReadOnlyEdge edge { get; }
				bool hasEdge { get; } // should be an extension property but cannot due to language limitations
				/*Nullable*/
				IReadOnlyNode parent { get; } // should be an extension property but cannot due to language limitations
			}

			public sealed class Node : IReadOnlyNode
			{
				public Node(TNodeID id, NodeState state)
				{
					this.id = id;
					this.state = state;
				}



				public TNodeID id { get; private set; }



				public NodeState state { get; set; } // NOTE: mutable state.
				IReadOnlyNodeState IReadOnlyNode.state => state;

				public TCost totalCost => state.totalCost;
				public TExtraNodeData extraData => state.extraData;



				public /*Nullable*/ Edge edge => state.edge;
				/*Nullable*/
				IReadOnlyEdge IReadOnlyNode.edge => edge;

				public bool hasEdge => edge != null;

				public /*Nullable*/ Node parent => edge?.parent;
				/*Nullable*/
				IReadOnlyNode IReadOnlyNode.parent => parent;
			}



			public static Node MakeNodeFromValidNeighbour(Node parent, UserProvidedEdge neighbour)
				=> MakeNode(new Edge(parent, neighbour.extraData), neighbour.destination);

			public static Node MakeStartNode(UserProvidedNode userProvidedNode)
				=> MakeNode(edge: null, userProvidedNode);

			public static Node MakeNode(Edge edge, UserProvidedNode userProvidedNode)
			{
				var state = new NodeState(edge: edge, userProvidedNode.totalCost, userProvidedNode.extraData);
				var node = new Node(userProvidedNode.id, state);
				return node;
			}



			#endregion
		}
	}



	public static class PathfinderNodeUtils
	{
		public static List<Pathfinder<TNodeID, TCost, TExtraNodeData, TExtraEdgeData>.Implementation.Node>
			GetPath<TNodeID, TCost, TExtraNodeData, TExtraEdgeData>(
			this Pathfinder<TNodeID, TCost, TExtraNodeData, TExtraEdgeData>.Implementation.Node node,
			bool reverse = true)
		{
			var list = new List<Pathfinder<TNodeID, TCost, TExtraNodeData, TExtraEdgeData>.Implementation.Node>();
			node.GetPathImpl(list);
			if (reverse)
				list.Reverse();
			return list;
		}

		public static List<Pathfinder<TNodeID, TCost, TExtraNodeData, TExtraEdgeData>.Implementation.IReadOnlyNode>
			GetPath<TNodeID, TCost, TExtraNodeData, TExtraEdgeData>(
			this Pathfinder<TNodeID, TCost, TExtraNodeData, TExtraEdgeData>.Implementation.IReadOnlyNode node,
			bool reverse = true)
		{
			var list = new List<Pathfinder<TNodeID, TCost, TExtraNodeData, TExtraEdgeData>.Implementation.IReadOnlyNode>();
			node.GetPathImpl(list);
			if (reverse)
				list.Reverse();
			return list;
		}



		private static void GetPathImpl<TNodeID, TCost, TExtraNodeData, TExtraEdgeData>(
			this Pathfinder<TNodeID, TCost, TExtraNodeData, TExtraEdgeData>.Implementation.Node node,
			ICollection<Pathfinder<TNodeID, TCost, TExtraNodeData, TExtraEdgeData>.Implementation.Node> list)
		{
			list.Add(node);
			node.edge?.parent.GetPathImpl(list);
		}

		private static void GetPathImpl<TNodeID, TCost, TExtraNodeData, TExtraEdgeData>(
			this Pathfinder<TNodeID, TCost, TExtraNodeData, TExtraEdgeData>.Implementation.IReadOnlyNode node,
			ICollection<Pathfinder<TNodeID, TCost, TExtraNodeData, TExtraEdgeData>.Implementation.IReadOnlyNode> list)
		{
			list.Add(node);
			node.edge?.parent.GetPathImpl(list);
		}



		//public Implementation.Node[] FindChildrenOfParent(Implementation.Node parentNode) =>
		//	exploredNodes.Values
		//	.Where(childNode => childNode.parent == parentNode)
		//	.ToArray();

		//public Dictionary<TNodeID, Implementation.Node[]> FindChildren() =>
		//	exploredNodes.ToDictionary(
		//	kvp => kvp.Key,
		//	kvp => kvp.Value.Pipe(FindChildrenOfParent)
		//	);
	}
}