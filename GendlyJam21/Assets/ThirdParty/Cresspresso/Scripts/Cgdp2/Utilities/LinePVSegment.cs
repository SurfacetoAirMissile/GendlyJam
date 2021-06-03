using System.Collections.Generic;
using System;
using UnityEngine;

namespace CGDP2.Utilities.Geometry2D
{
	/// <summary>
	/// Given a ray originating from a point on a line,
	/// whose direction is perpendicular to the line,
	/// then on which side does the point lie?
	/// </summary>
	public enum SideOfLineNormal
	{
		Forward = 1,
		Perpendicular = 0,
		Backward = -1,
	}

	/// <summary>
	///		A line segment represented by a start point and a vector spanning the length of the line segment.
	///		The PV stands for "Point and Vector".
	/// </summary>
	/// <changelog>
	///		<log author="Elijah Shadbolt" date="22/02/2021">
	///			Created this class.
	///		</log>
	///	</changelog>
	///	
	public struct LinePVSegment
	{
		public LinePVSegment(Vector2 start, Vector2 vector)
		{
			this.start = start;
			this.vector = vector;
		}

		public static LinePVSegment FromStartAndEnd(Vector2 start, Vector2 end)
			=> new LinePVSegment(start, end - start);

		public static LinePVSegment FromRay2D(Ray2D ray, float distance)
			=> new LinePVSegment(ray.origin, ray.direction.normalized * distance);



		public Vector2 start;
		public Vector2 vector;

		public Vector2 End() => start + vector;
		public Vector2 Lerp(float t) => start + vector * t;
		public Vector2 Midpoint() => Lerp(0.5f);

		public float Length() => vector.magnitude;
		public float SqrLength() => vector.sqrMagnitude;



		/// <summary>
		///		Splits a line segment into equidistant points.
		/// </summary>
		/// <param name="numPoints">
		///		The number of points to return,
		///		excluding the start and end points of the line segment.
		/// </param>
		/// <returns>
		///		A sequence of points evenly distributed along the line segment.
		///		Count == <paramref name="numPoints"/>.
		///		Note that the start and end points of the line segment are excluded.
		/// </returns>
		/// 
		public IEnumerable<Vector2> Subdivide(int numPoints)
		{
			if (numPoints <= 0)
				throw new ArgumentException(nameof(numPoints) + " must be > 0", nameof(numPoints));

			var c = numPoints + 2;
			for (int j = 1; j < c; j++)
			{
				var t = (float)j / c;
				yield return Lerp(t);
			}
		}



		public Ray2D ToRay2D() => new Ray2D(start, vector);
		public static explicit operator Ray2D(LinePVSegment lineSegment) => lineSegment.ToRay2D();



		/// <param name="normal">
		///		A direction perpendicular to the line.
		///		Not necessarily a unit vector.
		/// </param>
		public static bool IsPointInFrontOfLineNormal(
			Vector2 pointOnLine,
			Vector2 normal,
			Vector2 pointToCompare
			)
		{
			var vec = pointToCompare - pointOnLine;
			var dot = Vector2.Dot(normal, vec);
			return dot > 0;
		}

		/// <param name="normal">
		///		A direction perpendicular to the line.
		///		Must be a unit vector.
		/// </param>
		public static SideOfLineNormal WhichSideOfLineNormalIsThePoint(
			Vector2 pointOnLine,
			Vector2 normal,
			Vector2 pointToCompare,
			ZeroVariance zeroDot
			)
		{
			var vec = pointToCompare - pointOnLine;
			var dot = Vector2.Dot(normal, vec.normalized);

			if (dot > zeroDot)
				return SideOfLineNormal.Forward;

			if (dot < zeroDot)
				return SideOfLineNormal.Backward;

			return SideOfLineNormal.Perpendicular;
		}
	}
}
