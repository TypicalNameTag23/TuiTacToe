using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;

namespace Client;

/// <summary>
/// Processes incoming data from a <see cref="NetworkStream"/> and pipes it into the processData function
/// </summary>
/// <param name="stream">The <see cref="NetworkStream"/> to process incoming data from</param>
/// <param name="processData">The function to call with incoming data, takes in the raw byte data and returns the number of bytes read</param>
public class NetworkStreamProcessor(NetworkStream stream, Func<ReadOnlySequence<byte>, int> processData)
{
    private const int MinimumBufferSize = 512;
    
    private Pipe _pipe;

    /// <summary>
    /// Starts processing incoming data from the <see cref="NetworkStream"/> on a separate thread
    /// </summary>
    /// <param name="token">The <see cref="CancellationToken"/> to cancel data processing</param>
    public async Task StartAsync(CancellationToken token)
    {
        _pipe = new Pipe();
        var fillTask = FillPipeAsync(token);
        var readTask = ReadPipeAsync(token);
        
        await Task.WhenAll(fillTask, readTask);
    }
    
    /// <summary>
    /// Reads from the <see cref="NetworkStream"/> asynchronously and fills the <see cref="Pipe"/> with any incoming data
    /// </summary>
    /// <param name="token">The <see cref="CancellationToken"/> to cancel data processing</param>
    private async Task FillPipeAsync(CancellationToken token)
    {
        var writer = _pipe.Writer;
        while (!token.IsCancellationRequested)
        {
            var memory = writer.GetMemory(MinimumBufferSize);
            try
            {
                var bytesRead = await stream.ReadAsync(memory, token);
                if (bytesRead == 0)
                {
                    break;
                }
                    
                writer.Advance(bytesRead);
            }
            catch (Exception e)
            {
                Console.WriteLine("Failed to read NetworkStream into Pipe:");
                Console.WriteLine(e);
                break;
            }

            var result = await writer.FlushAsync(token);
            if (result.IsCompleted)
            {
                break;
            }
        }
          
        await writer.CompleteAsync();
    }

    /// <summary>
    /// Reads data from the <see cref="Pipe"/> asynchronously and hands it off to be processed by the <see cref="processData"/> callback
    /// </summary>
    /// <param name="token">The <see cref="CancellationToken"/> to cancel data processing</param>
    private async Task ReadPipeAsync(CancellationToken token)
    {
        var reader = _pipe.Reader;
        while (!token.IsCancellationRequested)
        {
            var result = await reader.ReadAsync(token);
            var buffer = result.Buffer;
            var dataProcessed = processData(buffer);

            reader.AdvanceTo(buffer.GetPosition(dataProcessed));
            if (result.IsCompleted)
            {
                break;
            }
        }

        await reader.CompleteAsync();
    }
}