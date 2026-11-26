namespace Common.Coding.Exceptions;

/// <summary>
/// Thrown when an exception is encountered when encoding.
/// </summary>
public class CodingException(string message) : Exception(message);