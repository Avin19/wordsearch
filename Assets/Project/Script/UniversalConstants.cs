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

        public enum SceneIndex { MAIN_MENU = 0, MAIN_GAMEPLAY = 1, LOADING_PANEL = 2, LEADERBOARD = 3 }
        public enum LeaderBoardCategory { WEEKLY, ALL_TIME, FRIENDS }
        public enum LeaderBoardResult { FAILURE = 0, SUCCESS = 1, ERROR = 2 }
        public enum MedalType { GOLD = 0, SILVER = 1, BRONZE = 2 }

        public enum GameStatus
        {
            NOT_STARTED = 0,
            PLAYING = (1 << 0),
            PAUSED = (1 << 1),
            WON = (1 << 2),
            LOST = (1 << 3),
            LEADERBOARD = (1 << 4)
        }

        //                  LOADING
        public enum LoadingPanelStatus { DISABLE, ENABLE, ENABLE_WITHOUT_LOAD, UPDATE }

        //                  LEADERBOARD
        public const int PLAYER_LIMIT = 10;

        //                  API-VALUES
        public const string GOOGLE_SHEETS_API_KEY = "AIzaSyDGcoCYiLbzkroM54VGKqQMj9pl83pjrfc";
        public const string SPREADSHEET_ID = "1VqmBuvmj9kNdr1fucZuWyxM-cU3nov0T8TgEefUk808";
        public const string SHEET_NAME = "Sheet1";
        public const string WORD_LIST_RANGE = "B1:B100";

        //                  REWARDS
        public const string DAILY_LOGIN_COUNT_LABEL = "DailyLoginCount";
        public const string PLAYER_COIN_COUNT_LABEL = "PlayerCoinCount";
        public const string LAST_CLAIM_LABEL = "LastRewardClaim";
        public const int LEVEL_WON_COINS_AMT = 50;
    }
}