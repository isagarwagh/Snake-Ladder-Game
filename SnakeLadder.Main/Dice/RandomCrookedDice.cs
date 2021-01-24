using System;

namespace SnakeLadder.Main
{
    public class RandomCrookedDice: RandomDice
    {
        private Random _random = new Random();

        public override int Roll()
        {
            return _random.Next(_min, _max / 2) * 2;
        }
    }
}