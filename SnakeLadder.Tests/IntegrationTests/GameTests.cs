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
        public void Game_when_player_is_still_playing_should_return_game_result_after_a_move()
        {
            var gameSetting = new GameSetting
            {
                BoardSetting = new BoardSetting { Min = 1, Max = 100 },
                MaxPlayersAllowed = 1,
                MinPlayersNeeded = 1
            };

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


        [Fact(DisplayName = "A snake moves a player from its start position to end position where start position > end position")]
        public void Game_when_player_faces_a_snake_should_move_towards_the_tail_end()
        {
            var gameSetting = new GameSetting
            {
                BoardSetting = new BoardSetting { Min = 0, Max = 100 },
                MaxPlayersAllowed = 1,
                MinPlayersNeeded = 1
            };
            gameSetting.Snakes.Add(new Snake(1, 0));
            gameSetting.Snakes.Add(new Snake(2, 0));
            gameSetting.Snakes.Add(new Snake(3, 0));
            gameSetting.Snakes.Add(new Snake(4, 0));
            gameSetting.Snakes.Add(new Snake(5, 0));
            gameSetting.Snakes.Add(new Snake(6, 0));

            var randomDice = new RandomDice();
            var player = new Player { Id = "p123" };
            int playerOriginalPosition = player.CurrentPosition;

            var playerProgressListnerMock = new Mock<IPlayerProgressListener>();
            playerProgressListnerMock.Setup(x => x.OnPlayed(It.IsAny<PlayerMoveResult>()))
                              .Callback((PlayerMoveResult playerResult) =>
                              {
                                  playerResult.Should().NotBeNull();
                                  playerResult.NewPosition.Should().Be(0);
                                  playerResult.Status.Should().Be(MoveStatus.Moved);
                              });

            var endGameListnerMock = new Mock<IEndGameListener>();
            endGameListnerMock.Setup(x => x.OnEndGame(It.IsAny<GameResult>()))
                              .Callback((GameResult gameResult) =>
                              {
                                  var playerResult = gameResult.PlayerResults.FirstOrDefault(x => x.Key.Equals(player));
                                  playerResult.Should().NotBeNull();
                                  playerResult.Value.NewPosition.Should().Be(0);
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

        [Theory(DisplayName = "The game can be started with normal dice or crooked dice")]
        [InlineData(DiceType.Random)]
        [InlineData(DiceType.RandomCrooked)]
        public void Game_can_be_started_with_normal_dice_or_crooked_dice(DiceType diceType)
        {
            var gameSetting = new GameSetting
            {
                BoardSetting = new BoardSetting { Min = 1, Max = 100 },
                MaxPlayersAllowed = 1,
                MinPlayersNeeded = 1
            };

            var dice = new DiceFactory().Create(diceType);

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

            var game = new Game(dice, gameSetting, endGameListnerMock.Object);
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
    }
}
