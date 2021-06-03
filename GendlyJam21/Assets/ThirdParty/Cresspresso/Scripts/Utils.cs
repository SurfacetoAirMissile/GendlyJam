///
/// This script was taken from a separate project Elijah Shadbolt worked on.
/// It was copied over 03/06/2021.
///

using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public static class TransformUtils
{
	public static Vector2 Get2DLocalPosition(this Transform transform) => transform.localPosition;
	public static void Set2DLocalPosition(this Transform transform, Vector2 localPosition)
	{
		transform.localPosition = new Vector3(localPosition.x, localPosition.y, transform.localPosition.z);
	}

	public static Vector2 Get2DPosition(this Transform transform) => transform.position;
	public static void Set2DPosition(this Transform transform, Vector2 position)
	{
		transform.position = new Vector3(position.x, position.y, transform.position.z);
	}

	public static float Get2DWorldZ(this Transform transform) => transform.position.z;
	public static void Set2DWorldZ(this Transform transform, float z)
	{
		var v = transform.position;
		v.z = z;
		transform.position = v;
	}

	public static float Get2DLocalZ(this Transform transform) => transform.localPosition.z;
	public static void Set2DLocalZ(this Transform transform, float z)
	{
		var v = transform.localPosition;
		v.z = z;
		transform.localPosition = v;
	}

	public static Vector2 Get2DLocalScale(this Transform transform) => transform.localScale;
	public static void Set2DLocalScale(this Transform transform, Vector2 localScale)
	{
		transform.localScale = new Vector3(localScale.x, localScale.y, transform.localScale.z);
	}

	public static void Set2DLocalScaleUniform(this Transform transform, float uniformLocalScale)
	{
		transform.localScale = new Vector3(uniformLocalScale, uniformLocalScale, transform.localScale.z);
	}

	public static float GetLocalRoll(this Transform transform) => transform.localEulerAngles.z;
	public static void SetLocalRoll(this Transform transform, float degrees)
	{
		transform.localEulerAngles = new Vector3(0, 0, degrees);
	}

	public static float GetRoll(this Transform transform) => transform.eulerAngles.z;
	public static void SetRoll(this Transform transform, float degrees)
	{
		transform.eulerAngles = new Vector3(0, 0, degrees);
	}

	public static Transform[] ChildrenToArray(this Transform parent)
	{
		var children = new Transform[parent.childCount];
		for (int i = 0; i < parent.childCount; i++)
		{
			children[i] = parent.GetChild(i);
		}
		return children;
	}

	public static IEnumerable<Transform> Children(this Transform parent) => parent.Cast<Transform>();
}

public static class CollectionUtils
{
	public static bool TryGet<T>(this IReadOnlyList<T> list, int index, out T value)
	{
		if (index >= 0 && index < list.Count)
		{
			value = list[index];
			return true;
		}
		else
		{
			value = default;
			return false;
		}
	}

	public static T TryGetClassNullable<T>(this IReadOnlyList<T> list, int index) where T : class
	{
		if (TryGet(list, index, out var value))
		{
			return value;
		}
		else
		{
			return default;
		}
	}

	public static T? TryGetStructNullable<T>(this IReadOnlyList<T> list, int index) where T : struct
	{
		if (TryGet(list, index, out var value))
		{
			return value;
		}
		else
		{
			return default;
		}
	}



	public static int IndexOf<T>(this IReadOnlyList<T> list, T value)
	{
		if (list is T[] array)
			return Array.IndexOf(array, value);
		else if (list is List<T> s)
			return s.IndexOf(value);
		else
		{
			var index = 0;
			foreach (var item in list)
			{
				if (Equals(value, item))
				{
					return index;
				}
				++index;
			}
			return -1;
		}
	}

	public static void FillArray<T>(T[] array, int start, int length, Func<int, T> select)
	{
		if (start < 0)
			throw new ArgumentOutOfRangeException(nameof(start));

		if (length < 0)
			throw new ArgumentOutOfRangeException(nameof(length));

		if (select == null)
			throw new ArgumentNullException(nameof(select));

		var end = start + length;
		if (end < 0 || end > array.Length)
			throw new IndexOutOfRangeException("subrange out of range");

		for (int i = start; i < end; ++i)
			array[i] = select(i);
	}

