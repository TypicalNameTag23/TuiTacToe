namespace Common.Packets;

// public record RegisteredValue<T>(Func<T> getter, Action<T> setter, bool isFixedSize)
//     : RegisteredValue(() => (T)getter, setter, typeof(T), isFixedSize);

public record RegisteredValue(
    Func<object?> Getter, 
    Action<object> Setter, 
    Type Type, 
    int? SizeBytes)
{
    public bool IsFixedSize => SizeBytes != null;
    
    public object? Value
    {
        get => Getter();
        set
        {
            if (value.GetType() != Type)
            {
                throw new InvalidOperationException($"Cannot set {value.GetType()} to {Type}");
            }
            
            Setter(value);
        }
    }
}