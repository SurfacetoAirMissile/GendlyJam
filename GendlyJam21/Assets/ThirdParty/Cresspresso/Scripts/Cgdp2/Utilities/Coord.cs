using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace CGDP2.Utilities
{
	/// <changelog>
	///		<log author="Elijah Shadbolt" date="04/02/2021">
	///			Created this script.
	///		</log>
	/// </changelog>
	/// 
	public struct Coord : IEquatable<Coord>
	{
		public int column, row;
		public Coord(int column, int row)
		{
			this.column = column;
			this.row = row;
		}
		public static Coord zero => new Coord();

		public static explicit operator Coord(Vector2Int vector) => new Coord(column: vector.x, row: vector.y);
		public static explicit operator Vector2Int(Coord coord) => new Vector2Int(x: coord.column, y: coord.row);

		public static Coord operator +(Coord start, Coord translation) => new Coord(start.column + translation.column, start.row + translation.row);
		public static Coord operator -(Coord start, Coord translation) => new Coord(start.column - translation.column, start.row - translation.row);
		public static Coord operator +(Coord value) => value;
		public static Coord operator -(Coord value) => new Coord(-value.column, -value.row);

		public static bool operator ==(Coord lhs, Coord rhs) => lhs.column == rhs.column && lhs.row == rhs.row;
		public static bool operator !=(Coord lhs, Coord rhs) => !(lhs == rhs);
		public bool Equals(Coord other) => this == other;
		public override bool Equals(object obj)
		{
			if (obj is Coord coord)
				return this == coord;

			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return column.GetHashCode() * 3 + row.GetHashCode() * 17;
			}
		}

		public override string ToString() => $"{nameof(Coord)}({nameof(column)}: {column}, {nameof(row)}: {row})";
	}



	public static class CoordUtils
	{
		public static IReadOnlyList<Coord> NeighbourOffsets => m_NeighbourOffsets.Value;
		private static readonly Lazy<IReadOnlyList<Coord>> m_NeighbourOffsets = new Lazy<IReadOnlyList<Coord>>(
			() => (new Coord[4] {
				new Coord(1, 0),
				new Coord(0, 1),
				new Coord(-1, 0),
				new Coord(0, -1),
			}));

		public static IEnumerable<Coord> Neighbours(this Coord coord) => NeighbourOffsets.Select(offset => coord + offset);

		public static int ManhattanHeuristic(this Coord lhs, Coord rhs)
		{
			var v = lhs - rhs;
			return Mathf.Abs(v.row) + Mathf.Abs(v.column);
		}
	}
}
