using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeLadder.Main
{
    public class GameResult
    {
        public readonly Dictionary<Player, PlayerMoveResult> PlayerResults = new Dictionary<Player, PlayerMoveResult>();
        public GameStatus Status
        {
            get
            {
                if (PlayerResults.Any(x => x.Value.Status == MoveStatus.Stopped))
                    return GameStatus.Stopped;

                if (PlayerResults.Any(x => x.Value.Status == MoveStatus.Won))
                    return GameStatus.Completed;

                if (PlayerResults.Any(x => x.Value.Status == MoveStatus.Moved || x.Value.Status == MoveStatus.Denied))
                    return GameStatus.InProgress;

                return GameStatus.NotStarted;
            }
        }

        internal void AddResult(Player player, PlayerMoveResult result)
        {
            if (PlayerResults.ContainsKey(player) == false)
                throw new InvalidOperationException($"{player.Name} is not registred into the game result");

            PlayerResults[player] = result;
        }

        internal void RegisterPlayers(List<Player> players)
        {
            foreach (var player in players)
            {
                PlayerResults.Add(player, new PlayerMoveResult(0, MoveStatus.StartingPoint));
            }
        }

        internal void SetEndStatus()
        {
            foreach (var playerResult in PlayerResults)
            {
                playerResult.Value.Status = MoveStatus.Stopped;
            }
        }
    }
}