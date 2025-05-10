using System;

public class ObservableArray<T>
{
    private T[] _array;
    public event Action<int, T> OnValueChanged; // (인덱스, 값)
    public bool EventOff = false;

    public int Length => _array.Length;

    public ObservableArray(int size, T value)
    {
        _array = new T[size];

        for (var i = 0; i < size; i++)
        {
            _array[i] = value;
        }
    }

    public T this[int index]
    {
        get => _array[index];
        set
        {
            if (!Equals(_array[index], value))
            {
                _array[index] = value;

                if (!EventOff)
                    OnValueChanged?.Invoke(index, value);
            }
        }
    }

    public T[] ToArray()
    {
        return (T[])_array.Clone();
    }
}