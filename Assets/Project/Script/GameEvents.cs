using System;
using System.Collections.Generic;

using UnityEngine;

namespace WordSearch
{
    public static class GameEvents
    {
        public static Action<string> OnLevelGenerated;
        public static Action<Vector2Int, Vector2Int> OnDragEnded;
        public static Action<int> OnCorrectSelection;

        //              AUDIO
        public static Action OnButtonClick;
        public static Action<int, float> OnGPOneShotSFXReqAsync;
        public static Action<int, bool> OnGPSFXChangeReq;
        public static Action<Action<bool>> OnToggleMute;

        //              LEADERBOARD
        public static Action<int, Action<int>> OnSubmitScore;
        public static Action<int, int, Action<int, List<LeaderBoardEntry>>> OnFetchScore;
    }
}