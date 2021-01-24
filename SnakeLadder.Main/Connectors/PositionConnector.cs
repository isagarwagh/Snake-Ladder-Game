namespace SnakeLadder.Main
{
    public abstract class PositionConnector
    {
        public PositionConnector(int start, int end)
        {
            Start = start;
            End = end;
        }

        public int Start { get; }
        public int End { get; }
    }
}

