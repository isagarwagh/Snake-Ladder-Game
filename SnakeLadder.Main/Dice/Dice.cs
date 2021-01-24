using System;

namespace SnakeLadder.Main
{
    public abstract class Dice
    {
        protected abstract int _min { get; }
        protected abstract int _max { get; }

        public abstract int Roll();
    }
}