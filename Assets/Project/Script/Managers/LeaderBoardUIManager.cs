using System.Collections.Generic;

using UnityEngine;
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

        [Header("Buttons")]
        [SerializeField] private Button _weeklyLBBt;
        [SerializeField] private Button _allTimeLBBt, _friendsLBBt;
        [SerializeField] private Button _buyCoinsBt;
        [SerializeField] private Button _closeLBBt, _infoLBBt;

        [Header("Panels")]
        [SerializeField] private RectTransform _playerList;

        [Header("Text")]
        [SerializeField] private TMPro.TMP_Text _levelTimeTxt;
        [SerializeField] private TMPro.TMP_Text _resetTimeTxt, _coinsAmtTxt;

        [Header("Score Card")]
        [SerializeField] private LBScoreCard _lbScoreCardPrefab;
        [SerializeField] private RectTransform _lbScoreCardContainer;
        [SerializeField] private Sprite[] _medalSprites;
        private List<LBScoreCard> _lbScoreCardList;

        private const int PLAYER_LIMIT = 10;

        void Start()
        {
            _lbScoreCardList = new List<LBScoreCard>();

            //              BUTTONS
            _weeklyLBBt.onClick.AddListener(() => HandleInteraction(UIInteraction.WEEKLY_LB_REQ));
            _allTimeLBBt.onClick.AddListener(() => HandleInteraction(UIInteraction.ALL_TIME_LB_REQ));
            _friendsLBBt.onClick.AddListener(() => HandleInteraction(UIInteraction.FRIENDS_LB_REQ));
            _buyCoinsBt.onClick.AddListener(() => HandleInteraction(UIInteraction.BUY_COINS_REQ));
            _closeLBBt.onClick.AddListener(() => HandleInteraction(UIInteraction.CLOSE_LB_REQ));
            _infoLBBt.onClick.AddListener(() => HandleInteraction(UIInteraction.INFO_LB_REQ));
        }

        private void HandleInteraction(UIInteraction interaction)
        {
            LeaderBoardCategory lbCategory = LeaderBoardCategory.ALL_TIME;

            switch (interaction)
            {
                case UIInteraction.WEEKLY_LB_REQ:

                    goto case UIInteraction.CALL_FETCH_SCORE;

                case UIInteraction.ALL_TIME_LB_REQ:

                    goto case UIInteraction.CALL_FETCH_SCORE;

                case UIInteraction.FRIENDS_LB_REQ:

                    goto case UIInteraction.CALL_FETCH_SCORE;

                case UIInteraction.CALL_FETCH_SCORE:
                    GameEvents.OnFetchScore((int)lbCategory, PLAYER_LIMIT, UpdatePlayerList);

                    break;

                case UIInteraction.BUY_COINS_REQ:
                    break;

                case UIInteraction.CLOSE_LB_REQ:
                    break;

                case UIInteraction.INFO_LB_REQ:
                    break;
            }
        }

        private void UpdatePlayerList(int fetchStatus, List<LeaderBoardEntry> lbEntries)
        {
            if (fetchStatus == (int)LeaderBoardResult.FAILURE) return;

            int diffNumber = 0;
            if (lbEntries.Count > _lbScoreCardList.Count)
                diffNumber = lbEntries.Count - _lbScoreCardList.Count;

            for (int i = 0; i < diffNumber; i++)
            {
                LBScoreCard scoreCard = Instantiate(_lbScoreCardPrefab, _lbScoreCardContainer);
                _lbScoreCardList.Add(scoreCard);
            }

            Sprite spriteToUse = null;
            for (int i = 0; i < lbEntries.Count; i++)
            {
                spriteToUse = null;
                LBScoreCard scoreCard = _lbScoreCardList[i];

                if (i < 3)
                    spriteToUse = _medalSprites[i];

                scoreCard.SetData(lbEntries[i].PlayerRank, lbEntries[i].PlayerName, lbEntries[i].PlayerScore, spriteToUse);
            }
        }
    }
}