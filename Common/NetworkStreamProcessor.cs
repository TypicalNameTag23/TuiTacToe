using System.Buffers;
using System.IO.Pipelines;
using System.Net.Sockets;

namespace Common;

public class NetworkStreamProcessor(NetworkStream stream, Func<ReadOnlySequence<byte>, int> processData)
{
    private const int MinimumBufferSize = 512;
    
    private Pipe _pipe;

    public async Task StartAsync(CancellationToken token)
    {
        _pipe = new Pipe();
        var fillTask = FillPipeAsync(token);
        var readTask = ReadPipeAsync(token);
        
        await Task.WhenAll(fillTask, readTask);
    }
    
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

    private async Task ReadPipeAsync(CancellationToken token)
    {
        var reader = _pipe.Reader;
        while (!token.IsCancellationRequested)
        {
            var result = await reader.ReadAsync(token);
            var buffer = result.Buffer;
            var dataProcessed = processData(buffer);
            if (dataProcessed == -1)
            {
                continue;
            }

            reader.AdvanceTo(buffer.GetPosition(dataProcessed));
            if (result.IsCompleted)
            {
                break;
            }
        }

        await reader.CompleteAsync();
    }
}