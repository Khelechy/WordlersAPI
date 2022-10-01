namespace WordlersAPI.Models.Constants
{
    public static class Constant
    {
        public const string StartGameRoundTopic = "start-game-round";
        public const string StopGameRoundTopic = "stop-game-round";
        public const string StoreUserGamePoint = "store-user-game-point";

        public const string GameStoreName = "GameStore_";
        public const string WordlerBotName = "WordlerBot";

        public const int BufferGameRoundTime = 10000;
        public const int BufferPoint = 2;
    }
}
