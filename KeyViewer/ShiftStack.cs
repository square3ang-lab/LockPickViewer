using System;

namespace KeyViewer;

public class ShiftStack<T>
{
	private T[] array;

	private int capacity;

	private bool dynamicCapacity;

	private Func<int> capacityProvider;

	private T defaultValue;

	public int Count { get; private set; }

	public int Capacity
	{
		get
		{
			return capacity;
		}
		set
		{
			if (value <= 1)
			{
				throw new ArgumentException("Capacity Must Be Greater Than 1!", "value");
			}
			if (capacity != value)
			{
				Array.Resize(ref array, capacity = value);
				Count = Math.Min(Count, capacity);
			}
		}
	}

	public ShiftStack(int capacity, T defaultValue)
	{
		if (capacity <= 1)
		{
			throw new ArgumentException("Capacity Must Be Greater Than 1!", "capacity");
		}
		this.capacity = capacity;
		array = new T[capacity];
		this.defaultValue = defaultValue;
	}

	public ShiftStack(Func<int> capacityProvider, T defaultValue)
	{
		if (capacityProvider == null)
		{
			throw new ArgumentNullException("CapacityProvider Must Not Be null!", "capacityProvider");
		}
		this.capacityProvider = capacityProvider;
		capacity = capacityProvider();
		array = new T[capacity];
		dynamicCapacity = true;
		this.defaultValue = defaultValue;
	}

	public void Push(T item)
	{
		if (dynamicCapacity)
		{
			Capacity = capacityProvider();
		}
		Array.Copy(array, 0, array, 1, array.Length - 1);
		array[0] = item;
		if (Count < Capacity)
		{
			int count = Count + 1;
			Count = count;
		}
	}

	public T Pop()
	{
		if (Count <= 0)
		{
			return defaultValue;
		}
		T result = array[0];
		Array.Copy(array, 1, array, 0, array.Length - 1);
		int count = Count - 1;
		Count = count;
		return result;
	}

	public T Peek()
	{
		return (Count <= 0) ? defaultValue : array[0];
	}

	public void Clear()
	{
		array = new T[capacity];
		Count = 0;
	}
}
