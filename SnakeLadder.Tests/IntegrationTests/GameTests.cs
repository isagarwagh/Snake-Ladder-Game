using FluentAssertions;
using Moq;
using SnakeLadder.Main;
using System;
using System.Linq;
using Xunit;

namespace SnakeLadder.Tests.Integration
{
    [Trait("Tests", "IntegrationTests")]
    public class GameTests
    {
        [Fact(DisplayName = "On a board (Of size 100), for a dice throw a player should move from initial position by the number on dice throw")]
        public void Game_when_player_is_still_playing_should_return_game_result_for_moved()
        {
            var gameSetting = GetSettings();
            var randomDice = new RandomDice();
            var player = new Player { Id = "p123" };
            int playerOriginalPosition = player.CurrentPosition;

            var playerProgressListnerMock = new Mock<IPlayerProgressListener>();            
            playerProgressListnerMock.Setup(x => x.OnPlayed(It.IsAny<PlayerMoveResult>()))
                              .Callback((PlayerMoveResult playerResult) =>
                              {
                                  playerResult.Should().NotBeNull();
                                  playerResult.NewPosition.Should().BeGreaterThan(playerOriginalPosition);
                                  playerResult.Status.Should().Be(MoveStatus.Moved);
                              });

            var endGameListnerMock = new Mock<IEndGameListener>();
            endGameListnerMock.Setup(x => x.OnEndGame(It.IsAny<GameResult>()))
                              .Callback((GameResult gameResult) =>
                              {
                                  var playerResult = gameResult.PlayerResults.FirstOrDefault(x => x.Key.Equals(player));
                                  playerResult.Should().NotBeNull();
                                  playerResult.Value.NewPosition.Should().BeGreaterThan(playerOriginalPosition);
                                  playerResult.Value.Status.Should().Be(MoveStatus.Moved);
                              });

            var game = new Game(randomDice, gameSetting, endGameListnerMock.Object);
            game.AddPlayer(player);
            game.RegisterPlayerProgressListener(playerProgressListnerMock.Object);

            game.Start();

            endGameListnerMock.Setup(x => x.OnEndGame(It.IsAny<GameResult>()))
                              .Callback((GameResult gameResult) =>
                              {
                                  var playerResult = gameResult.PlayerResults.FirstOrDefault(x => x.Key.Equals(player));
                                  playerResult.Value.Status.Should().Be(MoveStatus.Stopped);
                              });
            game.Stop();
        }

        private static GameSetting GetSettings()
        {
            return new GameSetting
            {
                BoardSetting = new BoardSetting { Min = 1, Max = 100 },
                MaxPlayersAllowed = 1,
                MinPlayersNeeded = 1
            };
        }
    }
}
