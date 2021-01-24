using System;
using System.Collections.Generic;
using System.Linq;

namespace SnakeLadder.Main
{
    public class GameSetting
    {
        public int MaxPlayersAllowed { get; set; }
        public BoardSetting BoardSetting { get; set; }
        public int MinPlayersNeeded { get; set; } = 1;
        public List<Snake> Snakes { get; set; } = new List<Snake>();

        public bool HasPlayerWonTheGame(Player player) => player.CurrentPosition == BoardSetting.Max;

        public bool IsValidMove(Player player, int stepCount) => player.CurrentPosition + stepCount <= BoardSetting.Max;

        public bool ShouldStopTheGame(GameResult result) 
            => result.PlayerResults.Any(playerResult => playerResult.Value.Status == MoveStatus.Won);
    }
}