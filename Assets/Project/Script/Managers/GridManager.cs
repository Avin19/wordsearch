// #define TEST_GRID
// #define USE_API

using System.Text;
using System.Threading.Tasks;
using System.Net;

using UnityEngine;

using WordSearch.API;
using Newtonsoft.Json;
using static WordSearch.UniversalConstants;

namespace WordSearch
{

    public class GridManager : MonoBehaviour
    {
        [SerializeField] private TMPro.TMP_Text _gridText;

        private char[][] _randWordGrid;
        private long[] _solutionArr;

        private StringBuilder _gridBuilder;

        private int _wordsFound;

        //          API
        private string[] _apiWordArr;
        private GoogleSheetResponse sheetResponse;

        //          SCRIPTS
        LevelGenerator _levelGenerator;

        private const int TOTAL_ROWS = 10;

        private const string INDENT_TAG = "<indent={0}>";
        private const char DEFAULT_CELL = '-';

        private readonly string[] _wordArr = new string[]
        {
            "SPOIL", "MEMBER", "YOU",     "STAY",  "FIXTURE", "YOU",      "TRY",    "SKATE",
            "NOISE", "COMING", "SUN",     "SITE",  "THE",     "MISPLACE", "BOLT",   "QUOTA",
            "DIE",   "LAZY",   "VET",     "WILL",  "ELF",     "FRIEND",   "ROUTE",  "TENSE",
            "GEEK",  "DEAD",   "BUT",     "ZOO",   "RISE",    "EXPRESS",  "TENSE",  "THRONE",
            "AGAIN", "ALIVE",  "DIALECT", "WITH",  "FEAR",    "SMELL",    "FORBID", "NIGHT",
            "NOW",   "BEARD",  "IRONY",   "END",   "SERIES",  "PEST",     "EXPOSE", "LITE",
            "YOUR",  "LIFE",   "HERB",    "TRUCE", "REVEAL",  "TOUT",     "HOPE",   "BRANCH",
            "ONLY",  "TAKE",   "NOT",     "PRIDE", "WHAT",    "ELEGANT",  "DIVE",   "CASH",
            "SURE",  "SYMBOL", "FOR",     "FREE",  "TESTIFY", "BERRY",    "EMPIRE", "RAISE"
        };


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

        private void OnDestroy()
        {
            GameEvents.OnDragEnded -= CheckForSelectedWord;
        }

        private void Start()
        {
            _gridBuilder = new StringBuilder();
            _levelGenerator = new LevelGenerator(GRID_SIZE);
            sheetResponse = new GoogleSheetResponse();

#if USE_API
            StartCoroutine(ApiManager.GetWordList(GetAndParseWordList));
#else
            InitializeLevel();
#endif

            GameEvents.OnDragEnded += CheckForSelectedWord;
        }

        private void GetAndParseWordList(string result, HttpStatusCode status)
        {
            sheetResponse = JsonConvert.DeserializeObject<GoogleSheetResponse>(result);
            // Debug.Log($"Word List | Status: {status} | result: \n {result}");

            int wordCount = sheetResponse.values.Count;
            _apiWordArr = new string[wordCount];

            // [IMP] Only considering the first column value
            for (int i = 0; i < wordCount; i++)
                _apiWordArr[i] = sheetResponse.values[i][0];

            // Multi-Column Values
            // [IMP] All the columns of the rows should be equal
            // int colCount = sheetResponse.values[0].Count;
            // _apiWordArr = new string[wordCount * colCount];
            // for (int i = 0; i < wordCount; i++)
            // {
            //     for (int j = 0; j < colCount; j++)
            //     {
            //         _apiWordArr[i] = sheetResponse.values[i][j];
            //     }
            // }

            InitializeLevel();
        }

        private async void InitializeLevel()
        {
            await Task.Delay(1000);             // Wait a second for everythhing to catch up

            InitializeGridWithRandom();

#if TEST_GRID
            // TestFillGrid();
            PrintSolutionArr();
            // PrintGeneratedLevel();
#endif
        }


        #region TEST
        private void TestFillGrid()
        {
            _randWordGrid = new char[TOTAL_ROWS][];
            for (int y = 0; y < TOTAL_ROWS; y++)
            {
                _randWordGrid[y] = new char[TOTAL_ROWS];
                for (int x = 0; x < TOTAL_ROWS; x++)
                    _randWordGrid[y][x] = DEFAULT_CELL;
            }

            Debug.Log($"Filling Grid");
            InitializeGridWithPreGen();
        }

