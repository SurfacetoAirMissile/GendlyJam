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
	public static class CgdUtils
	{
		public static T CacheFetchProperty<T>(
			ref T field,
			Func<T> fetch,
			Predicate<T> exists,
			Action logErrorMessage,
			Action<T> extraInitialisation = null
			)
		{
			if (!exists(field))
			{
				field = fetch();
				if (!exists(field))
				{
					logErrorMessage();
				}
				else
				{
					extraInitialisation?.Invoke(field);
				}
			}
			return field;
		}

		public static T CacheFetchProperty<T, U>(
			this U self,
			ref T field,
			Func<T> fetch,
			Predicate<T> exists,
			Action<U> logErrorMessage,
			Action<T> extraInitialisation = null
			)
		{ 
			return CacheFetchProperty(
				ref field,
				fetch,
				exists,
				() => logErrorMessage(self),
				extraInitialisation
				);
		}

		public static T CacheFetchComponent<T>(
			this Component script,
			ref T field,
			Func<T> fetch,
			Func<string> fetchErrorMessage = null,
			Action<T> extraInitialisation = null
			) where T : Component
		{
			if (!script)
			{
				throw new NullReferenceException("script is null");
			}
			return CacheFetchProperty(
				script,
				ref field,
				fetch,
				c => c,
				self => Debug.LogError(fetchErrorMessage?.Invoke() ?? (typeof(T).Name + " Component Not Found"), self),
				extraInitialisation
				);
		}



		public static T CacheGetComponent<T>(
			this Component script,
			ref T field,
			Action<T> extraInitialisation = null
			) where T : Component
		{
			return CacheFetchComponent(
				script,
				ref field,
				() => script.GetComponent<T>(),
				() => typeof(T).Name + " Component Not Found",
				extraInitialisation
				);
		}

		public static T CacheGetComponentInParent<T>(
			this Component script,
			ref T field,
			Action<T> extraInitialisation = null
			) where T : Component
		{
			return CacheFetchComponent(
				script,
				ref field,
				() => script.GetComponentInParent<T>(),
				() => typeof(T).Name + " Component Not Found in Parent Hierarchy",
				extraInitialisation
				);
		}



		public static IEnumerable<(int index, T element)> IndexedElements<T>(this IEnumerable<T> collection)
		{
			if (collection is IReadOnlyList<T> list)
			{
				return CollectionUtils.IndexedElements(list);
			}
			else
			{
				return IndexedElementsImpl();
			}

			IEnumerable<(int index, T element)> IndexedElementsImpl()
			{
				int i = 0;
				foreach (var item in collection)
				{
					yield return (index: i, element: item);
					++i;
				}
			}
		}



		// Original in JavaScript: https://github.com/Daplie/knuth-shuffle
		// Modified for Unity C# by Elijah Shadbolt
		// 09/02/2021
		public static T ShuffleInPlace<T>(this T list) where T : IList
		{
			var currentIndex = list.Count;

			// While there remain elements to shuffle...
			while (0 != currentIndex)
			{
				// Pick a remaining element...
				var randomIndex = UnityEngine.Random.Range(0, currentIndex);
				currentIndex -= 1;

				// And swap it with the current element.
				var temporaryValue = list[currentIndex];
				list[currentIndex] = list[randomIndex];
				list[randomIndex] = temporaryValue;
			}

			return list;
		}



		public static TValue GetOrCreate<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> create)
		{
			if (dict.TryGetValue(key, out var value))
			{
				return value;
			}
			else
			{
				value = create();
				dict.Add(key, value);
				return value;
			}
		}



		public static void QuitUnityApp()
		{
			Application.Quit();
#if UNITY_EDITOR
			UnityEditor.EditorApplication.isPlaying = false;
#endif
		}



		public static void Resize<T>(this IList<T> list, int count, Func<int, T> createItem)
		{
			var oldCount = list.Count;
			if (oldCount < count)
			{
				if (createItem == null) {
					throw new ArgumentNullException(nameof(createItem));
				}

				while (oldCount < count)
				{
					++oldCount;
					list.Add(createItem(oldCount));
				}
			}
			else if (oldCount > count)
			{
				if (list is List<T> concreteList)
				{
					concreteList.RemoveRange(count, oldCount - count);
				}
				else
				{
					while (oldCount > count)
					{
						--oldCount;
						list.RemoveAt(oldCount);
					}
				}
			}
		}



		public static T PickRandomItem<T>(this IReadOnlyList<T> list)
		{
			if (list.Count <= 0)
				throw new ArgumentException("list cannot be empty", nameof(list));

			return list[UnityEngine.Random.Range(0, list.Count)];
		}

		public static T PickRandomItem<T>(this IReadOnlyCollection<T> collection)
		{
			if (collection.Count <= 0)
				throw new ArgumentException("collection cannot be empty", nameof(collection));

			return collection.ElementAt(UnityEngine.Random.Range(0, collection.Count));
		}

		public static T RemoveRandomItem<T>(this IList<T> list)
		{
			if (list.Count <= 0)
				throw new ArgumentException("list cannot be empty", nameof(list));

			var index = UnityEngine.Random.Range(0, list.Count);
			var item = list[index];
			list.RemoveAt(index);
			return item;
		}



		public static void TryElseLog(this UnityEngine.Object context, Action action)
		{
			try
			{
				action();
			}
			catch (Exception e)
			{
				Debug.LogException(e, context);
			}
		}

		public static void TryElseLog(Action action)
		{
			try
			{
				action();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
			}
		}

		public static R TryElseLogThenMakeDefault<R>(this UnityEngine.Object context, Func<R> throwingFunc, Func<R> nothrowFunc)
		{
			try
			{
				return throwingFunc();
			}
			catch (Exception e)
			{
				Debug.LogException(e, context);
				return nothrowFunc();
			}
		}

		public static R TryElseLogThenMakeDefault<R>(Func<R> throwingFunc, Func<R> nothrowFunc)
		{
			try
			{
				return throwingFunc();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				return nothrowFunc();
			}
		}

		public static R TryElseLogThenDefault<R>(this UnityEngine.Object context, Func<R> throwingFunc, R defaultValue)
		{
			try
			{
				return throwingFunc();
			}
			catch (Exception e)
			{
				Debug.LogException(e, context);
				return defaultValue;
			}
		}

		public static R TryElseLogThenDefault<R>(this UnityEngine.Object context, Func<R> throwingFunc)
		{
			try
			{
				return throwingFunc();
			}
			catch (Exception e)
			{
				Debug.LogException(e, context);
				return default(R);
			}
		}

		public static R TryElseLogThenDefault<R>(Func<R> throwingFunc, R defaultValue)
		{
			try
			{
				return throwingFunc();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				return defaultValue;
			}
		}

		public static R TryElseLogThenDefault<R>(Func<R> throwingFunc)
		{
			try
			{
				return throwingFunc();
			}
			catch (Exception e)
			{
				Debug.LogException(e);
				return default(R);
			}
		}

		public static void TryInitCatchDestroyThrow(
			UnityEngine.Object newlyCreatedObject,
			Action init
			)
		{
			try
			{
				init();
			}
			catch
			{
				UnityEngine.Object.Destroy(newlyCreatedObject);
				throw;
			}
		}



		public static Vector2 Vec2NaN => new Vector2(float.NaN, float.NaN);



		public static T ThrowIfNull<T>(this T obj) where T : class
		{
			if (obj is null) throw new NullReferenceException();
			return obj;
		}
		public static T ThrowIfNull<T>(this T obj, Func<string> fetchErrorMessage) where T : class
		{
			if (obj is null) throw new NullReferenceException(fetchErrorMessage());
			return obj;
		}

		public static T LogErrorIfNull<T>(this T obj) where T : class
		{
			if (obj is null) Debug.LogError("object is null");
			return obj;
		}
		public static T LogErrorIfNull<T>(this T obj, Func<string> fetchErrorMessage) where T : class
		{
			if (obj is null) Debug.LogError(fetchErrorMessage());
			return obj;
		}
		public static T LogErrorIfNull<T>(this T obj, Func<string> fetchErrorMessage, UnityEngine.Object context) where T : class
		{
			if (obj is null) Debug.LogError(fetchErrorMessage(), context);
			return obj;
		}
	}



	/// <changelog>
	///		<log author="Elijah Shadbolt" date="11/02/2021">
	///			Created this interface.
	///		</log>
	/// </changelog>
	/// 
	public interface IReadOnlyArray2DLinearListWrapper<out T> : IReadOnlyList<T>
	{
		T[,] array { get; }

		int numColumns { get; }
		int numRows { get; }
		Coord size { get; }

		T this[int column, int row] { get; }
		T this[Coord coord] { get; }

		IEnumerable<Coord> EnumerateCoords();
	}

	public static class Array2DLinearListExtensions
	{
		public static bool TryGetValue<T>(this IReadOnlyArray2DLinearListWrapper<T> grid, Coord coord, out T value)
		{
			if (coord.column < 0 || coord.column >= grid.numColumns
				|| coord.row < 0 || coord.row >= grid.numRows)
			{
				value = default;
				return false;
			}

			value = grid[coord];
			return true;
		}

		public static T TryGetValueNullableClass<T>(
			this IReadOnlyArray2DLinearListWrapper<T> grid,
			Coord coord
			)
			where T : class
		{
			if (grid.TryGetValue(coord, out var value))
				return value;
			return null;
		}

		public static T? TryGetValueNullableStruct<T>(
			this IReadOnlyArray2DLinearListWrapper<T> grid,
			Coord coord
			)
			where T : struct
		{
			if (grid.TryGetValue(coord, out var value))
				return value;
			return null;
		}

		public static Array2DLinearListWrapper<R> SelectToArray2D<T, R>(
			this IReadOnlyArray2DLinearListWrapper<T> grid,
			Func<T, Coord, R> select
			)
		{
			var outGrid = new Array2DLinearListWrapper<R>(grid.size);
			foreach (var coord in grid.EnumerateCoords())
			{
				outGrid[coord] = select(grid[coord], coord);
			}
			return outGrid;
		}

		public static Array2DLinearListWrapper<R> SelectToArray2D<T, R>(
			this IReadOnlyArray2DLinearListWrapper<T> grid,
			Func<T, R> select
			)
		{
			return SelectToArray2D(grid, (cell, _) => select(cell));
		}
	}

	/// <changelog>
	///		<log author="Elijah Shadbolt" date="11/02/2021">
	///			Created this class.
	///		</log>
	/// </changelog>
	/// 
	public class Array2DLinearListWrapper<T> : IReadOnlyArray2DLinearListWrapper<T>
	{
		public Array2DLinearListWrapper(T[,] array)
		{
			if (array == null) throw new ArgumentNullException(nameof(array));
			m_array = array;
		}

		public Array2DLinearListWrapper(int numColumns, int numRows)
			: this(new T[numColumns, numRows])
		{
		}

		public Array2DLinearListWrapper(Coord size)
			: this(numColumns: size.column, numRows: size.row)
		{
		}

		private T[,] m_array;
		public T[,] array
		{
			get => m_array;
			set
			{
				if (value == null) throw new ArgumentNullException(nameof(value));
				m_array = value;
			}
		}

		public int numColumns => array.GetLength(0);
		public int numRows => array.GetLength(1);
		public Coord size => new Coord(column: numColumns, row: numRows);

		public T this[int column, int row]
		{
			get => array[column, row];
			set => array[column, row] = value;
		}
		public T this[Coord coord]
		{
			get => this[coord.column, coord.row];
			set => this[coord.column, coord.row] = value;
		}
		public T this[int index]
		{
			get => this[index % numColumns, index / numColumns];
			set => this[index % numColumns, index / numColumns] = value;
		}

		public int Count => array.Length;

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public IEnumerator<T> GetEnumerator()
		{
			for (int row = 0; row < numRows; row++)
			{
				for (int col = 0; col < numColumns; col++)
				{
					yield return array[col, row];
				}
			}
		}

		public IEnumerable<Coord> EnumerateCoords()
		{
			for (int row = 0; row < numRows; row++)
			{
				for (int col = 0; col < numColumns; col++)
				{
					yield return new Coord(column: col, row: row);
				}
			}
		}
	}



	public class ExceptionWithChildExceptions : Exception
	{
		public /*not-null*/ IReadOnlyList<Exception> ChildExceptions { get; private set; }



		public ExceptionWithChildExceptions(string message, IEnumerable<Exception> childExceptions, Exception innerException)
			: base(message, innerException)
		{
			ChildExceptions = Process(childExceptions);
		}

		public ExceptionWithChildExceptions(string message, IEnumerable<Exception> childExceptions)
			: base(message)
		{
			ChildExceptions = Process(childExceptions);
		}

		public ExceptionWithChildExceptions(string message, Exception innerException)
			: this(message, Array.Empty<Exception>(), innerException) { }

		public ExceptionWithChildExceptions(string message)
			: this(message, Array.Empty<Exception>()) { }

		public ExceptionWithChildExceptions(IEnumerable<Exception> childExceptions, Exception innerException)
			: this(nameof(ExceptionWithChildExceptions), childExceptions, innerException) { }

		public ExceptionWithChildExceptions(IEnumerable<Exception> childExceptions)
			: this(nameof(ExceptionWithChildExceptions), childExceptions) { }

		public ExceptionWithChildExceptions()
			: this(Array.Empty<Exception>()) { }



		private static IReadOnlyList<Exception> Process(IEnumerable<Exception> childExceptions)
		{
			return childExceptions == null
				? Array.Empty<Exception>()
				: childExceptions.Where(e => e != null).ToArray();
		}
	}



}
