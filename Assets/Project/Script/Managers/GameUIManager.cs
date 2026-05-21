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

        //          HIGHLIGHT
        private static readonly Color CorrectSelection = new Color(0f, 0.8396226f, 0.1217717f, 0.5019608f);

        void Start()
        {
            GameEvents.OnCorrectSelection += InstantiateBar;
        }

        // Instantiate a new bar in the selected place
        private void InstantiateBar()
        {
            Image highlightBar = Instantiate(_highlightImg, _highlightContainer);
            highlightBar.color = CorrectSelection;

            _highlightImg.rectTransform.sizeDelta = new Vector2(0, CELL_SIZE);
        }
    }
}