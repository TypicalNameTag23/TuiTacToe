using Common;
using Painter.Drawables;
using Painter.Drawing;

namespace Client.Views;

public class LeaderboardView : Drawable
{
    private readonly LeaderboardPlacement? _first;
    private readonly LeaderboardPlacement? _second;
    private readonly LeaderboardPlacement? _third;

    public LeaderboardView(LeaderboardPlacement? first, LeaderboardPlacement? second, LeaderboardPlacement? third)
    {
        _first = first;
        _second = second;
        _third = third;
        
        BuildDrawable();
    }

    private void BuildDrawable()
    {
        var first = new TextBlock
        {
            Text = _first is null ? $"No First Place!!" : $"1: {_first.Username} | Wins: {_first.Wins}",
            Height = 1
        };
        
        var second = new TextBlock
        {
            Text = _second is null ? $"No Second Place!!" : $"2: {_second.Username} | Wins: {_second.Wins}",
            Height = 1
        };
        
        var third = new TextBlock
        {
            Text = _third is null ? $"No Third Place!!" : $"3: {_third.Username} | Wins: {_third.Wins}",
            Height = 1
        };

        Attach(new StackPanel
        {
            Orientation = Orientation.Vertical,
            Content =
            [
                first,
                second,
                third
            ]
        });
    }
}