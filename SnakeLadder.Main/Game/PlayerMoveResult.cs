namespace SnakeLadder.Main
{
    public class PlayerMoveResult
    {
        public PlayerMoveResult(int newPosition, MoveStatus status)
        {
            NewPosition = newPosition;
            Status = status;
        }

        public int NewPosition { get; set; }
        public MoveStatus Status { get; set; }
    }
}