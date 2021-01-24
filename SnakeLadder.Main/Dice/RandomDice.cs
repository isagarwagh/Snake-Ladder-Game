using System;

namespace SnakeLadder.Main
{
    public class RandomDice : Dice
    {
        private Random _random = new Random();

        protected override int _min => 1;

        protected override int _max => 6;

        public override int Roll()
        {
            return _random.Next(_min, _max);
        }
    }
}