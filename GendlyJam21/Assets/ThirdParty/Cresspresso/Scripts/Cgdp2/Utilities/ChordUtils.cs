using System.Collections.Generic;
using UnityEngine;

namespace CGDP2.Utilities.Geometry2D
{
	public static class ChordUtils
	{
		/// <param name="chordLength">
		///		The distance from one chord point (where the chord intersects the circle edge) to the other chord point.
		/// </param>
		/// <param name="chordWidth">
		///		The distance from the midpoint of the chord to the edge of the circle.
		/// </param>
		public static float CalcCircleRadiusOfChord(
			float chordLength,
			float chordWidth
			)
		{
			var halfChordLength = chordLength * 0.5f;
			var a = Mathf.Atan2(halfChordLength, chordWidth);
			var phi = Mathf.PI - 2 * a;
			var circleRadius = halfChordLength / Mathf.Sin(phi);
			return circleRadius;
		}



		/// <param name="chordWidth">
		///		The distance from the midpoint of the chord to the edge of the circle.
		/// </param>
		public static Vector2 CalcCircleCentreOfChord(
			float circleRadius,
			Vector2 chordPointAcw,
			Vector2 chordPointCw,
			float chordWidth
			)
		{
			var chordVector = chordPointCw - chordPointAcw;
			var chordMidpoint = chordPointAcw + chordVector * 0.5f;
			var perp = Vector2.Perpendicular(chordVector).normalized;
			var distance = circleRadius - chordWidth;
			var circleCentre = chordMidpoint - distance * perp;
			return circleCentre;
		}



		/// <summary>
		/// 
		/// </summary>
		/// <param name="chord">
		///		Both the start point and the end point are on the edge of the circle.
		///		The start point is anticlockwise from the end point around the centre of the circle.
		/// </param>
		/// <param name="chordWidth">
		///		Distance from midpoint of chord to edge of circle.
		/// </param>
		/// <param name="pointsOnChord">
		///		Can be created with <see cref="LineUtils.SubdivideLineSegment(Vector2, Vector2, int)"/>,
		///		passing in <paramref name="chordPointAcw"/> and <paramref name="chordPointCw"/>.
		/// </param>
		/// <param name="zeroThreshold"></param>
		/// <returns>
		///		Points on circle edge, excluding chord start and end points.
		/// </returns>
		public static IEnumerable<Vector2>
			ProjectChordPointsToCircleEdge(
			LinePVSegment chord,
			float chordWidth,
			IEnumerable<Vector2> pointsOnChord,
			ZeroVariance zeroIntersectionDiscriminant,
			ZeroVariance zeroDegrees
			)
		{
			var halfChordLength = chord.Length() * 0.5f;
			var chordNormal = Vector2.Perpendicular(chord.vector).normalized;

			var a = Mathf.Atan2(halfChordLength, chordWidth);
			var phi = Mathf.PI - 2 * a;
			var circleRadius = halfChordLength / Mathf.Sin(phi);
			var distanceFromCircleCentreToChord = circleRadius - chordWidth;
			var chordMidpoint = chord.Midpoint();
			var circleCentre = chordMidpoint - distanceFromCircleCentreToChord * chordNormal;

			foreach (var pointOnChord in pointsOnChord)
			{
				yield return ProjectChordPointToCircleEdge(
					pointOnChord:	pointOnChord,
					chordNormal:	chordNormal,
					circleRadius:	circleRadius,
					circleCentre:	circleCentre,
					zeroIntersectionDiscriminant: zeroIntersectionDiscriminant
					);
			}
		}



		public static Vector2 ProjectChordPointToCircleEdge(
			Vector2 pointOnChord,
			Vector2 chordNormal,
			float circleRadius,
			Vector2 circleCentre,
			ZeroVariance zeroIntersectionDiscriminant
			)
		{
			var projRay = new Ray2D(pointOnChord, chordNormal);

			var intersection = CircleLineIntersection.Intersect(
				projRay,
				circleRadius,
				circleCentre,
				zeroIntersectionDiscriminant
				);

			var pointOnEdgeOfCircle = intersection.Match<Vector2>(
				whenComplex: () =>
				{
					Debug.LogError("Expected two intersection points");
					return pointOnChord;
				},
				whenTangent: point =>
				{
					return point;
				},
				whenTwoPoints: (p1, p2) =>
				{
					var isP1InFrontOfChord = LinePVSegment.IsPointInFrontOfLineNormal(
						pointOnLine: pointOnChord,
						normal: chordNormal,
						pointToCompare: p1
						);
					return isP1InFrontOfChord ? p1 : p2;
				}
				);

			return pointOnEdgeOfCircle;
		}
	}
}
