using Painter.Drawables;
using Painter.Drawing;
using Painter.Drawing.Canvas;
using Painter.Events;

namespace Client.Game.Drawables;

public class HsvColorSpaceVisualizer : Drawable
{
    private readonly Timer _timer;
    private double _unfocusedValue = 1;
    private double _currentValue = 1;

    public HsvColorSpaceVisualizer()
    {
        _timer = new Timer(_ =>
        {
            HueOffset += 1;
            if (HueOffset > 359)
            {
                HueOffset = 0;
            }
        }, null , 250, 50);
    }

    public double HueOffset
    {
        get;
        set
        {
            var offset = value;
            if (offset is > 359 or < 0)
            {
                offset = 0;
            }

            SetAndRedrawIfChanged(ref field, offset);
        }
    }

    public double SaturationOffset
    {
        get;
        set
        {
            var offset = value;
            if (offset is > 1 or < 0)
            {
                offset = 0;
            }
            
            SetAndRedrawIfChanged(ref field, offset);
        }
    }

    public double Value
    {
        get => _currentValue;
        set
        {
            var clamped = value;
            if (clamped > 1)
            {
                clamped = 1;
            }
            else if (clamped < 0)
            {
                clamped = 0;
            }
            
            if (_currentValue == clamped)
            {
                return;
            }

            _currentValue = clamped;
            _unfocusedValue = clamped;
            RequestRedraw();
        }
    }

    public override bool Focusable { get; set; } = true;

    public override bool Focused
    {
        get;
        set
        {
            if (field == value)
            {
                return;
            }
            
            field = value;
            _currentValue = Focused ? 0.7 : _unfocusedValue;
            RequestRedraw();
        }
    }

    public override void Draw(ICanvas canvas)
    {
        var width = Width;
        var height = Height;
        
        var hueStep = 359d / width;
        var saturationStep = 1d / height;
        for (var y = 0; y < canvas.Height; y++)
        {
            for (var x = 0; x < canvas.Width; x++)
            {
                var hue = hueStep * x + HueOffset;
                if (hue > 359)
                {
                    hue -= 359;
                }
                
                var saturation = saturationStep * y + SaturationOffset;
                if (saturation > 1)
                {
                    saturation -= 1;
                }
                
                canvas.Draw(x, y, " ", new StrokeStyle
                {
                    BackgroundColor = Color.FromHsv(hue, saturation, Value)
                });
            }
        }
    }

    public override void Interact(KeyPressedEventArgs args)
    {
        if (args.Key.KeyType != ConsoleKey.Spacebar)
        {
            return;
        }
        
        args.Handled = true;
        SaturationOffset += 1d / Height;
        RequestRedraw();
    }
}