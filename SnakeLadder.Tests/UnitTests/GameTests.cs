using FluentAssertions;
using Moq;
using SnakeLadder.Main;
using System;
using System.Linq;
using Xunit;

namespace SnakeLadder.Tests.UnitTests
{
    [Trait("Tests", "UnitTests")]
    public class GameTests
    {
        [Fact(DisplayName = "Game should be able to add player")]
        public void Game_should_be_able_to_add_player()
        {
            var diceMock = new Mock<Dice>();
            var endGameListnerMock = new Mock<IEndGameListener>();

            GameSetting gameSetting = GetSettings();
            var player = new Player { Id = "p123" };

            var game = new Game(diceMock.Object, gameSetting, endGameListnerMock.Object);

            game.AddPlayer(player);

            game.Players.Count.Should().Be(1);
            game.Players[0].Id.Should().Be(player.Id);
        }

        [Fact(DisplayName = "Game should not allow adding players more than set limits")]
        public void Game_should_not_allow_adding_players_more_than_set_limits()
        {
            var diceMock = new Mock<Dice>();
            var endGameListnerMock = new Mock<IEndGameListener>();

            var gameSetting = GetSettings();
            var player = new Player { Id = "p123" };

            var game = new Game(diceMock.Object, gameSetting, endGameListnerMock.Object);

            game.AddPlayer(player);

            Action action = () => game.AddPlayer(player);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact(DisplayName = "Game should be able to remove player")]
        public void Game_should_be_able_to_remove_player()
        {
            var diceMock = new Mock<Dice>();
            var endGameListnerMock = new Mock<IEndGameListener>();

            GameSetting gameSetting = GetSettings();
            var player = new Player { Id = "p123" };

            var game = new Game(diceMock.Object, gameSetting, endGameListnerMock.Object);
            game.AddPlayer(player);

            game.RemovePlayer(player);

            game.Players.Count.Should().Be(0);
        }

        [Fact(DisplayName = "Game should fail when removing player which does not exists")]
        public void Game_should_fail_when_removing_player_which_does_not_exists()
        {
            var diceMock = new Mock<Dice>();
            var endGameListnerMock = new Mock<IEndGameListener>();

            var gameSetting = GetSettings();

            var player = new Player { Id = "p123" };

            var game = new Game(diceMock.Object, gameSetting, endGameListnerMock.Object);
            
            Action action = () => game.RemovePlayer(player);

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact(DisplayName = "When starting already running game should fail")]
        public void When_starting_already_running_game_should_fail()
        {
            var diceMock = new Mock<Dice>();
            var endGameListnerMock = new Mock<IEndGameListener>();

            var gameSetting = GetSettings();

            var player = new Player { Id = "p123" };

            var game = new Game(diceMock.Object, gameSetting, endGameListnerMock.Object);
            game.AddPlayer(player);
            game.Start();

            Action action = () => game.Start();

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact(DisplayName = "Game when started with players less than required should fail")]
        public void Game_when_started_with_players_less_than_required_should_fail()
        {
            var diceMock = new Mock<Dice>();
            var endGameListnerMock = new Mock<IEndGameListener>();

            var gameSetting = GetSettings();

            var player = new Player { Id = "p123" };

            var game = new Game(diceMock.Object, gameSetting, endGameListnerMock.Object);

            Action action = () => game.Start();

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact(DisplayName = "Game when finished should return game result for winning")]
        public void Game_when_finished_should_return_game_result_for_winning()
        {
            var gameSetting = GetSettings();
            var diceMock = new Mock<Dice>();
            diceMock.Setup(x => x.Roll()).Returns(gameSetting.BoardSetting.Max);

            var endGameListnerMock = new Mock<IEndGameListener>();
            

            var player = new Player { Id = "p123" };

            var game = new Game(diceMock.Object, gameSetting, endGameListnerMock.Object);
            game.AddPlayer(player);

            endGameListnerMock.Setup(x => x.OnEndGame(It.IsAny<GameResult>()))
                              .Callback((GameResult gameResult) =>
                              {
                                  var playerResult = gameResult.PlayerResults.FirstOrDefault(x => x.Key.Equals(player));
                                  playerResult.Should().NotBeNull();

                                  playerResult.Value.NewPosition.Should().Be(gameSetting.BoardSetting.Max);
                                  playerResult.Value.Status.Should().Be(MoveStatus.Won);
                              });

            game.Start();
        }

