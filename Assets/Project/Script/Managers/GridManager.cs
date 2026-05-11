#define TEST_GRID

using System;
using System.Text;

using UnityEngine;
using static WordSearch.UniversalConstants;

namespace WordSearch
{

    public class GridManager : MonoBehaviour
    {
        [SerializeField] private RectTransform _highlightImg;
        [SerializeField] private TMPro.TMP_Text _gridText;

        private string[] _wordArr = new string[TOTAL_ROWS];

        private StringBuilder _gridBuilder;

        private const int TOTAL_ROWS = 10;

        private const string INDENT_TAG = "<indent={0}>";

        //              HIGHLIGHT
        private const int HIGHLIGHT_BASE_WIDTH = 42;

        //              TEST
        private readonly string[] _testGrid = new string[]
        {
            "ELEPHANTZO",
            "RQWRCABDYE",
            "BUTTERFLYB",
            "PANDAFGHJQ",
            "OCEANSKLAH",
            "AEOGFZEBRA",
            "GIRAFFEASF",
            "DOLPHINPQM",
            "DOJKENLKEM",
            "MOUNTAINJR",
        };
        // private readonly string INDENT_TAG = $"<{INDENT_LABEL}={INDENT_VAL_INCREMENT}>";

        private void Start()
        {
            _gridBuilder = new StringBuilder();

#if TEST_GRID
            TestFillGrid();
#endif
        }

        #region TEST
        private void TestFillGrid()
        {
            for (int i = 0; i < TOTAL_ROWS; i++)
                _wordArr[i] = _testGrid[i];

            Debug.Log($"Filling Grid");
            FillGrid();
        }
        #endregion TEST

        private void FillGrid()
        {
            int row, col;
            for (row = 0; row < TOTAL_ROWS; row++)
            {
                _gridBuilder.AppendFormat(INDENT_TAG, 0);
                for (col = 0; col < TOTAL_ROWS - 1; col++)
                {
                    _gridBuilder.AppendFormat("{0}", _wordArr[row][col]);
                    _gridBuilder.AppendFormat(INDENT_TAG, (col + 1) * INDENT_VAL_INCREMENT);
                }
                _gridBuilder.AppendFormat("{0}", _wordArr[row][col]);
                _gridBuilder.Append('\n');
            }
            _gridText.text = _gridBuilder.ToString();
        }

        public void ProcessTileClick(int x, int y, TileStatus currentStatus)
        {
        }
    }
}