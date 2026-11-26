using Common.Packets;

namespace Common.Coding;

/// <summary>
/// An object that can be coded, i.e. read to and from a byte array 
/// </summary>
public abstract class Codable
{
    /// <summary>
    /// A list of fields on this object that can be coded
    /// </summary>
    internal readonly List<CodableValue> CodableValues = [];

    public Codable()
    {
        RegisterValues();
    }
    
    /// <summary>
    /// Registers the fields that can be coded
    /// </summary>
    protected virtual void RegisterValues() { }
    
    /// <summary>
    /// Registers a fixed size field to be codable
    /// </summary>
    /// <param name="name">The name of the field</param>
    /// <param name="getter">The field's getter</param>
    /// <param name="setter">The field's setter, used for decoding</param>
    /// <param name="sizeBytes">The size of the field</param>
    /// <typeparam name="T">The type of the field</typeparam>
    protected void RegisterFixedSizeValue<T>(string name, Func<T> getter, Action<T> setter, int sizeBytes)
    {
        CodableValues.Add(new CodableValue<T>(name, getter, setter, sizeBytes));
    }
    
    /// <summary>
    /// Registers a dynamically size field to be codable
    /// </summary>
    /// <param name="name">The name of the field</param>
    /// <param name="getter">The field's getter</param>
    /// <param name="setter">The field's setter, used for decoding</param>
    /// <typeparam name="T">The type of the field</typeparam>
    protected void RegisterDynamicSizeValue<T>(string name, Func<T> getter, Action<T> setter)
    {
        CodableValues.Add(new CodableValue<T>(name, getter, setter, null));
    }
}