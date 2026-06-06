using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using static WordSearch.UniversalConstants;

namespace WordSearch
{
    public class LeaderBoardUIManager : MonoBehaviour
    {
        enum UIInteraction
        {
            WEEKLY_LB_REQ = 0, ALL_TIME_LB_REQ = 1, FRIENDS_LB_REQ = 2,
            BUY_COINS_REQ = 3, CLOSE_LB_REQ = 4, INFO_LB_REQ = 5,

            CALL_FETCH_SCORE = 1000,
        }
        enum ButtonStatus
        {
            NONE = 0, ACTIVE = 1
        }

        // [SerializeField] private RectTransform _leaderboardPanel;

        [Header("Buttons")]
        [SerializeField] private Button _weeklyLBBt;
        [SerializeField] private Button _allTimeLBBt, _friendsLBBt;
        [SerializeField] private Button _buyCoinsBt;
        [SerializeField] private Button _closeLBBt, _infoLBBt;
        [SerializeField] private Button _backBt;

        [SerializeField] private Color[] _btColors;
        private Button _prevActiveBt;

        // [Header("Panels")]
        // [SerializeField] private RectTransform _playerList;

        [Header("Text")]
        [SerializeField] private TMPro.TMP_Text _levelTimeTxt;
        [SerializeField] private TMPro.TMP_Text _resetTimeTxt, _coinsAmtTxt;

        [Header("Score Card")]
        [SerializeField] private LBScoreCard _lbScoreCardPrefab;
        [SerializeField] private RectTransform _lbScoreCardContainer;
        [SerializeField] private Sprite[] _medalSprites;
        private List<LBScoreCard> _lbScoreCardList;

        [Header("Player Score Card")]
        [SerializeField] private RectTransform _playerCard;
        [SerializeField] private TMPro.TMP_Text _playerRankTxt;
        [SerializeField] private TMPro.TMP_Text _playerScoreTxt;
        [SerializeField] private Image _playerProgressImg;

        [Header("Game Data")]
        [SerializeField] private GameData _gameData;

        private const string PLAYER_LABEL = "You";

        void Start()
        {
            _lbScoreCardList = new List<LBScoreCard>();
            _prevActiveBt = _weeklyLBBt;

            //              BUTTONS
            _weeklyLBBt.onClick.AddListener(() => HandleInteraction(UIInteraction.WEEKLY_LB_REQ));
            _allTimeLBBt.onClick.AddListener(() => HandleInteraction(UIInteraction.ALL_TIME_LB_REQ));
            _friendsLBBt.onClick.AddListener(() => HandleInteraction(UIInteraction.FRIENDS_LB_REQ));
            _buyCoinsBt.onClick.AddListener(() => HandleInteraction(UIInteraction.BUY_COINS_REQ));
            _closeLBBt.onClick.AddListener(() => HandleInteraction(UIInteraction.CLOSE_LB_REQ));
            _infoLBBt.onClick.AddListener(() => HandleInteraction(UIInteraction.INFO_LB_REQ));

            Invoke(nameof(Initialize), 1.0f);           // Wait a sec for events to be assigned
        }

        private void Initialize()
        {
            GameEvents.OnFetchScore?.Invoke((int)LeaderBoardCategory.WEEKLY, PLAYER_LIMIT, (fetchStatus, lbEntries) => {
                UpdatePlayerList(fetchStatus, lbEntries);
                GameEvents.OnLoadingUpdate?.Invoke(LoadingPanelStatus.DISABLE, 0f);
            });
        }

