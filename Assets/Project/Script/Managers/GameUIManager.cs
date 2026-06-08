#define RESET_GAME_DATA

using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

using static WordSearch.UniversalConstants;

namespace WordSearch
{
    public class GameUIManager : MonoBehaviour
    {
        enum GameUIInteraction
        {
            MAIN_MENU_REQ, NEXT_LEVEL_REQ, LEADERBOARD_REQ,
            CLAIM_DAILY_REWARD_REQ, BACK_REQ, TUTORIAL_OPEN_REQ, TUTORIAL_CLOSE_REQ
        }

        [Header("Gameplay")]
        [SerializeField] private TMPro.TMP_Text _currMonthTxt;
        [SerializeField] private TMPro.TMP_Text _currDateTxt;
        [SerializeField] private Button _backGMBt, _infoGMBt;

        [Header("Tutorial")]
        [SerializeField] private RectTransform _tutorialPanel;
        [SerializeField] private Button _backTutBt;
        [SerializeField] private UnityEngine.Video.VideoPlayer _tutorialPlayer;

        //              GRID
        [Header("GRID")]
        [SerializeField] private Image _highlightImg;
        [SerializeField] private RectTransform _highlightContainer;

        [SerializeField] private TMPro.TMP_Text _wordListTxt;
        [SerializeField] private RectTransform[] _wordCorrectArr;

        //              START
        [Header("Game Data")]
        [SerializeField] private GameData _gameData;

        [Header("Game Over")]
        [SerializeField] private RectTransform _gameOverPanel;
        [SerializeField] private Button _nextLevelGOBt, _mainMenuGOBt, _leaderBoardGOBt;
        [SerializeField] private TMPro.TMP_Text _timeElapsedTxt, _coinsGainedTxt;

        [Header("Rewards")]
        [SerializeField] private Button _claimRewardBt;
        [SerializeField] private RectTransform[] _dailyCoinBtCheckmarkImgs;
        [SerializeField] private int[] _dailyCoinsAmt;

        //          HIGHLIGHT
        // private static readonly Color CorrectSelection = new Color(0f, 0.8396226f, 0.1217717f, 1f);

        // "DAAFF9", "FAB4AE", "B3DDFD", "BDEB7B", "9EEAEA",
        // "FDAD30", "FDDF43", "D697FB", "4C9AEF", "D92243"

        private static readonly Color[] CorrectColorsArr = new Color[]
        {
            new Color(0.854902f, 0.6862745f, 0.9764706f, 1f),
            new Color(0.9803922f, 0.7058824f, 0.682353f, 1f),
            new Color(0.7019608f, 0.8666667f, 0.9921569f, 1f),
            new Color(0.7411765f, 0.9215686f, 0.4823529f, 1f),
            new Color(0.6196079f, 0.9176471f, 0.9176471f, 1f),
            new Color(0.9921569f, 0.6784314f, 0.1882353f, 1f),
            new Color(0.9921569f, 0.8745098f, 0.2627451f, 1f),
            new Color(0.8392157f, 0.5921569f, 0.9843137f, 1f),
            new Color(0.2980392f, 0.6039216f, 0.9372549f, 1f),
            new Color(0.8509804f, 0.1333333f, 0.2627451f, 1f),
        };

        //          GAME OVER
        private const int GO_STATS_WAIT_TIME = 500;

        private void OnDestroy()
        {
#if UNITY_EDITOR && RESET_GAME_DATA
            _gameData.PlayerCoinCount = 0;
            _gameData.DailyLoginCount = 0;
#endif

            GameEvents.OnLevelGenerated -= UpdateWordList;
            GameEvents.OnCorrectSelection -= SpawnHighlightBar;
            GameEvents.OnGameStatusUpdate -= HandleStatusUpdate;
        }

        void Start()
        {
            // PlayerPrefs.DeleteAll();            //TEST

            // Debug.Log($"Month: {DateTime.Today.Month} | Date: {DateTime.Today.Day}");
            _currMonthTxt.text = DateTime.Now.ToString("MMM").ToUpper();
            _currDateTxt.text = DateTime.Now.Day.ToString();

            //              BUTTONS
            _mainMenuGOBt.onClick.AddListener(() => HandleUIInteraction(GameUIInteraction.MAIN_MENU_REQ));
            _nextLevelGOBt.onClick.AddListener(() => HandleUIInteraction(GameUIInteraction.NEXT_LEVEL_REQ));
            _leaderBoardGOBt.onClick.AddListener(() => HandleUIInteraction(GameUIInteraction.LEADERBOARD_REQ));
            _backGMBt.onClick.AddListener(() => HandleUIInteraction(GameUIInteraction.BACK_REQ));

            _infoGMBt.onClick.AddListener(() => HandleUIInteraction(GameUIInteraction.TUTORIAL_OPEN_REQ));
            _backTutBt.onClick.AddListener(() => HandleUIInteraction(GameUIInteraction.TUTORIAL_OPEN_REQ));

            _tutorialPlayer.Prepare();
            InitializeRewardsUI();

            //              ACTIONS
            GameEvents.OnLevelGenerated += UpdateWordList;
            GameEvents.OnCorrectSelection += SpawnHighlightBar;
            GameEvents.OnGameStatusUpdate += HandleStatusUpdate;
        }