	public static void FillArray<T>(T[] array, Func<int, T> select) => FillArray(array, 0, array.Length, select);

	public static T[] CreateArray<T>(int length, Func<int, T> select)
	{
		var array = new T[length];
		for (int i = 0; i < length; ++i)
		{
			array[i] = select(i);
		}
		return array;
	}

	public static int FindIndex<T>(this IReadOnlyList<T> list, Predicate<T> predicate)
	{
		if (list is List<T> s)
			return s.FindIndex(predicate);
		else
		{
			var index = 0;
			foreach (var item in list)
			{
				if (predicate(item))
				{
					return index;
				}
				++index;
			}
			return -1;
		}
	}

	public static bool TryPeek<T>(this Queue<T> queue, out T value)
	{
		if (queue.Any())
		{
			value = queue.Peek();
			return true;
		}
		else
		{
			value = default;
			return false;
		}
	}

	public static bool TryDequeue<T>(this Queue<T> queue, out T value)
	{
		if (queue.Any())
		{
			value = queue.Dequeue();
			return true;
		}
		else
		{
			value = default;
			return false;
		}
	}

	public static IEnumerator<(int index, T value)> ForEachIndexed<T>(this IEnumerator<T> it)
	{
		int i = 0;
		while (it.MoveNext())
		{
			yield return (i, it.Current);
		}
	}

	public class ForEachIndexedList<T> : IReadOnlyList<(int index, T element)>
	{
		public ForEachIndexedList(IReadOnlyList<T> list)
		{
			this.m_list = list;
		}

		public (int index, T element) this[int index] { get => (index, m_list[index]); }

		public int Count => m_list.Count;

		IEnumerator IEnumerable.GetEnumerator() => Enumerator();
		public IEnumerator<(int index, T element)> GetEnumerator() => Enumerator();
		private IEnumerator<(int index, T element)> Enumerator()
		{
			for (int index = 0; index < Count; ++index)
			{
				yield return this[index];
			}
		}

		private IReadOnlyList<T> m_list;
	}

	public static IEnumerable<(int index, T element)> IndexedElements<T>(this IReadOnlyList<T> list)
	{
		return new ForEachIndexedList<T>(list);
	}



	public static S AggregateAddInPlace<S, V>(this IEnumerable<V> items, S mutableSeed, Action<S, V> addInPlace)
		=> items.Aggregate(mutableSeed, FuncUtils.AggregateAddInPlace(addInPlace));



	public static void RemoveWhere<TKey, TValue>(
		this IDictionary<TKey, TValue> dictionary,
		Func<TKey, TValue, bool> predicate)
	{
		var deadKeys = new List<TKey>();
		foreach (var key in dictionary.Keys.ToArray())
		{
			var item = dictionary[key];
			if (predicate(key, item))
			{
				deadKeys.Add(key);
			}
		}
		foreach (var key in deadKeys)
		{
			dictionary.Remove(key);
		}
	}

	public static void AddToNestedCollection<TKey, TCollection, TValue>(this IDictionary<TKey, TCollection> dictionary, TKey key, TValue value, Func<TValue, TCollection> newCollection)
		where TCollection : ICollection<TValue>
	{
		if (dictionary.TryGetValue(key, out var collection))
		{
			collection.Add(value);
		}
		else
		{
			dictionary.Add(key, newCollection(value));
		}
	}

	public static Dictionary<TKey, TValue> WhereToDictionary<TKey, TValue>(
		this IReadOnlyDictionary<TKey, TValue> dictionary,
		Func<TKey, TValue, bool> predicate)
	{
		var result = new Dictionary<TKey, TValue>();
		foreach (var kvp in dictionary)
		{
			if (predicate(kvp.Key, kvp.Value))
			{
				result.Add(kvp.Key, kvp.Value);
			}
		}
		return result;
	}