        [Fact(DisplayName = "Game when player is still playing should return game result for moved")]
        public void Game_when_player_is_still_playing_should_return_game_result_for_moved()
        {
            var gameSetting = GetSettings();
            var diceMock = new Mock<Dice>();
            var newPosition = gameSetting.BoardSetting.Max / 2;
            diceMock.Setup(x => x.Roll()).Returns(newPosition);

            var endGameListnerMock = new Mock<IEndGameListener>();
            var playerProgressListnerMock = new Mock<IPlayerProgressListener>();

            var player = new Player { Id = "p123" };

            var game = new Game(diceMock.Object, gameSetting, endGameListnerMock.Object);
            game.AddPlayer(player);
            game.RegisterPlayerProgressListener(playerProgressListnerMock.Object);

            playerProgressListnerMock.Setup(x => x.OnPlayed(It.IsAny<PlayerMoveResult>()))
                              .Callback((PlayerMoveResult playerResult) =>
                              {
                                  playerResult.Should().NotBeNull();

                                  playerResult.NewPosition.Should().Be(newPosition);
                                  playerResult.Status.Should().Be(MoveStatus.Moved);
                              });

            game.Start();
        }

        [Fact(DisplayName = "Game when player is still playing should return game result for denied")]
        public void Game_when_player_is_still_playing_should_return_game_result_for_denied()
        {
            var gameSetting = GetSettings();
            var diceMock = new Mock<Dice>();
            var newPositionA = gameSetting.BoardSetting.Max/2;
            var newPositionB = gameSetting.BoardSetting.Max - 1;
            diceMock.Setup(x => x.Roll()).Returns(newPositionA);

            var endGameListnerMock = new Mock<IEndGameListener>();
            var playerProgressListnerMock = new Mock<IPlayerProgressListener>();

            var player = new Player { Id = "p123" };

            var game = new Game(diceMock.Object, gameSetting, endGameListnerMock.Object);
            game.AddPlayer(player);
            game.RegisterPlayerProgressListener(playerProgressListnerMock.Object);

            playerProgressListnerMock.Setup(x => x.OnPlayed(It.Is<PlayerMoveResult>(y=>y.NewPosition == newPositionA)))
                              .Callback((PlayerMoveResult playerResult) =>
                              {
                                  diceMock.Setup(x => x.Roll()).Returns(newPositionB);
                              });

            playerProgressListnerMock.Setup(x => x.OnPlayed(It.Is<PlayerMoveResult>(y => y.NewPosition == newPositionB)))
                              .Callback((PlayerMoveResult playerResult) =>
                              {
                                  playerResult.Should().NotBeNull();

                                  playerResult.NewPosition.Should().Be(newPositionB);
                                  playerResult.Status.Should().Be(MoveStatus.Denied);
                              });

            game.Start();
        }

        [Fact(DisplayName = "Stopping a game which is not started yet should fail")]
        public void Stopping_a_game_which_is_not_started_yet_should_fail()
        {
            var diceMock = new Mock<Dice>();
            var endGameListnerMock = new Mock<IEndGameListener>();

            var gameSetting = GetSettings();

            var player = new Player { Id = "p123" };
            var game = new Game(diceMock.Object, gameSetting, endGameListnerMock.Object);
            game.AddPlayer(player);

            Action action = () => game.Stop();

            action.Should().Throw<InvalidOperationException>();
        }

        [Fact(DisplayName = "Stopping a game which is already stopped should fail")]
        public void Stopping_a_game_which_is_already_stopped_should_fail()
        {
            var diceMock = new Mock<Dice>();
            var endGameListnerMock = new Mock<IEndGameListener>();

            var gameSetting = GetSettings();

            var player = new Player { Id = "p123" };
            var game = new Game(diceMock.Object, gameSetting, endGameListnerMock.Object);
            game.AddPlayer(player);
            game.Start();
            game.Stop();

            Action action = () => game.Stop();

            action.Should().Throw<InvalidOperationException>();
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
