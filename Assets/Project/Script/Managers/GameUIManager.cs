using System;

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
        }

        [SerializeField] private TMPro.TMP_Text _currMonthTxt, _currDateTxt;

        //              GRID
        [SerializeField] private Image _highlightImg;
        [SerializeField] private RectTransform _highlightContainer;

        [SerializeField] private TMPro.TMP_Text _wordListTxt;
        [SerializeField] private RectTransform[] _wordCorrectArr;

        //              START
        [SerializeField] private GameData _gameData;

        [Header("Game Over")]
        [SerializeField] private RectTransform _gameOverPanel;
        [SerializeField] private Button _nextLevelBt, _mainMenuBt, _leaderBoardBt;


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

        void Start()
        {
            // Debug.Log($"Month: {DateTime.Today.Month} | Date: {DateTime.Today.Day}");
            _currMonthTxt.text = DateTime.Now.ToString("MMM").ToUpper();
            _currDateTxt.text = DateTime.Now.Day.ToString();

            //              BUTTONS
            _mainMenuBt.onClick.AddListener(() => HandleUIInteraction(GameUIInteraction.MAIN_MENU_REQ));
            _nextLevelBt.onClick.AddListener(() => HandleUIInteraction(GameUIInteraction.NEXT_LEVEL_REQ));
            _leaderBoardBt.onClick.AddListener(() => HandleUIInteraction(GameUIInteraction.LEADERBOARD_REQ));

            //              ACTIONS
            GameEvents.OnLevelGenerated += UpdateWordList;
            GameEvents.OnCorrectSelection += SpawnHighlightBar;
            GameEvents.OnGameStatusUpdate += HandleStatusUpdate;
        }

        private void HandleStatusUpdate(int status)
        {
            switch ((GameStatus)status)
            {
                case GameStatus.WON:
                    _gameOverPanel.gameObject.SetActive(true);

                    break;
            }
        }

        private void HandleUIInteraction(GameUIInteraction interaction)
        {
            switch (interaction)
            {
                case GameUIInteraction.MAIN_MENU_REQ:
                    SceneManager.LoadScene((int)SceneIndex.MAIN_MENU);

                    break;

                case GameUIInteraction.NEXT_LEVEL_REQ:
                    break;

                case GameUIInteraction.LEADERBOARD_REQ:
                    GameEvents.OnLoadingUpdate?.Invoke(LoadingPanelStatus.ENABLE, (int)SceneIndex.LEADERBOARD);

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