	public static Dictionary<TKey, TValue> ToDictionary<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items)
	{
		return items.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
	}

	public static IEnumerable<(TKey Key, TValue Value)> EntriesAsTuples<TKey, TValue>(this IEnumerable<KeyValuePair<TKey, TValue>> items)
	{
		return items.Select(kvp => (Key: kvp.Key, Value: kvp.Value));
	}



	public static T BestBy<T, U>(this IEnumerable<T> items, T seedItem, U seedValue, Func<U, U, bool> isLhsBetterThanRhs, Func<T, U> select)
	{
		foreach (var item in items)
		{
			var value = select(item);
			if (isLhsBetterThanRhs(value, seedValue))
			{
				seedValue = value;
				seedItem = item;
			}
		}
		return seedItem;
	}
	public static T BestBy<T, U>(this IEnumerable<T> items, Func<U, U, bool> isLhsBetterThanRhs, Func<T, U> select)
	{
		if (!items.Any())
			throw new ArgumentException("empty", nameof(items));
		var seedItem = items.First();
		var seedValue = select(seedItem);
		return BestBy(items.Skip(1), seedItem, seedValue, isLhsBetterThanRhs, select);
	}



	public static T MinBy<T>(this IEnumerable<T> items, T seedItem, int seedValue, Func<T, int> select)
	{
		foreach (var item in items)
		{
			var value = select(item);
			if (value < seedValue)
			{
				seedValue = value;
				seedItem = item;
			}
		}
		return seedItem;
	}
	public static T MinBy<T>(this IEnumerable<T> items, Func<T, int> select)
	{
		if (!items.Any())
			throw new ArgumentException("empty", nameof(items));
		var seedItem = items.First();
		var seedValue = select(seedItem);
		return MinBy(items.Skip(1), seedItem, seedValue, select);
	}

	public static T MinBy<T>(this IEnumerable<T> items, T seedItem, float seedValue, Func<T, float> select)
	{
		foreach (var item in items)
		{
			var value = select(item);
			if (value < seedValue)
			{
				seedValue = value;
				seedItem = item;
			}
		}
		return seedItem;
	}
	public static T MinBy<T>(this IEnumerable<T> items, Func<T, float> select)
	{
		if (!items.Any())
			throw new ArgumentException("empty", nameof(items));
		var seedItem = items.First();
		var seedValue = select(seedItem);
		return MinBy(items.Skip(1), seedItem, seedValue, select);
	}



	public static T MaxBy<T>(this IEnumerable<T> items, T seedItem, int seedValue, Func<T, int> select)
	{
		foreach (var item in items)
		{
			var value = select(item);
			if (value > seedValue)
			{
				seedValue = value;
				seedItem = item;
			}
		}
		return seedItem;
	}
	public static T MaxBy<T>(this IEnumerable<T> items, Func<T, int> select)
	{
		if (!items.Any())
			throw new ArgumentException("empty", nameof(items));
		var seedItem = items.First();
		var seedValue = select(seedItem);
		return MaxBy(items.Skip(1), seedItem, seedValue, select);
	}

	public static T MaxBy<T>(this IEnumerable<T> items, T seedItem, float seedValue, Func<T, float> select)
	{
		foreach (var item in items)
		{
			var value = select(item);
			if (value > seedValue)
			{
				seedValue = value;
				seedItem = item;
			}
		}
		return seedItem;
	}
	public static T MaxBy<T>(this IEnumerable<T> items, Func<T, float> select)
	{
		if (!items.Any())
			throw new ArgumentException("empty", nameof(items));
		var seedItem = items.First();
		var seedValue = select(seedItem);
		return MaxBy(items.Skip(1), seedItem, seedValue, select);
	}




	public static T? FirstOrNull<T>(this IEnumerable<T> items) where T : struct
	{
		var it = items.GetEnumerator();
		if (it.MoveNext())
			return it.Current;
		else
			return null;
	}

	public static T? FirstOrNull<T>(this IEnumerable<T> items, Predicate<T> predicate) where T : struct
	{
		foreach (var item in items)
		{
			if (predicate(item))
				return item;
		}
		return null;
	}

	public static C? AnyAsNullable<C>(this C items) where C : struct, IEnumerable
		=> items.GetEnumerator().MoveNext() ? (C?)items : null;

	public static T? FirstToNullableStruct<T>(this IEnumerable<T> items) where T : struct
	{
		var it = items.GetEnumerator();
		if (it.MoveNext())
		{
			return it.Current;
		}
		else
		{
			return null;
		}
	}

	public static T FirstToNullableClass<T>(this IEnumerable<T> items) where T : class
	{
		var it = items.GetEnumerator();
		if (it.MoveNext())
		{
			return it.Current;
		}
		else
		{
			return null;
		}
	}

	public static T? FirstToNullable<T>(this IEnumerable<T> items) where T : struct
		=> items.FirstToNullableStruct();

	public static TValue? TryGetValueNullable<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) where TValue : struct
	{
		if (dictionary.TryGetValue(key, out var value))
		{
			return value;
		}
		else
		{
			return null;
		}
	}
}

