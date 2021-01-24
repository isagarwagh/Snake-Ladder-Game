using FluentAssertions;
using Moq;
using SnakeLadder.Main;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace SnakeLadder.Tests.UnitTests
{
    [Trait("Tests", "UnitTests")]
    public class PlayerTests
    {
        [Fact]
        public void Player_should_be_able_to_roll_dice()
        {
            var diceMock = new Mock<Dice>();
            var player = new Player { Id = "p123" };

            player.Roll(diceMock.Object);

            diceMock.Verify(x => x.Roll(), Times.Once);
        }

        [Fact]
        public void Player_when_takes_steps_should_update_current_position()
        {
            var player = new Player { Id = "p123" };
            var stepsToTake = 10;
            var currentPosition = player.CurrentPosition;

            player.TakeSteps(stepsToTake);

            var newPosition = currentPosition + stepsToTake;

            player.CurrentPosition.Should().Be(newPosition);
        }
    }
}
