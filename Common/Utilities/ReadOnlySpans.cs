namespace Common.Utilities;

public static class ReadOnlySpans
{
    public static ReadOnlySpan<byte> Reversed(this ReadOnlySpan<byte> span)
    {
        var reversed = new byte[span.Length];
        for (var i = 0; i < span.Length; i++)
        {
            reversed[i] = span[span.Length - i - 1];
        }

        return reversed.AsSpan();
    }
}