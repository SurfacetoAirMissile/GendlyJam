using System;
using UnityEngine;

namespace CGDP2.Utilities
{
	public struct ZeroVariance
	{
		public float threshold;

		public ZeroVariance(float threshold) { this.threshold = threshold; }

		public static ZeroVariance Default => new ZeroVariance(float.Epsilon * 5.0f);
		public static ZeroVariance TrueZero => new ZeroVariance(0.0f);

		public bool IsValueApproximatelyEqualToZero(float value) => Mathf.Abs(value) <= threshold;
		public bool IsValueApproximatelyNotEqualToZero(float value) => Mathf.Abs(value) > threshold;
		public bool IsValueApproximatelyLessThanZero(float value) => value < -threshold;
		public bool IsValueApproximatelyGreaterThanZero(float value) => value > +threshold;
		public bool IsValueApproximatelyLessThanOrEqualToZero(float value) => value <= +threshold;
		public bool IsValueApproximatelyGreaterThanOrEqualToZero(float value) => value >= -threshold;

		public static bool operator ==(float value, ZeroVariance variance) => variance.IsValueApproximatelyEqualToZero(value);
		public static bool operator !=(float value, ZeroVariance variance) => variance.IsValueApproximatelyNotEqualToZero(value);

		public static bool operator ==(ZeroVariance variance, float value) => variance.IsValueApproximatelyEqualToZero(value);
		public static bool operator !=(ZeroVariance variance, float value) => variance.IsValueApproximatelyNotEqualToZero(value);

		public static bool operator <(float value, ZeroVariance variance) => variance.IsValueApproximatelyLessThanZero(value);
		public static bool operator >(float value, ZeroVariance variance) => variance.IsValueApproximatelyGreaterThanZero(value);

		public static bool operator <(ZeroVariance variance, float value) => variance.IsValueApproximatelyGreaterThanZero(value);
		public static bool operator >(ZeroVariance variance, float value) => variance.IsValueApproximatelyLessThanZero(value);

		public static bool operator <=(float value, ZeroVariance variance) => variance.IsValueApproximatelyLessThanOrEqualToZero(value);
		public static bool operator >=(float value, ZeroVariance variance) => variance.IsValueApproximatelyGreaterThanOrEqualToZero(value);

		public static bool operator <=(ZeroVariance variance, float value) => variance.IsValueApproximatelyGreaterThanOrEqualToZero(value);
		public static bool operator >=(ZeroVariance variance, float value) => variance.IsValueApproximatelyLessThanOrEqualToZero(value);

		public override bool Equals(object obj)
		{
			if (obj is ZeroVariance variance)
				return threshold == variance.threshold;

			try
			{
				var value = Convert.ToSingle(obj);
				return IsValueApproximatelyEqualToZero(value);
			}
			catch
			{
			}

			return false;
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return threshold.GetHashCode() * 7;
			}
		}
	}
}
