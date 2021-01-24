using System;
using System.Collections.Generic;

namespace SnakeLadder.Main
{
    public class Game
    {
        private readonly IEndGameListener _endGameListner;
        private IPlayerProgressListener _playerProgressListner;

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
            _board = new Board(_gameSettings.BoardSetting.Min, _gameSettings.BoardSetting.Max);
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
            if (_gameResult.Status == GameStatus.InProgress)
                throw new InvalidOperationException($"cannot start the game which is already running");

            if (Players.Count < _gameSettings.MinPlayersNeeded)
                throw new InvalidOperationException($"minimum {_gameSettings.MinPlayersNeeded} player(s) needed to start the game");

            _gameResult.RegisterPlayers(Players);

            foreach (var player in Players)
            {
                var playerResult = PlayTurn(player);
                _playerProgressListner?.OnPlayed(playerResult);

                _gameResult.AddResult(player, playerResult);

                if (_gameSettings.ShouldStopTheGame(_gameResult) == true)
                    _endGameListner.OnEndGame(_gameResult);
            }

            _endGameListner.OnEndGame(_gameResult);
        }

        public void Stop()
        {
            if(_gameResult.Status == GameStatus.NotStarted || _gameResult.Status == GameStatus.Stopped)
                throw new InvalidOperationException($"cannot stop the game which is not started yet");

            _gameResult.SetEndStatus();

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
    }
}