public static class CollectionClassUtils
{
	public static C AnyAsNullable<C>(this C items) where C : class, IEnumerable
		=> items.GetEnumerator().MoveNext() ? items : null;

	public static T FirstToNullable<T>(this IEnumerable<T> items) where T : class
		=> items.FirstToNullableClass();

	public static TValue TryGetValueNullable<TKey, TValue>(this IReadOnlyDictionary<TKey, TValue> dictionary, TKey key) where TValue : class
	{
		if (dictionary.TryGetValue(key, out var value))
		{
			return value;
		}
		else
		{
			return null;
		}
	}
}

public interface IReadOnlyNullableContainer<out T> : IReadOnlyList<T>
{
	bool HasValue { get; }
	T Value { get; }
}

public class NullableContainer<T> : IReadOnlyNullableContainer<T>
{
	public NullableContainer() { }
	public NullableContainer(T value) { m_nullable = value; }

	private object m_nullable;

	public bool HasValue => m_nullable != null;
	public T Value => m_nullable == null ? throw new InvalidOperationException() : (T)m_nullable;
	public T this[int index] => m_nullable == null || index != 0 ? throw new IndexOutOfRangeException() : (T)m_nullable;
	public int Count => m_nullable == null ? 0 : 1;

	public IEnumerator<T> GetEnumerator()
	{
		if (m_nullable != null)
			yield return (T)m_nullable;
	}

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public bool TryGetValue(out T value)
	{
		if (HasValue)
		{
			value = Value;
			return true;
		}
		value = default;
		return false;
	}

	public bool TryPush(T value)
	{
		if (HasValue)
			return false;

		m_nullable = value;
		return true;
	}

	public bool TryPop(out T value)
	{
		if (HasValue)
		{
			value = (T)m_nullable;
			m_nullable = null;
			return true;
		}
		value = default;
		return false;
	}
}

public static class NullableContainerUtils
{
	public static bool TryGetValue<T>(this IReadOnlyNullableContainer<T> container, out T value)
	{
		if (container.HasValue)
		{
			value = container.Value;
			return true;
		}
		value = default;
		return false;
	}



	public static NullableContainer<T> FromStruct<T>(T value) where T : struct
		=> new NullableContainer<T>(value);

	public static NullableContainer<T> FromNullableStruct<T>(T? nullable) where T : struct
		=> nullable.HasValue ? new NullableContainer<T>(nullable.Value) : new NullableContainer<T>();

	public static NullableContainer<T> FromNullableClass<T>(T nullable) where T : class
		=> nullable == null ? new NullableContainer<T>(nullable) : new NullableContainer<T>();

	public static NullableContainer<T> FromNullableUnityObject<T>(T nullable) where T : UnityEngine.Object
		=> nullable ? new NullableContainer<T>(nullable) : new NullableContainer<T>();



	public static T? ToNullableStruct<T>(this IReadOnlyNullableContainer<T> container) where T : struct
		=> container.HasValue ? (T?)container.Value : null;

	public static T ToNullableClass<T>(this IReadOnlyNullableContainer<T> container) where T : class
		=> container.HasValue ? container.Value : null;

	public static T? ToNullable<T>(this IReadOnlyNullableContainer<T> container) where T : struct
		=> container.ToNullableStruct();


	public static NullableContainer<T> ToNullableContainer<T>(this T? nullable) where T : struct
		=> FromNullableStruct(nullable);
}

public static class NullableContainerClassUtils
{
	public static T ToNullable<T>(this IReadOnlyNullableContainer<T> container) where T : class
		=> container.ToNullableClass();

	public static NullableContainer<T> ToNullableContainer<T>(this T nullable) where T : class
		=> NullableContainerUtils.FromNullableClass(nullable);

	public static NullableContainer<T> ToNullableContainerFromUnityObject<T>(this T nullable) where T : UnityEngine.Object
		=> NullableContainerUtils.FromNullableUnityObject(nullable);
}

