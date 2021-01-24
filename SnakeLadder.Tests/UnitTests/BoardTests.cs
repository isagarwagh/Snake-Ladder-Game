using FluentAssertions;
using Moq;
using SnakeLadder.Main;
using System;
using Xunit;

namespace SnakeLadder.Tests.UnitTests
{
    [Trait("Tests", "UnitTests")]
    public class BoardTests
    {
        [Fact(DisplayName = "Board should be able to move player")]
        public void Board_should_be_able_to_move_player()
        {
            var player = new Player { Id = "p123" };
            var board = new Board(0, 100);

            int playerOriginalPosition = player.CurrentPosition;
            int stepsToTake = 10;

            board.MovePlayer(player, stepsToTake);

            int playerNewPosition = playerOriginalPosition + stepsToTake;

            player.CurrentPosition.Should().Be(playerNewPosition);
        }

        [Fact(DisplayName = "Board should be able to add position connector")]
        public void Board_should_be_able_to_add_position_connector()
        {
            var positionConnector = new Mock<PositionConnector>(14, 7);
            var board = new Board(0, 100);

            Action action = () => board.AddPositionConnector(positionConnector.Object);

            action.Should().NotThrow();
        }

        [Fact(DisplayName = "Board should be able to change player position if encounter a snake")]
        public void Board_should_be_able_to_change_player_position_if_encounter_a_snake()
        {
            var positionConnector = new Mock<PositionConnector>(14, 7);
            var board = new Board(0, 100);
            board.AddPositionConnector(positionConnector.Object);
            var player = new Player { Id = "p123" };
            int playerOriginalPosition = player.CurrentPosition;
            int stepsToTake = 14;

            board.MovePlayer(player, stepsToTake);

            player.CurrentPosition.Should().Be(positionConnector.Object.End);
        }

        [Fact(DisplayName = "Board should not allow add position connector which starts at end of another connector")]
        public void Board_should_not_allow_add_position_connector_which_starts_at_end_of_another_connector()
        {
            var positionConnectorA = new Mock<PositionConnector>(2, 14);
            var positionConnectorB = new Mock<PositionConnector>(14, 7);
            var board = new Board(0, 100);
            board.AddPositionConnector(positionConnectorA.Object);

            Action action = () => board.AddPositionConnector(positionConnectorB.Object);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact(DisplayName = "Board should not allow add position connector which ends at start of another connector")]
        public void Board_should_not_allow_add_position_connector_which_ends_at_start_of_another_connector()
        {
            var positionConnectorA = new Mock<PositionConnector>(7, 14);
            var positionConnectorB = new Mock<PositionConnector>(9, 7);
            var board = new Board(0, 100);
            board.AddPositionConnector(positionConnectorA.Object);

            Action action = () => board.AddPositionConnector(positionConnectorB.Object);

            action.Should().Throw<InvalidOperationException>();
        }
    }
}
