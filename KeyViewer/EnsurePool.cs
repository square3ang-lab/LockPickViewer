using System;
using System.Collections.Generic;

namespace KeyViewer;

public class EnsurePool<T>
{
	private List<T> pool;

	private Func<T> ensurer;

	private Predicate<T> criteria;

	private Action<T> onGet;

	private Action<T> onRemove;

	public int Count => pool.Count;

	public EnsurePool(Func<T> ensurer, Predicate<T> ensureCriteria, Action<T> onGet = null, Action<T> onRemove = null, int capacity = -1)
	{
		if (ensurer == null)
		{
			throw new ArgumentNullException("ensurer", "Ensurer Cannot Be Null!");
		}
		if (ensureCriteria == null)
		{
			throw new ArgumentNullException("ensureCriteria", "Ensure Criteria Cannot Be Null!");
		}
		pool = new List<T>();
		this.ensurer = ensurer;
		criteria = ensureCriteria;
		this.onGet = onGet;
		this.onRemove = onRemove;
		if (capacity > 0)
		{
			Fill(capacity);
		}
	}

	public T Get()
	{
		foreach (T item in pool)
		{
			if (criteria(item))
			{
				onGet?.Invoke(item);
				return item;
			}
		}
		T val = ensurer();
		pool.Add(val);
		onGet?.Invoke(val);
		return val;
	}

	public void Clear()
	{
		ForEach(onRemove);
		pool.Clear();
	}

	public void Remove(T t)
	{
		onRemove(t);
		pool.Remove(t);
	}

	public void RemoveAt(int index)
	{
		if (index < Count)
		{
			onRemove(pool[index]);
			pool.RemoveAt(index);
		}
	}

	public void Fill(int count)
	{
		for (int i = 0; i < count; i++)
		{
			pool.Add(ensurer());
		}
	}

	public void ForEach(Action<T> forEach)
	{
		if (forEach == null)
		{
			return;
		}
		foreach (T item in pool)
		{
			forEach(item);
		}
	}
}
