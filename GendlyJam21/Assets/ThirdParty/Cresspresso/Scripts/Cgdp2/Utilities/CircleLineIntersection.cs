using System;
using UnityEngine;

namespace CGDP2.Utilities.Geometry2D
{
	public enum CircleLineIntersectionType
	{
		Complex = 0,
		Tangent = 1,
		TwoPoints = 2,
	}

	/// <summary>
	/// The result of intersecting a line with a circle.
	/// </summary>
	/// <seealso cref="Intersect(Ray2D, float, float)"/>
	/// <changelog>
	///		<log author="Elijah Shadbolt" date="22/02/2021">
	///			Created this class.
	///		</log>
	///	</changelog>
	///	
	public struct CircleLineIntersection
	{
		public static CircleLineIntersection Intersect(
			Ray2D ray,
			float radius,
			Vector2 centre,
			ZeroVariance zeroDiscriminant
			)
			=> IntersectCircleAtOrigin(
				new Ray2D(ray.origin - centre, ray.direction),
				radius,
				zeroDiscriminant
				).Translate(centre);

		/// <summary>
		///		Assumes circle centre at origin (0, 0), therefore ray origin must be relative to circle centre.
		/// </summary>
		/// <remarks>
		///		https://mathworld.wolfram.com/Circle-LineIntersection.html
		/// </remarks>
		public static CircleLineIntersection IntersectCircleAtOrigin(
			Ray2D ray,
			float radius,
			ZeroVariance zeroDiscriminant
			)
		{
			var direction = ray.direction;
			var dx = direction.x;
			var dy = direction.y;
			var dr = direction.magnitude;

			var p1 = ray.origin;
			var x1 = p1.x;
			var y1 = p1.y;

			var D = x1 * dy - y1 * dx;
			var dr_sq = dr * dr;
			var discriminant = radius * radius * dr_sq - D * D;
			if (discriminant < zeroDiscriminant)
			{
				return CircleLineIntersection.Complex();
			}
			var sqrt_disc = Mathf.Sqrt(discriminant);

			if (discriminant == zeroDiscriminant)
			{
				var qx = (D * dy) / dr_sq;
				var qy = (-D * dx) / dr_sq;
				var q = new Vector2(qx, qy);
				return CircleLineIntersection.Tangent(q);
			}

			var sx = Mathf.Sign(dy) * dx * sqrt_disc;
			var qx1 = (D * dy + sx) / dr_sq;
			var qx2 = (D * dy - sx) / dr_sq;

			var sy = Mathf.Abs(dy) * sqrt_disc;
			var qy2 = (-D * dx - sy) / dr_sq;
			var qy1 = (-D * dx + sy) / dr_sq;

			var q1 = new Vector2(qx1, qy1);
			var q2 = new Vector2(qx2, qy2);
			return CircleLineIntersection.TwoPoints(q1, q2);
		}

		public static CircleLineIntersection TwoPoints(Vector2 point1, Vector2 point2)
			=> new CircleLineIntersection(CircleLineIntersectionType.TwoPoints, point1, point2);

		public static CircleLineIntersection Tangent(Vector2 point)
			=> new CircleLineIntersection(CircleLineIntersectionType.Tangent, point, CgdUtils.Vec2NaN);

		public static CircleLineIntersection Complex()
			=> new CircleLineIntersection(CircleLineIntersectionType.Complex, CgdUtils.Vec2NaN, CgdUtils.Vec2NaN);

		public R Match<R>(
			Func<R> whenComplex,
			Func<Vector2, R> whenTangent,
			Func<Vector2, Vector2, R> whenTwoPoints
			)
		{
			switch (type)
			{
				default:
				case CircleLineIntersectionType.Complex: return whenComplex();
				case CircleLineIntersectionType.Tangent: return whenTangent(m_point1);
				case CircleLineIntersectionType.TwoPoints: return whenTwoPoints(m_point1, m_point2);
			}
		}

		public void Match(
			Action whenComplex,
			Action<Vector2> whenTangent,
			Action<Vector2, Vector2> whenTwoPoints
			)
		{
			switch (type)
			{
				default:
				case CircleLineIntersectionType.Complex: whenComplex(); break;
				case CircleLineIntersectionType.Tangent: whenTangent(m_point1); break;
				case CircleLineIntersectionType.TwoPoints: whenTwoPoints(m_point1, m_point2); break;
			}
		}

		public CircleLineIntersection Transform(
			Func<Vector2, Vector2> whenTangent,
			Func<Vector2, Vector2, (Vector2, Vector2)> whenTwoPoints
			)
		{
			switch (type)
			{
				default:
				case CircleLineIntersectionType.Complex:
					return Complex();

				case CircleLineIntersectionType.Tangent:
					return Tangent(whenTangent(m_point1));

				case CircleLineIntersectionType.TwoPoints:
					return whenTwoPoints(m_point1, m_point2)
						.Pipe(tup => TwoPoints(tup.Item1, tup.Item2));
			}
		}

		public CircleLineIntersection Translate(Vector2 vector)
		{
			return Transform(
				whenTangent: q => q + vector,
				whenTwoPoints: (q1, q2) => (q1 + vector, q2 + vector)
				);
		}

		public CircleLineIntersectionType type => m_type;

		public Vector2 point => type != CircleLineIntersectionType.Tangent
			? Throw()
			: m_point1;

		public Vector2 point1 => type == CircleLineIntersectionType.Complex
			? Throw()
			: m_point1;

		public Vector2 point2 => type == CircleLineIntersectionType.Complex
			? Throw()
			: m_point2;

		private Vector2 Throw() => throw new InvalidOperationException("cannot get that property from this intersection type");

		private CircleLineIntersection(
			CircleLineIntersectionType type,
			Vector2 point1,
			Vector2 point2
			)
		{
			m_type = type;
			m_point1 = point1;
			m_point2 = point2;
		}

		private CircleLineIntersectionType m_type;
		private Vector2 m_point1, m_point2;
	}
}
