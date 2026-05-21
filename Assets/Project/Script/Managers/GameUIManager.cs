using System;

using UnityEngine;
using UnityEngine.UI;

using static WordSearch.UniversalConstants;

namespace WordSearch
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField] private Image _highlightImg;

        [SerializeField] private RectTransform _highlightContainer;

        [SerializeField] private TMPro.TMP_Text _wordListTxt;
        [SerializeField] private RectTransform[] _wordCorrectArr;

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
            GameEvents.OnLevelGenerated += UpdateWordList;
            GameEvents.OnCorrectSelection += InstantiateBar;
        }

        private void UpdateWordList(string wordList)
        {
            _wordListTxt.text = wordList;
        }

        // Instantiate a new bar in the selected place
        private void InstantiateBar(int wordIndex)
        {
            Image highlightBar = Instantiate(_highlightImg, _highlightContainer);
            highlightBar.color = CorrectColorsArr[wordIndex];
            _wordCorrectArr[wordIndex].gameObject.SetActive(true);

            _highlightImg.rectTransform.sizeDelta = new Vector2(0, CELL_SIZE);
        }
    }
}