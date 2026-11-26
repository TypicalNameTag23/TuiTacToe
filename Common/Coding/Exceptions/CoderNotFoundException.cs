namespace Common.Coding.Exceptions;

/// <summary>
/// Thrown when a coder can not be found for a given type.
/// </summary>
/// <param name="type">The type without a coder</param>
public class CoderNotFoundException(Type type) : CodingException($"Coder not found for type {type}.");