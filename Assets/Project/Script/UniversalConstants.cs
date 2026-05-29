namespace WordSearch
{
    public static class UniversalConstants
    {
        public const int CELL_SIZE = 46;
        public const int GRID_SIZE = 10;
        // X: [-7 , 2] | Y: [5 , -4]
        public const int GRID_START_X = -7, GRID_START_Y = 5;

        public const int WORD_VER_FLAG = 0, WORD_DIAG_FLAG = 1;
        public const int LENGTH_VAL_OFFSET = 10, ROW_VAL_OFFSET = 10, ORIENTATION_OFFSET = 2;

        public enum SceneIndex { MAIN_MENU = 0, MAIN_GAMEPLAY = 1, LOADING_PANEL = 2, LEADERBOARD }
        public enum LeaderBoardCategory { WEEKLY, ALL_TIME, FRIENDS }
        public enum LeaderBoardResult { FAILURE = 0, SUCCESS = 1, ERROR = 2 }
        public enum MedalType { GOLD = 0, SILVER = 1, BRONZE = 2 }

        public enum LoadingPanelStatus { DISABLE, ENABLE, UPDATE }
    }
}