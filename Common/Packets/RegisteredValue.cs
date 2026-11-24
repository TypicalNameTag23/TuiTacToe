using System.Reflection;
using System.Runtime.InteropServices;

namespace Common.Packets;

public record RegisteredValue(
    string FieldName,
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

    public static RegisteredValue Create(object sourceObject, FieldInfo field, bool fixedSize = true)
    {
        return new RegisteredValue(
            field.Name,
            () => field.GetValue(sourceObject),
            value => field.SetValue(sourceObject, value),
            field.FieldType,
            fixedSize ? Marshal.SizeOf(field.FieldType) : null);
    }
}