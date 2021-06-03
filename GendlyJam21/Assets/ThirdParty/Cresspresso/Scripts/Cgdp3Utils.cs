///
/// This script was taken from a separate project Elijah Shadbolt worked on.
/// It was copied over 03/06/2021.
///

using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public static class Cgdp3Utils
{
	public static T IdentityAssertExists<T>(
		this T obj,
		string name = "Object",
		UnityEngine.Object context = null
		) where T : UnityEngine.Object
	{
		if (!obj)
		{
			Debug.LogError(name + " is null", context);
			return null;
		}
		return obj;
	}



	// https://forum.unity.com/threads/how-to-calculate-horizontal-field-of-view.16114/
	//
	public static float HorizontalFieldOfView(float verticalFov, float aspect)
	{
		var vFOVrad = verticalFov * Mathf.Deg2Rad;
		var cameraHeightAt1 = Mathf.Tan(vFOVrad * 0.5f);
		var hFOVrad = Mathf.Atan(cameraHeightAt1 * aspect) * 2;
		var hFOV = hFOVrad * Mathf.Rad2Deg;
		return hFOV;
	}




	/// <summary>
	/// Copies elements from a source array to a destination array,
	/// cycling from the start of a slice in the source array
	/// if the destination length extends past the length of the slice.
	/// </summary>
	/// <param name="sourceArray">The array to copy elements from.</param>
	/// <param name="sourceIndex">
	/// Where copying should start from.
	/// An index into the source array.
	/// If this index is past the end of the slice, it is cycled into the slice beforehand.
	/// </param>
	/// <param name="sourceSliceIndex">Where it will loop back to. The start of the span looking into the source array.</param>
	/// <param name="sourceSliceLength">The length of the span looking into the source array.</param>
	/// <param name="destinationArray">The array to copy elements into.</param>
	/// <param name="destinationIndex">Where elements should be copied to. The start of the span looking into the destination array.</param>
	/// <param name="destinationLength">How many elements should be copied into the destination array.</param>
	public static void CopyArrayCyclic(
		Array sourceArray,
		int sourceIndex,
		int sourceSliceIndex,
		int sourceSliceLength,
		Array destinationArray,
		int destinationIndex,
		int destinationLength
		)
	{
		// Validate arguments.
		{
			if (sourceArray is null)
			{
				throw new ArgumentNullException(nameof(sourceArray));
			}

			if (destinationArray is null)
			{
				throw new ArgumentNullException(nameof(destinationArray));
			}

			if (sourceSliceIndex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(sourceSliceIndex));
			}

			if (sourceSliceLength <= 0)
			{
				throw new ArgumentException(nameof(sourceSliceLength));
			}

			if (sourceSliceIndex + sourceSliceLength > sourceArray.Length)
			{
				throw new ArgumentException(nameof(sourceSliceLength));
			}

			if (destinationIndex < 0)
			{
				throw new ArgumentOutOfRangeException(nameof(destinationIndex));
			}

			if (destinationLength < 0)
			{
				throw new ArgumentException(nameof(destinationLength));
			}

			if (destinationIndex + destinationLength > destinationArray.Length)
			{
				throw new ArgumentException(nameof(destinationLength));
			}
		}

		if (destinationLength == 0)
		{
			return;
		}

		if (sourceIndex >= sourceSliceIndex + sourceSliceLength)
		{
			sourceIndex = (sourceIndex - sourceSliceIndex) % sourceSliceLength + sourceSliceIndex;
		}

		int sourceTempLength = sourceSliceIndex + sourceSliceLength - sourceIndex;

		while (sourceTempLength < destinationLength)
		{
			Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, sourceTempLength);
			destinationIndex += sourceTempLength;
			destinationLength -= sourceTempLength;
			sourceIndex = sourceSliceIndex;
			sourceTempLength = sourceSliceLength;
		}

		Array.Copy(sourceArray, sourceIndex, destinationArray, destinationIndex, destinationLength);
	}



	public static IEnumerable<T> EnumerateReversed<T>(this IReadOnlyList<T> list)
	{
		int i = list.Count;
		while (i > 0)
		{
			--i;
			yield return list[i];
		}
	}





	// https://stackoverflow.com/questions/1190657/add-two-lists-of-different-length-in-c-sharp
	// credit to Damian on stackoverflow
	//
	public static IEnumerable<TResult>
		Merge<TFirst, TSecond, TResult>(
		this IEnumerable<TFirst> first,
			IEnumerable<TSecond> second,
			Func<TFirst, TSecond, TResult> operation
		)
	{
		using (var iter1 = first.GetEnumerator())
		{
			using (var iter2 = second.GetEnumerator())
			{
				while (iter1.MoveNext())
				{
					if (iter2.MoveNext())
					{
						yield return operation(iter1.Current, iter2.Current);
					}
					else
					{
						yield return operation(iter1.Current, default(TSecond));
					}
				}
				while (iter2.MoveNext())
				{
					yield return operation(default(TFirst), iter2.Current);
				}
			}
		}
	}
}