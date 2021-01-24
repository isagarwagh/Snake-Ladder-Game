using System;

namespace SnakeLadder.Main
{
    public class Snake : PositionConnector
    {
        public Snake(int start, int end) : base(start, end)
        {
            if (start < end)
                throw new ArgumentException($"A Snake must have a lower end compared it its start");
        }
    }
}

