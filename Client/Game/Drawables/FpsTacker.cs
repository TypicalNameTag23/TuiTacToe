using System.Diagnostics;
using Painter;
using Painter.Drawables;
using Painter.Drawables.Control;
using Painter.Drawing;

namespace Client.Game.Drawables;

public sealed class FpsTacker : DrawableControl
{
    // Will hit ulong.MaxValue in ~58494241 years **assuming** we have 10,000 FPS (would be a nice problem to have, but I'm too stupid to come up with optimizations like that)
    private ulong _totalFramesDrawn;
    
    // uint.MaxValue as seconds is ~136 years, ~~likely~~ don't have to worry about this overflowing
    private uint _fpsAveragesCalculated;
    private int _averageFps;
    private int _minFps = int.MaxValue;
    
    private double _renderTime;
    private double _averageRenderTime;
    private double _maxRenderTime;
    
    private readonly Stopwatch _renderTimeStopwatch;
    private readonly Stopwatch _fpsTimerStopwatch;

    public FpsTacker()
    {
        var textBlock = new TextBlock
        {
            BackgroundColor = Color.Black,
            TextColor = Color.DarkSalmon,
            Text = "a"
        };
        
        Attach(textBlock);
        _renderTimeStopwatch = new Stopwatch();
        _fpsTimerStopwatch = new Stopwatch();
        var framesDrawn = 0;
        var fps = 0;
        _fpsTimerStopwatch.Start();
        Application.BeforeApplicationDrawFrameEvent += () =>
        {
            _renderTimeStopwatch.Restart();
        };
        
        Application.AfterApplicationDrawFrameEvent += () =>
        {
            framesDrawn += 1;
            _renderTimeStopwatch.Stop();
            _renderTime = _renderTimeStopwatch.Elapsed.TotalMilliseconds;
            if (_renderTime > _maxRenderTime)
            {
                _maxRenderTime = _renderTime;
            }

            _averageRenderTime = (_averageRenderTime * _totalFramesDrawn + _renderTime) / ++_totalFramesDrawn;
            textBlock.Text = 
                $"FPS: {fps} | FPS (Low): {_minFps} | FPS (Average): {_averageFps} | Draw: {_renderTime:N2}ms | Max Draw: {_maxRenderTime:N2}ms | Average Draw: {_averageRenderTime:N2}ms | Frames Drawn: {_totalFramesDrawn}";
            if (_fpsTimerStopwatch.ElapsedMilliseconds >= 1000)
            {
                try
                {
                    fps = framesDrawn / (int) (_fpsTimerStopwatch.ElapsedMilliseconds / 1000);
                    if (fps < _minFps)
                    {
                        _minFps = fps;
                    }

                    _averageFps = (int)((_averageFps * _fpsAveragesCalculated + fps) / ++_fpsAveragesCalculated);
                    _fpsAveragesCalculated++;

                    framesDrawn = 0;
                    _fpsTimerStopwatch.Restart();
                }
                catch (DivideByZeroException) { }
            }
        };
    }
}