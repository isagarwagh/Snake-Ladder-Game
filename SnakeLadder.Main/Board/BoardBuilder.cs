using System.Collections.Generic;

namespace SnakeLadder.Main
{
    public class BoardBuilder
    {
        private Board _board;
        private int _min;
        private int _max;
        private IEnumerable<Snake> _snakes;

        public BoardBuilder WithMinPosition(int min)
        {
            _min = min;
            return this;
        }

        public BoardBuilder WithMaxPosition(int max)
        {
            _max = max;
            return this;
        }

        public BoardBuilder WithSnakes(IEnumerable<Snake> snakes)
        {
            _snakes = snakes;
            return this;
        }

        public Board Build()
        {
            _board = new Board(_min, _max);
            foreach (var snake in _snakes)
            {
                _board.AddPositionConnector(snake);
            }

            return _board;
        }        
    }
}