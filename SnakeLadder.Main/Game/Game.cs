using System;
using System.Collections.Generic;

namespace SnakeLadder.Main
{
    public class Game
    {
        private readonly IEndGameListener _endGameListner;
        private IPlayerProgressListener _playerProgressListner;
        private readonly object gameLock = new object();

        private readonly GameSetting _gameSettings;
        private readonly Board _board;
        private readonly Dice _dice;
        public readonly List<Player> Players = new List<Player>();
        public readonly GameResult _gameResult = new GameResult();

        public Game(Dice dice, GameSetting gameSetting, IEndGameListener endGameListner)
        {
            _endGameListner = endGameListner;
            _gameSettings = gameSetting;
            _dice = dice;
            _board = new BoardBuilder()
                    .WithMinPosition(_gameSettings.BoardSetting.Min)
                    .WithMaxPosition(_gameSettings.BoardSetting.Max)
                    .WithSnakes(_gameSettings.Snakes)
                    .Build();
        }

        public void AddPlayer(Player player)
        {
            if (Players.Count == _gameSettings.MaxPlayersAllowed)
                throw new InvalidOperationException($"a game can have maximium of {Players.Count} players");

            Players.Add(player);
        }

        public void RemovePlayer(Player player)
        {
            if (Players.Exists(x => x.Equals(player)) == false)
                throw new InvalidOperationException($"{player.Name} does not exists in players list");

            Players.RemoveAll(p => p.Id == player.Id);
        }

        public void RegisterPlayerProgressListener(IPlayerProgressListener playerProgressListner)
        {
            _playerProgressListner = playerProgressListner;
        }

        public void Start()
        {
            if (Players.Count < _gameSettings.MinPlayersNeeded)
                throw new InvalidOperationException($"minimum {_gameSettings.MinPlayersNeeded} player(s) needed to start the game");

            EnsureGameIsNotAlreadyStarted();

            lock (gameLock)
            {
                EnsureGameIsNotAlreadyStarted();
                PlacePlayersAtZeroPosition();
            }

            for (int i = 0; i < _gameSettings.TotalTurns && _gameResult.IsRunning; i++)
            {
                foreach (var player in Players)
                {
                    var playerResult = PlayTurn(player);
                    _playerProgressListner?.OnPlayed(playerResult);

                    _gameResult.AddResult(player, playerResult);

                    if (_gameSettings.ShouldStopTheGame(_gameResult) == true)
                    {
                        Stop();
                        return;
                    }
                }
            }
        }

        public void Stop()
        {
            if (_gameResult.IsRunning == false)
                throw new InvalidOperationException($"cannot stop the game which is not running currently");

            _gameResult.SetEndResult();

            _endGameListner.OnEndGame(_gameResult);
        }

        private PlayerMoveResult PlayTurn(Player player)
        {
            int diceScore = player.Roll(_dice);

            if (_gameSettings.IsValidMove(player, diceScore) == false)
                return new PlayerMoveResult(player.CurrentPosition, MoveStatus.Denied);

            _board.MovePlayer(player, diceScore);

            if (_gameSettings.HasPlayerWonTheGame(player) == true)
                return new PlayerMoveResult(player.CurrentPosition, MoveStatus.Won);

            return new PlayerMoveResult(player.CurrentPosition, MoveStatus.Moved);
        }

        private void PlacePlayersAtZeroPosition()
        {
            foreach (var player in Players)
            {
                _board.MovePlayer(player, 0);
                _gameResult.AddResult(player, new PlayerMoveResult(0, MoveStatus.Moved));
            }
        }

        private void EnsureGameIsNotAlreadyStarted()
        {
            if (_gameResult.IsRunning == true)
                throw new InvalidOperationException($"cannot start the game which is already running");
        }
    }
}