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
                if (PlayerResults.All(x => x.Value.Status == MoveStatus.Stopped))
                    return GameStatus.Stopped;

                if (PlayerResults.Any(x => x.Value.Status == MoveStatus.Won))
                    return GameStatus.Completed;

                if (PlayerResults.Any(x => x.Value.Status == MoveStatus.Moved
                                     || x.Value.Status == MoveStatus.Denied))
                    return GameStatus.InProgress;

                return GameStatus.NotStarted;
            }
        }

        public bool IsRunning => Status == GameStatus.InProgress || Status == GameStatus.Completed;

        internal void AddResult(Player player, PlayerMoveResult result)
        {
            if (PlayerResults.ContainsKey(player) == false)
            {
                PlayerResults.Add(player, result);
            }
            else
                PlayerResults[player] = result;
        }

        internal void RegisterPlayers(List<Player> players)
        {
            PlayerResults.Clear();
            foreach (var player in players)
            {
                PlayerResults.Add(player, new PlayerMoveResult(0, MoveStatus.Stopped));
            }
        }

        internal void SetEndResult()
        {
            foreach (var playerResult in PlayerResults)
            {
                if (playerResult.Value.Status != MoveStatus.Won)
                    playerResult.Value.Status = MoveStatus.Stopped;
            }
        }
    }
}