        private void InitializeRewardsUI()
        {
            int loginCount = PlayerPrefs.GetInt(DAILY_LOGIN_COUNT_LABEL, 0);
            int lastClaimDate = PlayerPrefs.GetInt(LAST_CLAIM_LABEL, -1);

            int currDate = DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day;


            _gameData.DailyLoginCount = loginCount;

            // Update the tickmark on all previous bts as they have been claimed or not active
            // As bts start inactive, no need to toggle interactable
            for (int i = 0; i < loginCount; i++)
                _dailyCoinBtCheckmarkImgs[i].gameObject.SetActive(true);

            // Same Session
            if (lastClaimDate == currDate)
            {
                _claimRewardBt.interactable = false;
                return;     // Already taken the reward
            }
            _claimRewardBt.onClick.AddListener(() => HandleUIInteraction(GameUIInteraction.CLAIM_DAILY_REWARD_REQ));
        }

        private async void HandleStatusUpdate(int status)
        {
            switch ((GameStatus)status)
            {
                case GameStatus.WON:
                    await Task.Delay(GO_STATS_WAIT_TIME);                      // Wait a bit for stats to update

                    _gameOverPanel.gameObject.SetActive(true);

                    int elapsedMinutes = (int)(_gameData.LevelCompletionTime / 60f);
                    int elapsedSeconds = (int)(_gameData.LevelCompletionTime % 60f);
                    _timeElapsedTxt.text = $"{elapsedMinutes:D2}:{elapsedSeconds:D2}";

                    _coinsGainedTxt.text = LEVEL_WON_COINS_AMT.ToString();

                    break;

                case GameStatus.WORD_LIST_GEN_ERROR:

                    break;
            }
        }

        private void HandleUIInteraction(GameUIInteraction interaction, float value = 0)
        {
            switch (interaction)
            {
                case GameUIInteraction.BACK_REQ:
                case GameUIInteraction.MAIN_MENU_REQ:
                    // SceneManager.LoadScene((int)SceneIndex.MAIN_MENU);
                    SceneManager.UnloadSceneAsync((int)SceneIndex.MAIN_GAMEPLAY);
                    GameEvents.OnLoadingUpdate?.Invoke(LoadingPanelStatus.ENABLE, (int)SceneIndex.MAIN_MENU);
                    _gameData.PrevSceneIndex = SceneIndex.MAIN_GAMEPLAY;

                    break;

                case GameUIInteraction.NEXT_LEVEL_REQ:
                    break;

                case GameUIInteraction.LEADERBOARD_REQ:
                    GameEvents.OnLoadingUpdate?.Invoke(LoadingPanelStatus.ENABLE, (int)SceneIndex.LEADERBOARD);

                    break;

                case GameUIInteraction.CLAIM_DAILY_REWARD_REQ:
                    // int loginCount = PlayerPrefs.GetInt(DAILY_LOGIN_COUNT_LABEL, -1);

                    int lastClaimDate = DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day;
                    PlayerPrefs.SetInt(LAST_CLAIM_LABEL, lastClaimDate);

                    _claimRewardBt.interactable = false;

                    _dailyCoinBtCheckmarkImgs[_gameData.DailyLoginCount].gameObject.SetActive(true);
                    _gameData.DailyLoginCount++;

                    //TODO: Extra bonus?
                    if (_gameData.DailyLoginCount >= 6)        // Reset back to 0 | Bonus for 7 days login
                    {

                        _gameData.PlayerCoinCount += _dailyCoinsAmt[6];
                        _gameData.DailyLoginCount = 0;
                    }
                    else
                        _gameData.PlayerCoinCount += _dailyCoinsAmt[_gameData.DailyLoginCount];

                    PlayerPrefs.SetInt(DAILY_LOGIN_COUNT_LABEL, _gameData.DailyLoginCount);
                    PlayerPrefs.SetInt(PLAYER_COIN_COUNT_LABEL, _gameData.PlayerCoinCount);

                    break;

                case GameUIInteraction.TUTORIAL_OPEN_REQ:
                    _tutorialPanel.gameObject.SetActive(true);
                    _tutorialPlayer.Play();

                    break;

                case GameUIInteraction.TUTORIAL_CLOSE_REQ:
                    _tutorialPanel.gameObject.SetActive(false);
                    _tutorialPlayer.Stop();

                    break;
            }
        }

        private void UpdateWordList(string wordList)
        {
            _wordListTxt.text = wordList;
            GameEvents.OnLoadingUpdate?.Invoke(LoadingPanelStatus.DISABLE, 0f);
        }

        // Instantiate a new bar in the selected place
        private void SpawnHighlightBar(int wordIndex)
        {
            Image highlightBar = Instantiate(_highlightImg, _highlightContainer);
            highlightBar.color = CorrectColorsArr[wordIndex];
            _wordCorrectArr[wordIndex].gameObject.SetActive(true);

            _highlightImg.rectTransform.sizeDelta = new Vector2(0, CELL_SIZE);
        }
    }
}