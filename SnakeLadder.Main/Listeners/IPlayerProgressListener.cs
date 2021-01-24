namespace SnakeLadder.Main
{
    public interface IPlayerProgressListener
    {
        void OnPlayed(PlayerMoveResult playerResult);
    }
}

