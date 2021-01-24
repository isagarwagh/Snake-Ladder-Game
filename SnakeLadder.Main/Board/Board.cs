using System;

namespace SnakeLadder.Main
{
    public class Board
    {
        private readonly int _minPosition;
        private readonly int _maxPosition;


        public Board(int minPosition, int maxPosition)
        {
            _minPosition = minPosition;
            _maxPosition = maxPosition;
        }

        public void MovePlayer(Player player, int stepCount)
        {
            player.TakeSteps(stepCount);
        }
    }
}
