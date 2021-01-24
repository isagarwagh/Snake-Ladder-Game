using FluentAssertions;
using Moq;
using SnakeLadder.Main;
using System;
using Xunit;

namespace SnakeLadder.Tests.UnitTests
{
    [Trait("Tests", "UnitTests")]
    public class DiceTests
    {
        [Fact(DisplayName = "Random dice should be able to generate a number between 1 to 6")]
        public void Random_dice_should_be_able_to_generate_a_number_between_1_to_6()
        {
            var dice = new RandomDice();
            var diceScore = dice.Roll();

            diceScore.Should().BeInRange(1, 6);
        }

        [Fact(DisplayName = "Crooked dice should be able to generate an even number between 1 to 6")]
        public void Crooked_dice_should_be_able_to_generate_a_number_between_1_to_6()
        {
            var dice = new RandomCrookedDice();
            var diceScore = dice.Roll();

            diceScore.Should().BeInRange(1, 6);
            var isEven = diceScore % 2 == 0;
            isEven.Should().BeTrue();
        }
    }
}
