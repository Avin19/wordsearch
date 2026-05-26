// #define TEST_LEADERBOARD

using System;
using System.Collections.Generic;

using GooglePlayGames;
using GooglePlayGames.BasicApi;
using UnityEngine.SocialPlatforms;

using UnityEngine;
using static WordSearch.UniversalConstants;

namespace WordSearch
{
    [Serializable]
    public class LeaderBoardEntry
    {
        public string PlayerName;
        public int PlayerRank;
        public long PlayerScore;

        public LeaderBoardEntry(string name, int rank, long score)
        {
            PlayerName = name;
            PlayerRank = rank;
            PlayerScore = score;
        }
    }

    public class LeaderBoardManager : MonoBehaviour
    {
        private const string LEADERBOARD_ID = "Play Console ID";
        private const string DEFAULT_TAG = "LB_Score";

        private List<LeaderBoardEntry> _lbEntries;

        void Start()
        {
            PlayGamesPlatform.Activate();
            // PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);

            _lbEntries = new List<LeaderBoardEntry>();

            GameEvents.OnSubmitScore += SubmitScore;
            GameEvents.OnFetchScore += FetchScores;
        }

        private void ProcessAuthentication(SignInStatus status)
        {
            if (status == SignInStatus.Success)
            {
                // Continue with Play Games Services
            }
            else
            {
                // Disable your integration with Play Games Services or show a login button
                // to ask users to authenticate. Clicking it should call
                // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
            }
        }

        private void SubmitScore(int score, Action<int> OnComplete)
        {
#if TEST_LEADERBOARD
            Debug.Log($"Score Submitted | LEADERBOARD_ID: {LEADERBOARD_ID} | Score: {score}");
            OnComplete?.Invoke((int)LeaderBoardResult.SUCCESS);
#else
            PlayGamesPlatform.Instance.ReportScore(score, LEADERBOARD_ID, DEFAULT_TAG, (bool success) =>
            {
                OnComplete?.Invoke(success ? (int)LeaderBoardResult.SUCCESS : (int)LeaderBoardResult.FAILURE);
            });
#endif
        }

        private async void FetchScores(int lbCategory, int limit, Action<int, List<LeaderBoardEntry>> OnComplete)
        {
            _lbEntries.Clear();

#if TEST_LEADERBOARD
            switch ((LeaderBoardCategory)lbCategory)
            {
                case LeaderBoardCategory.WEEKLY:
                    _lbEntries.Add(new LeaderBoardEntry("PlayerWeekly1", 1, 153791));
                    _lbEntries.Add(new LeaderBoardEntry("PlayerWeekly2", 2, 121321));
                    _lbEntries.Add(new LeaderBoardEntry("PlayerWeekly3", 3, 131461));
                    _lbEntries.Add(new LeaderBoardEntry("PlayerWeekly4", 4, 171441));
                    _lbEntries.Add(new LeaderBoardEntry("PlayerWeekly5", 5, 133731));
                    _lbEntries.Add(new LeaderBoardEntry("PlayerWeekly6", 6, 153621));

                    break;

                case LeaderBoardCategory.ALL_TIME:
                    _lbEntries.Add(new LeaderBoardEntry("PlayerAllTime1", 1, 125791));
                    _lbEntries.Add(new LeaderBoardEntry("PlayerAllTime2", 2, 124391));
                    _lbEntries.Add(new LeaderBoardEntry("PlayerAllTime3", 3, 134261));
                    _lbEntries.Add(new LeaderBoardEntry("PlayerAllTime4", 4, 176241));
                    _lbEntries.Add(new LeaderBoardEntry("PlayerAllTime5", 5, 136231));
                    _lbEntries.Add(new LeaderBoardEntry("PlayerAllTime6", 6, 154631));

                    break;

                case LeaderBoardCategory.FRIENDS:
                    _lbEntries.Add(new LeaderBoardEntry("Friend1", 1, 12591));
                    _lbEntries.Add(new LeaderBoardEntry("Friend2", 2, 12191));
                    _lbEntries.Add(new LeaderBoardEntry("Friend3", 3, 13461));
                    _lbEntries.Add(new LeaderBoardEntry("Friend4", 4, 17541));
                    _lbEntries.Add(new LeaderBoardEntry("Friend5", 5, 13431));
                    _lbEntries.Add(new LeaderBoardEntry("Friend6", 6, 15631));

                    break;
            }

            OnComplete?.Invoke((int)LeaderBoardResult.SUCCESS, _lbEntries);
            return;
#endif
            LeaderboardTimeSpan lbTimeSpan = LeaderboardTimeSpan.AllTime;
            LeaderboardCollection lbCollection = LeaderboardCollection.Public;

            switch ((LeaderBoardCategory)lbCategory)
            {
                case LeaderBoardCategory.WEEKLY:
                    lbTimeSpan = LeaderboardTimeSpan.Weekly;
                    lbCollection = LeaderboardCollection.Public;

                    break;

                case LeaderBoardCategory.ALL_TIME:
                    lbTimeSpan = LeaderboardTimeSpan.AllTime;
                    lbCollection = LeaderboardCollection.Public;

                    break;

                case LeaderBoardCategory.FRIENDS:
                    lbTimeSpan = LeaderboardTimeSpan.AllTime;
                    lbCollection = LeaderboardCollection.Social;

                    break;
            }

            PlayGamesPlatform.Instance.LoadScores(
                LEADERBOARD_ID,
                LeaderboardStart.TopScores,
                limit,
                lbCollection,
                lbTimeSpan,
                (LeaderboardScoreData data) => ProcessLeaderBoardScores(data, OnComplete)
            );
        }

        private void ProcessLeaderBoardScores(LeaderboardScoreData data, Action<int, List<LeaderBoardEntry>> OnLeaderBoardListMade)
        {
            if (data.Status != ResponseStatus.Success)
            {
                OnLeaderBoardListMade?.Invoke((int)LeaderBoardResult.FAILURE, null);
                return;
            }

            for (int i = 0; i < data.Scores.Length; i++)
            {
                IScore tempScore = data.Scores[i];
                _lbEntries.Add(new LeaderBoardEntry(tempScore.userID, tempScore.rank, tempScore.value));
            }

            OnLeaderBoardListMade?.Invoke((int)LeaderBoardResult.SUCCESS, _lbEntries);
        }
    }
}