public static class NullableUtils
{
	public static T AsTrueNullable<T>(this T unityObject) where T : UnityEngine.Object
		=> unityObject ? unityObject : null;

	public static R NullableSelect<T, R>(this T nullable, Func<T, R> select)
		=> nullable is T value ? select(value) : default;

	public static T NullableWhere<T>(this T nullable, Predicate<T> predicate)
		=> nullable is T value && predicate(value) ? value : default;

#if false
	public static R? NullableSelectNested<T, R>(this T? nullable, Func<T, R?> select)
		where T : struct
		where R : struct
	{
		return nullable.HasValue ? select(nullable.Value) : null;
	}

	public static R? NullableSelectStruct<T, R>(this T? nullable, Func<T, R> select)
		where T : struct
		where R : struct
	{
		return nullable.HasValue ? (R?)select(nullable.Value) : null;
	}

	public static R NullableSelectClass<T, R>(this T? nullable, Func<T, R> select)
		where T : struct
		where R : class
	{
		return nullable.HasValue ? select(nullable.Value) : null;
	}



	public static R? NullableSelectNested<T, R>(this T nullable, Func<T, R?> select)
		where T : class
		where R : struct
	{
		return null == nullable ? null : select(nullable);
	}

	public static R? NullableSelectStruct<T, R>(this T nullable, Func<T, R> select)
		where T : class
		where R : struct
	{
		return null == nullable ? null : (R?)select(nullable);
	}

	public static R NullableSelectClass<T, R>(this T nullable, Func<T, R> select)
		where T : class
		where R : class
	{
		return null == nullable ? null : select(nullable);
	}


	
	public static T? NullableWhere<T>(this T? nullable, Predicate<T> predicate) where T : struct
		=> nullable.HasValue && predicate(nullable.Value) ? nullable : null;

	public static T NullableWhere<T>(this T nullable, Predicate<T> predicate) where T : class
		=> nullable != null && predicate(nullable) ? nullable : null;
#endif

	public static T? WhereToNullable<T>(this T value, Predicate<T> predicate) where T : struct
		=> predicate(value) ? (T?)value : null;
}

public static class NullableClassUtils
{
	public static T WhereToNullable<T>(this T value, Predicate<T> predicate) where T : class
		=> predicate(value) ? value : null;
}

public static class FuncUtils
{
	// like Enumerable.Select but for one element
	public static R Pipe<T, R>(this T value, Func<T, R> function) => function(value);
	public static void Pipe<T>(this T value, Action<T> function) => function(value);
	public static T IdentityWithSideEffect<T>(this T value, Action<T> function) { function(value); return value; }
	public static T ForwardWithSideEffect<T>(this T value, RefArgAction<T> function) { function(ref value); return value; }
	public delegate void RefArgAction<T>(ref T value);

	public static R Make<R>(Func<R> function) => function();

	public static Func<R> FuncIdentity<R>(Func<R> function) => function;
	public static Func<T, R> FuncIdentity<T, R>(Func<T, R> function) => function;
	public static Action FuncIdentity(Action function) => function;
	public static Action<T> FuncIdentity<T>(Action<T> function) => function;



	public static Func<S, V, S> AggregateAddInPlace<S, V>(Action<S, V> addInPlace)
		=> (s, v) => { addInPlace(s, v); return s; };


	public delegate bool TryGetFunc<TOut>(out TOut value);
	public static TOut? NullableStructFromTryGet<TOut>(TryGetFunc<TOut> tryGetFunc) where TOut : struct
	{
		if (tryGetFunc(out var value))
		{
			return value;
		}
		else
		{
			return null;
		}
	}
	public static TOut NullableClassFromTryGet<TOut>(TryGetFunc<TOut> tryGetFunc) where TOut : class
	{
		if (tryGetFunc(out var value))
		{
			return value;
		}
		else
		{
			return null;
		}
	}
	public static TOut GetFromTryGet<TOut>(TryGetFunc<TOut> tryGetFunc)
	{
		if (tryGetFunc(out var value))
			return value;
		else
			throw new NullReferenceException("TryGet returned false");
	}

