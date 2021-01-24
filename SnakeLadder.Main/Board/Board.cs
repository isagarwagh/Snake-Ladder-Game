using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeLadder.Main
{
    public class Board
    {
        private readonly int _minPosition;
        private readonly int _maxPosition;
        private List<PositionConnector> _positionConnectors = new List<PositionConnector>();

        public Board(int minPosition, int maxPosition)
        {
            _minPosition = minPosition;
            _maxPosition = maxPosition;
        }

        public void MovePlayer(Player player, int stepCount)
        {
            player.TakeSteps(stepCount);

            var connector = _positionConnectors.FirstOrDefault(x => x.Start == player.CurrentPosition);
            if(connector != null)
                player.JumpTo(connector.End);
        }

        public void AddPositionConnector(PositionConnector positionConnector)
        {
            if (_positionConnectors.Exists(x => x.End == positionConnector.Start) == true)
                throw new InvalidOperationException("position connector cannot start at the end of another connector");

            if (_positionConnectors.Exists(x => x.Start == positionConnector.End) == true)
                throw new InvalidOperationException("position connector cannot end at another connector");

            if (_positionConnectors.Exists(x => x.Start == positionConnector.End) == true
              || _positionConnectors.Exists(x => x.End == positionConnector.Start) == true)
                throw new InvalidOperationException("position connector cannot end at another connector");


            if (_positionConnectors.Exists(x => x.Start == positionConnector.Start && x.End == positionConnector.End) == true)
            {
                _positionConnectors.RemoveAll(x => x.Start == positionConnector.Start && x.End == positionConnector.End);
            }

            _positionConnectors.Add(positionConnector);
        }
    }
}