        private void HandleInteraction(UIInteraction interaction)
        {
            // Debug.Log($"Interaction Made: {interaction}");
            LeaderBoardCategory lbCategory = LeaderBoardCategory.ALL_TIME;

            switch (interaction)
            {
                case UIInteraction.WEEKLY_LB_REQ:
                    _prevActiveBt.image.color = _btColors[(int)ButtonStatus.NONE];
                    _weeklyLBBt.image.color = _btColors[(int)ButtonStatus.ACTIVE];

                    _prevActiveBt = _weeklyLBBt;
                    lbCategory = LeaderBoardCategory.WEEKLY;

                    goto case UIInteraction.CALL_FETCH_SCORE;

                case UIInteraction.ALL_TIME_LB_REQ:
                    _prevActiveBt.image.color = _btColors[(int)ButtonStatus.NONE];
                    _allTimeLBBt.image.color = _btColors[(int)ButtonStatus.ACTIVE];

                    _prevActiveBt = _allTimeLBBt;
                    lbCategory = LeaderBoardCategory.ALL_TIME;

                    goto case UIInteraction.CALL_FETCH_SCORE;

                case UIInteraction.FRIENDS_LB_REQ:
                    _prevActiveBt.image.color = _btColors[(int)ButtonStatus.NONE];
                    _friendsLBBt.image.color = _btColors[(int)ButtonStatus.ACTIVE];

                    _prevActiveBt = _friendsLBBt;
                    lbCategory = LeaderBoardCategory.FRIENDS;

                    goto case UIInteraction.CALL_FETCH_SCORE;

                case UIInteraction.CALL_FETCH_SCORE:
                    GameEvents.OnFetchScore?.Invoke((int)lbCategory, PLAYER_LIMIT, UpdatePlayerList);

                    break;

                case UIInteraction.BUY_COINS_REQ:
                    break;

                case UIInteraction.CLOSE_LB_REQ:
					SceneManager.UnloadSceneAsync((int)SceneIndex.LEADERBOARD);
					GameEvents.OnLoadingUpdate?.Invoke(LoadingPanelStatus.ENABLE, (int)_gameData.PrevSceneIndex);
					_gameData.PrevSceneIndex = SceneIndex.LEADERBOARD;

                    break;

                case UIInteraction.INFO_LB_REQ:
                    break;
            }
        }

public List<LeaderBoardEntry> testLBEntries;
        private void UpdatePlayerList(int fetchStatus, List<LeaderBoardEntry> lbEntries)
        {
            testLBEntries = lbEntries;
            if (fetchStatus == (int)LeaderBoardResult.FAILURE) return;

            int diffNumber = 0;
            if ((lbEntries.Count - 1) > _lbScoreCardList.Count)             // -1 to offset player data
                diffNumber = lbEntries.Count - _lbScoreCardList.Count - 1;

            for (int i = 0; i < diffNumber; i++)
            {
                LBScoreCard scoreCard = Instantiate(_lbScoreCardPrefab, _lbScoreCardContainer);
                _lbScoreCardList.Add(scoreCard);
            }

            int currPlayerRank = -1;
            if (string.IsNullOrEmpty(lbEntries[lbEntries.Count - 1].PlayerName))
            {
                currPlayerRank = lbEntries[lbEntries.Count - 1].PlayerRank;
                _playerCard.gameObject.SetActive(false);
            }
            else
            {
                _playerCard.gameObject.SetActive(true);
                _playerRankTxt.text = lbEntries[lbEntries.Count - 1].PlayerRank.ToString();
                _playerScoreTxt.text = lbEntries[lbEntries.Count - 1].PlayerScore.ToString();
            }

            Sprite spriteToUse = null;
            for (int i = 0; i < (lbEntries.Count - 1); i++)
            {
                spriteToUse = null;
                LBScoreCard scoreCard = _lbScoreCardList[i];

                if (i < 3)
                    spriteToUse = _medalSprites[i];

                if (currPlayerRank != -1 && i == currPlayerRank)
                    scoreCard.SetData(lbEntries[i].PlayerRank, PLAYER_LABEL, lbEntries[i].PlayerScore, spriteToUse);
                else
                    scoreCard.SetData(lbEntries[i].PlayerRank, lbEntries[i].PlayerName, lbEntries[i].PlayerScore, spriteToUse);
            }

            // Player is within the limit requested and on the main list
        }
    }
}