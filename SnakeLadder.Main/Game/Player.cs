using System;

namespace SnakeLadder.Main
{
    public class Player
    {
        internal int CurrentPosition { get; private set; } = 0;
        public string Name { get; set; }
        public string Id { get; set; }

        internal void TakeSteps(int stepCount)
        {
            CurrentPosition += stepCount;
        }

        internal int Roll(Dice dice)
        {
            return dice.Roll();
        }

        public override bool Equals(object obj)
        {
            var player = obj as Player;
            return player != null && string.Equals(Id, player.Id, StringComparison.InvariantCultureIgnoreCase);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }
    }
}