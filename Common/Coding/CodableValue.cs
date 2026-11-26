namespace Common.Coding;

public record CodableValue
{
    public string FieldName { get; }
    public Func<object?> Getter { get; }
    public Action<object?> Setter { get; }
    public Type Type { get; }
    public int? SizeBytes { get; }

    /// <summary>
    /// A value that can be coded
    /// </summary>
    /// <param name="fieldName">The name of the field</param>
    /// <param name="getter">The getter</param>
    /// <param name="setter">The setter</param>
    /// <param name="type">The type</param>
    /// <param name="sizeBytes">The size in bytes, null if variable</param>
    public CodableValue(string fieldName, Func<object?> getter, Action<object?> setter, Type type, int? sizeBytes)
    {
        if (sizeBytes is < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(sizeBytes), "Size can not be less than 1 byte");
        }

        FieldName = fieldName;
        Getter = getter;
        Setter = setter;
        Type = type;
        SizeBytes = sizeBytes;
    }
    
    public bool IsFixedSize => SizeBytes != null;
    
    /// <summary>
    /// Used to access the backing value
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when trying to set the value to a type it is not</exception>
    public object? Value
    {
        get => Getter();
        set
        {
            if (value == null)
            {
                Setter(value);
                return;
            }

            if (value.GetType() != Type)
            {
                throw new InvalidOperationException($"Cannot set {value.GetType()} to {Type}");
            }

            Setter(value);
        }
    }
}

/// <summary>
/// A value that can be coded
/// </summary>
/// <param name="FieldName">The name of the field</param>
/// <param name="TGetter">The getter</param>
/// <param name="TSetter">The setter</param>
/// <param name="SizeBytes">The size in bytes, null if variable</param>
/// <typeparam name="T">The type</typeparam>
public record CodableValue<T>(
    string FieldName,
    Func<T> TGetter, 
    Action<T> TSetter, 
    int? SizeBytes) : CodableValue(FieldName, () => TGetter(), v => TSetter((T) v), typeof(T), SizeBytes)
{
    /// <summary>
    /// Used to access the backing value
    /// </summary>
    public new T Value
    {
        get => TGetter();
        set
        {
            if (Equals(value, default(T)))
            {
                TSetter(value);
                return;
            }

            Setter(value);
        }
    }
}