        private void InitializeGridWithPreGen()
        {
            int row, col;
            for (row = 0; row < TOTAL_ROWS; row++)
            {
                _gridBuilder.AppendFormat(INDENT_TAG, 0);
                for (col = 0; col < TOTAL_ROWS - 1; col++)
                {
                    _gridBuilder.AppendFormat("{0}", _testGrid[row][col]);
                    _gridBuilder.AppendFormat(INDENT_TAG, (col + 1) * CELL_SIZE);
                }
                _gridBuilder.AppendFormat("{0}", _testGrid[row][col]);
                _gridBuilder.Append('\n');
            }
            _gridText.text = _gridBuilder.ToString();
        }
        #endregion TEST

        // public void ProcessTileClick(int x, int y, TileStatus currentStatus) { }

        private void CheckForSelectedWord(Vector2Int startIndex, Vector2Int endIndex)
        {
            startIndex.x += (GRID_START_X * -1);
            startIndex.y = (startIndex.y - GRID_START_Y) * -1;        // INDEX OFFSET

            endIndex.x += (GRID_START_X * -1);
            endIndex.y = (endIndex.y - GRID_START_Y) * -1;          // INDEX OFFSET
            // Debug.Log($"Got Drag | startIndex: {startIndex} | endIndex: {endIndex}");

            long selectedSol = 0;
            int selectedLength = 0;
            // DIAGONAL SELECTION
            if (Mathf.Abs(endIndex.x - startIndex.x) ==Mathf.Abs(startIndex.y - endIndex.y))
            {
                selectedSol |= (1 << WORD_DIAG_FLAG);
                selectedLength = endIndex.y - startIndex.y;
            }
            // VERTICAL SELECTION
            else if (endIndex.x == startIndex.x)
            {
                selectedSol |= (1 << WORD_VER_FLAG);
                selectedLength = endIndex.y - startIndex.y;
            }
            else
            {
                selectedLength = endIndex.x - startIndex.x;
            }

            // First set the row value
            selectedSol |= (1L << (startIndex.y + ROW_VAL_OFFSET + ORIENTATION_OFFSET));
            // Set the col value
            selectedSol |= (1L << (startIndex.x + ORIENTATION_OFFSET));
            // Set the length value
            selectedSol |= (1L << (selectedLength + LENGTH_VAL_OFFSET + ROW_VAL_OFFSET + ORIENTATION_OFFSET));

            for (int i = 0; i < _solutionArr.Length; i++)
            {
                if (selectedSol == _solutionArr[i])
                {
                    GameEvents.OnCorrectSelection?.Invoke(i);
                    _wordsFound++;

                    if (_wordsFound == GRID_SIZE)
                        GameEvents.OnGameStatusUpdate?.Invoke((int)GameStatus.WON);
                    // Debug.Log($"Selected Solution: {selectedSol} | selectedLength: {selectedLength} | Found at: {i}");
                }
            }
        }

        private void InitializeGridWithRandom()
        {
            int[] wordIndex;

#if USE_API
            _levelGenerator.Generate(_apiWordArr, out _randWordGrid, out _solutionArr, out wordIndex);
#else
            _levelGenerator.Generate(_wordArr, out _randWordGrid, out _solutionArr, out wordIndex);
#endif

            int row, col;
            _gridBuilder.Clear();
            for (row = 0; row < TOTAL_ROWS; row++)
            {
                _gridBuilder.AppendFormat(INDENT_TAG, 0);
                for (col = 0; col < TOTAL_ROWS - 1; col++)
                {
                    _gridBuilder.AppendFormat("{0}", _randWordGrid[row][col]);
                    _gridBuilder.AppendFormat(INDENT_TAG, (col + 1) * CELL_SIZE);
                }
                _gridBuilder.AppendFormat("{0}", _randWordGrid[row][col]);
                _gridBuilder.Append('\n');
            }
            _gridText.text = _gridBuilder.ToString();

            _gridBuilder.Clear();
            for (row = 0; row < TOTAL_ROWS; row++)
            {
                _gridBuilder.Append(_wordArr[wordIndex[row]]);
                _gridBuilder.Append('\n');
            }
            GameEvents.OnLevelGenerated?.Invoke(_gridBuilder.ToString());
        }

        private void PrintGeneratedLevel()
        {
            _gridBuilder.Clear();
            for (int r = 0; r < GRID_SIZE; r++)
            {
                for (int c = 0; c < GRID_SIZE; c++)
                {
                    _gridBuilder.Append(_randWordGrid[r][c]);
                    _gridBuilder.Append(' ');
                }
                _gridBuilder.Append('\n');
            }

            Debug.Log("Level:\n" + _gridBuilder);
        }

        private void PrintSolutionArr()
        {
            _gridBuilder.Clear();
            for (int r = 0; r < GRID_SIZE; r++)
            {
                _gridBuilder.Append(_solutionArr[r]);
                _gridBuilder.Append('\n');
            }

            Debug.Log("Solution:\n" + _gridBuilder);
        }
    }
}