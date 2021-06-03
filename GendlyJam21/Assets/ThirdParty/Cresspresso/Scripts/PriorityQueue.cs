///
/// This script was taken from a separate project Elijah Shadbolt worked on.
/// It was copied over 03/06/2021.
///

using System.Collections;
using System.Collections.Generic;
using System.Linq;

public interface IPriorityQueue<TKey, TValue> : IReadOnlyDictionary<TKey, IReadOnlyCollection<TValue>>
{
	void Add(TKey key, TValue value);

	bool TryPeek(out TValue value);
	bool TryPeek(out TKey key, out TValue value);

	bool TryDequeue(out TValue value);
	bool TryDequeue(out TKey key, out TValue value);
}

public class PriorityQueue<TKey, TValue> : IPriorityQueue<TKey, TValue>
{
	public void Add(TKey key, TValue value)
	{
		if (m_queuesByPriority.TryGetValue(key, out var them))
		{
			them.Enqueue(value);
		}
		else
		{
			them = new Queue<TValue>();
			them.Enqueue(value);
			m_queuesByPriority.Add(key, them);
		}
		++m_count;
	}

	public bool TryPeek(out TValue value) => TryPeek(out _, out value);
	public bool TryPeek(out TKey key, out TValue value)
	{
		var it = m_queuesByPriority.GetEnumerator();
		if (it.MoveNext())
		{
			var them = it.Current.Value;
			if (them.Count > 0)
			{
				value = them.Peek();
				key = it.Current.Key;
				return true;
			}
		}

		value = default;
		key = default;
		return false;
	}

	public bool TryDequeue(out TValue value) => TryDequeue(out _, out value);
	public bool TryDequeue(out TKey key, out TValue value)
	{
		var it = m_queuesByPriority.GetEnumerator();
		if (it.MoveNext())
		{
			var them = it.Current.Value;
			var c = them.Count;
			if (c > 0)
			{
				value = them.Dequeue();
				key = it.Current.Key;
				--m_count;
				if (c == 1) // if them is empty after we dequeued
				{
					m_queuesByPriority.Remove(it.Current.Key);
				}
				return true;
			}
		}

		value = default;
		key = default;
		return false;
	}

	public PriorityQueue(IComparer<TKey> comparer)
	{
		m_queuesByPriority = new SortedDictionary<TKey, Queue<TValue>>(comparer);
	}

	public PriorityQueue()
		: this(Comparer<TKey>.Default)
	{ }

	public PriorityQueue(IEnumerable<KeyValuePair<TKey, TValue>> items, IComparer<TKey> comparer)
		: this(comparer)
	{
		foreach (var kvp in items)
		{
			Add(kvp.Key, kvp.Value);
		}
	}

	public PriorityQueue(IEnumerable<KeyValuePair<TKey, TValue>> items)
		: this(items, Comparer<TKey>.Default)
	{ }



	public IComparer<TKey> Comparer => m_queuesByPriority.Comparer;

	public int Count => m_count;

	private SortedDictionary<TKey, Queue<TValue>> m_queuesByPriority;
	private int m_count = 0;



	public bool ContainsKey(TKey key) => m_queuesByPriority.ContainsKey(key);

	bool
		IReadOnlyDictionary<TKey, IReadOnlyCollection<TValue>>
		.TryGetValue(TKey key, out IReadOnlyCollection<TValue> value)
	{
		if (m_queuesByPriority.TryGetValue(key, out var m))
		{
			value = m;
			return true;
		}
		else
		{
			value = default;
			return false;
		}
	}

	IReadOnlyCollection<TValue>
		IReadOnlyDictionary<TKey, IReadOnlyCollection<TValue>>
		.this[TKey key] => m_queuesByPriority[key];

	IEnumerable<TKey>
		IReadOnlyDictionary<TKey, IReadOnlyCollection<TValue>>
		.Keys => m_queuesByPriority.Keys;

	IEnumerable<IReadOnlyCollection<TValue>>
		IReadOnlyDictionary<TKey, IReadOnlyCollection<TValue>>
		.Values => m_queuesByPriority.Values.Select(v => (IReadOnlyCollection<TValue>)v);

	IEnumerator<KeyValuePair<TKey, IReadOnlyCollection<TValue>>>
		IEnumerable<KeyValuePair<TKey, IReadOnlyCollection<TValue>>>
		.GetEnumerator()
	{
		var it = m_queuesByPriority.GetEnumerator();
		while (it.MoveNext())
		{
			var kvp = it.Current;
			yield return new KeyValuePair<TKey, IReadOnlyCollection<TValue>>(kvp.Key, kvp.Value);
		}
	}

	IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable<KeyValuePair<TKey, IReadOnlyCollection<TValue>>>)this).GetEnumerator();
}