	public delegate bool TryGetFunc<T1, TOut>(T1 obj1, out TOut value);
	public static TOut? NullableStructFromTryGet<T1, TOut>(T1 obj1, TryGetFunc<T1, TOut> tryGetFunc) where TOut : struct
	{
		if (tryGetFunc(obj1, out var value))
		{
			return value;
		}
		else
		{
			return null;
		}
	}
	public static TOut NullableClassFromTryGet<T1, TOut>(T1 obj1, TryGetFunc<T1, TOut> tryGetFunc) where TOut : class
	{
		if (tryGetFunc(obj1, out var value))
		{
			return value;
		}
		else
		{
			return null;
		}
	}
	public static TOut GetFromTryGet<T1, TOut>(T1 obj1, TryGetFunc<T1, TOut> tryGetFunc)
	{
		if (tryGetFunc(obj1, out var value))
			return value;
		else
			throw new NullReferenceException("TryGet returned false");
	}
}

public static class VectorUtils
{
	public static Vector2 InverseScale(this Vector2 numerator, Vector2 denominator)
	{
		numerator.x /= denominator.x;
		numerator.y /= denominator.y;
		return numerator;
	}

	public static Vector2Int InverseScale(this Vector2Int numerator, Vector2Int denominator)
	{
		numerator.x /= denominator.x;
		numerator.y /= denominator.y;
		return numerator;
	}

	public static Vector3 WithZ(this Vector2 vector, float z) => new Vector3(vector.x, vector.y, z);
	public static Vector3Int WithZ(this Vector2Int vector, int z) => new Vector3Int(vector.x, vector.y, z);
}

public static class ColorUtils
{
	public static Color WithAlpha(this Color color, float alpha)
	{
		return new Color(color.r, color.g, color.b, alpha);
	}
}

public static class MathUtils
{
	public static int Cycle(int value, int length)
	{
		value %= length;
		return value < 0 ? length + value : value;
	}
	public static float Cycle(float value, float length)
	{
		value %= length;
		return value < 0 ? length + value : value;
	}

	public static float MapRange(float inValue, float inMin, float inMax, float outMin, float outMax)
	{
		// case v=0, 0,1, 0,1 => 0
		// case v=1, 0,1, 0,1 => 1
		// case v=0, -1,1, 0,1 => 0.5

		float inRange = inMax - inMin;
		float valuef = (inValue - inMin) / inRange; // NOTE: divide by zero => floating point NaN
		float outRange = outMax - outMin;
		float outValue = outMin + valuef * outRange;
		return outValue;
	}

	public static float MapRange_f_from_fm(float inValue) => MapRange(inValue, -1.0f, 1.0f, 0.0f, 1.0f);
	public static float MapRange_fm_from_f(float inValue) => MapRange(inValue, 0.0f, 1.0f, -1.0f, 1.0f);
}

public struct CountRange : IReadOnlyList<int>
{
	public int Count { get; private set; }
	public int this[int index] => index;

	private CountRange(int count) { Count = count; }
	public static CountRange From(int count) => new CountRange(count);

	public IEnumerator<int> GetEnumerator() => new Enumerator(this);
	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

	public class Enumerator : IEnumerator<int>
	{
		public CountRange Range { get; private set; }
		public int Current { get; private set; } = -1;
		object IEnumerator.Current => Current;

		public Enumerator(CountRange range) { this.Range = range; }

		public bool MoveNext()
		{
			++Current;
			return Current < Range.Count;
		}

		public void Reset()
		{
			Current = -1;
		}

		public void Dispose() { }
	}

	public T[] SelectToArray<T>(Func<int, T> select)
	{
		var array = new T[Count];
		for (int i = 0; i < Count; ++i)
		{
			array[i] = select(i);
		}
		return array;
	}
}

public static class EnumUtils
{
	public static T CyclicAdd<T>(T value, T length, int add) where T : Enum
	{
		return (T)(object)MathUtils.Cycle((int)(object)value + add, (int)(object)length);
	}

	public static T CyclicAdd<T>(T value, int add) where T : Enum
	{
		var t = typeof(T);
		var i = t.GetEnumNames().IndexOf("ENUM_LENGTH");
		if (i < 0)
			throw new InvalidOperationException($"Enum {nameof(T)} has no member named ENUM_LENGTH");
		var length = (T)t.GetEnumValues().GetValue(i);
		return CyclicAdd(value, length, add);
	}

	public static T CyclicIncrement<T>(T value) where T : Enum
	{
		return CyclicAdd(value, 1);
	}
}

public struct Voidlike { }