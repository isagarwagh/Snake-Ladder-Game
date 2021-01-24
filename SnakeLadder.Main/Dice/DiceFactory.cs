using System;

namespace SnakeLadder.Main
{
    public class DiceFactory
    {
        public Dice Create(DiceType diceType)
        {
            switch (diceType)
            {
                case DiceType.Random:
                    return new RandomDice();
                case DiceType.RandomCrooked:
                    return new RandomCrookedDice();
            }

            throw new NotSupportedException($"{diceType.ToString()} is not a supported device");
        }
    }
}