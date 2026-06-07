// #define DEBUG_CELL_PLACEMENT
// #define DEBUG_DRAG

using UnityEngine;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

using static WordSearch.UniversalConstants;
using UnityEngine.SceneManagement;
using System;

namespace WordSearch
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private RectTransform _highlightImg;
        [SerializeField] private RectTransform _gridContent;
        private Vector2 _intialTouchPos, _intitalCanvasPos;
        private Vector2Int _initalCellIndex, _prevCellIndex;
        private bool _initalPosSet;
        private Vector2 _resRatio;

        private int _gameStatus = (int)GameStatus.NOT_STARTED;

        // private const float CANVAS_HEIGHT = 1544;

        [Header("Game Data")]
        [SerializeField] private GameData _gameData;

        //              TEST
        [Header("Canvas")]
        [SerializeField] private RectTransform _mainCanvas;
        private float _canvasWidthOffset;

        private const int GRID_X_OFFSET = -13, GRID_Y_OFFSET = 17;
        private const float DRAG_BOUND_OFFSET = 15;
        private const int DIAG_DRAG_OFFSET = 20, TOUCH_BOUNDS_OFFSET = 5;

        private readonly Quaternion _vertAlignQuart = new Quaternion(0f, 0f, 0.7071068f, 0.7071068f);
        private readonly Quaternion _diagAlignQuart = new Quaternion(0f, 0f, -0.3826834f, 0.9238796f);
        private readonly Vector2Int _topLeftBound = new Vector2Int(-335, 247);
        private readonly Vector2Int _bottomRightBound = new Vector2Int(79, -167);

        // TOP_LEFT_INDEX: [-7, 5], BOTTOM_RIGHT_INDEX: [2, -4];

        private void OnDestroy()
        {
            GameEvents.OnGameStatusUpdate -= HandleStatusUpdate;
        }

        void Start()
        {
            _resRatio.x = _mainCanvas.sizeDelta.x / Screen.width;
            _resRatio.y = _mainCanvas.sizeDelta.y / Screen.height;

            _gameStatus |= (int)GameStatus.PLAYING;

            Initialize();

            // _resRatio.x = Screen.width / _mainCanvas.sizeDelta.x ;
            // _resRatio.y = Screen.height /_mainCanvas.sizeDelta.y ;

            _canvasWidthOffset = _mainCanvas.sizeDelta.x / 2;
            Debug.Log($"Screen Resolution: {Screen.width} | {Screen.height} | _canvasWidthOffset: {_canvasWidthOffset}\n" +
                        $" _mainCanvas Res: {_mainCanvas.sizeDelta}");

            UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();

            //              ACTIONS
            GameEvents.OnGameStatusUpdate += HandleStatusUpdate;

            // FillCirclesWhole();

#if UNITY_EDITOR
            if (PlayerPrefs.GetInt("CurrentScene", -1) != (int)SceneIndex.MAIN_MENU)
            {
                SceneManager.LoadSceneAsync((int)SceneIndex.LOADING_PANEL, LoadSceneMode.Additive);
            }
#endif
        }

        private void Initialize()
        {
            int coinCount = PlayerPrefs.GetInt(PLAYER_COIN_COUNT_LABEL, 0);
            _gameData.PlayerCoinCount = coinCount;
        }

        void LateUpdate()
        {
            HandleTouchInput();
        }

        private void HandleStatusUpdate(int status)
        {
            switch ((GameStatus)status)
            {
                case GameStatus.WON:
                    _gameData.PlayerCoinCount += LEVEL_WON_COINS_AMT;
                    PlayerPrefs.SetInt(PLAYER_COIN_COUNT_LABEL, _gameData.PlayerCoinCount);

                    break;
            }
        }

#if DEBUG_CELL_PLACEMENT
        [SerializeField] private Vector2 debugCellSize = new Vector2(45.1f, 48.2f);
        [SerializeField] private Vector2 debugCellOffset = new Vector2(19.95f, -23.5f);
        private Vector2 cellSnapPos;
        private Vector2 debugScreenPos;
        private Vector2 debugScreenOffsetPos;
        // private Vector2 screenPos;
#endif

#if DEBUG_DRAG
        Vector2 dragDiff;
        Vector2 dragPos;
        Vector2Int cellIndex;
        Vector2 highLightSize;
        // int testHorAlignedCount = 0, testDiagAlignedCount = 0, testVertAlignedCount = 0;
        [SerializeField] float testTouchYDragOffset;
#endif

        private void HandleTouchInput()
        {
            if (Touch.activeTouches.Count != 0)
            {
                Vector2 screenTouchPos = Touch.activeTouches[0].screenPosition;
                if (!_initalPosSet)
                {
                    _intialTouchPos = screenTouchPos;

                    _intitalCanvasPos = _intialTouchPos;
                    _intitalCanvasPos.x = ((_intialTouchPos.x / Screen.width) * _mainCanvas.sizeDelta.x) - (_mainCanvas.sizeDelta.x / 2);
                    // This is not opposite as the touchi goes from bottom-left to top-right
                    _intitalCanvasPos.y = Mathf.FloorToInt(((_intialTouchPos.y / Screen.height) * _mainCanvas.sizeDelta.y) - (_mainCanvas.sizeDelta.y / 2));

                    // Offset to adjust grid start pos
                    _intitalCanvasPos.x -= GRID_X_OFFSET;
                    _intitalCanvasPos.y -= GRID_Y_OFFSET;

                    _initalCellIndex.x = Mathf.FloorToInt(_intitalCanvasPos.x / CELL_SIZE);
                    _initalCellIndex.y = Mathf.CeilToInt(_intitalCanvasPos.y / CELL_SIZE);
                    _prevCellIndex = _initalCellIndex;

                    // Shift to grid pos
                    _intitalCanvasPos.x = (_initalCellIndex.x * CELL_SIZE) + GRID_X_OFFSET;
                    _intitalCanvasPos.y = (_initalCellIndex.y * CELL_SIZE) + GRID_Y_OFFSET;

                    // Bounds Check
                    if (_intitalCanvasPos.x < _topLeftBound.x || _intitalCanvasPos.x > _bottomRightBound.x ||
                        _intitalCanvasPos.y > _topLeftBound.y || _intitalCanvasPos.y < _bottomRightBound.y) return;

                    _initalPosSet = true;

                    _highlightImg.sizeDelta = new Vector2(CELL_SIZE, CELL_SIZE);
                    Vector2 cellOffset = new Vector2(CELL_SIZE / 2, CELL_SIZE / -2);
                    _highlightImg.anchoredPosition = _intitalCanvasPos;
                    _highlightImg.anchoredPosition += cellOffset;
                }
                else
                {
                    // return;         //TEST

#if !DEBUG_DRAG
                    Vector2 dragPos = _intitalCanvasPos;
                    Vector2 highLightSize;
                    Vector2Int cellIndex = Vector2Int.zero;
                    // int testHorAligned = 0;
#endif
                    // Drag position check to not allow negative values i.e. Only allowing Left->Right | Up->Down drag for now
                    // Touch position | LEFT->RIGHT: Positive | UP->DOWN: Negative
                    if ((screenTouchPos - _intialTouchPos).x < -DRAG_BOUND_OFFSET
                        || (screenTouchPos - _intialTouchPos).y > DRAG_BOUND_OFFSET) return;

                    dragPos.x = ((screenTouchPos.x / Screen.width) * _mainCanvas.sizeDelta.x) - (_mainCanvas.sizeDelta.x / 2);
                    dragPos.y = ((screenTouchPos.y / Screen.height) * _mainCanvas.sizeDelta.y) - (_mainCanvas.sizeDelta.y / 2);

                    // Offset to adjust grid start pos
                    dragPos.x -= GRID_X_OFFSET;
                    dragPos.y -= GRID_Y_OFFSET;

                    cellIndex.x = Mathf.FloorToInt(dragPos.x / CELL_SIZE);
                    cellIndex.y = Mathf.CeilToInt(dragPos.y / CELL_SIZE);

                    // Shift to grid pos
                    dragPos.x = (cellIndex.x * CELL_SIZE) + GRID_X_OFFSET;
                    dragPos.y = (cellIndex.y * CELL_SIZE) + GRID_Y_OFFSET;

                    Vector2 cellOffset = new Vector2(CELL_SIZE / 2, CELL_SIZE / 2);

                    // Drag orientation | Horizontal/Vertical | Going CC

                    //              HORIZONTAL DRAG | Check if x-difference is greater than y-difference
                    if ((screenTouchPos.x - _intialTouchPos.x) >
                        (_intialTouchPos.y - screenTouchPos.y + (DIAG_DRAG_OFFSET * (cellIndex.x - _initalCellIndex.x))))
                    {
                        // testHorAlignedCount++;
                        highLightSize.x = CELL_SIZE * (cellIndex.x - _initalCellIndex.x + 1);    // Index-offset

                        // Taking the average as the anchor is at the midddle
                        dragPos.x = (_intitalCanvasPos.x + dragPos.x) / 2;
                        dragPos.y = _intitalCanvasPos.y;
                        cellOffset.y *= -1;
                        _highlightImg.rotation = Quaternion.identity;
                    }
                    //              VERTICAL DRAG
                    else if ((screenTouchPos.x - _intialTouchPos.x + (DIAG_DRAG_OFFSET * (_initalCellIndex.y - cellIndex.y))) <
                            (_intialTouchPos.y - screenTouchPos.y))
                    {
                        // testVertAlignedCount++;
                        highLightSize.x = CELL_SIZE * (_initalCellIndex.y - cellIndex.y + 1);    // Index-offset

                        dragPos.y = (_intitalCanvasPos.y + dragPos.y) / 2;
                        dragPos.x = _intitalCanvasPos.x;
                        cellOffset.y *= -1;
                        _highlightImg.rotation = _vertAlignQuart;
                    }
                    //              DIAGONAL DRAG
                    else if ((cellIndex.x - _initalCellIndex.x) == (_initalCellIndex.y - cellIndex.y))
                    {
                        // testDiagAlignedCount++;
                        highLightSize.x = CELL_SIZE * Mathf.Abs(cellIndex.x - _initalCellIndex.x + 1);    // Index-offset
                        highLightSize.x += (CELL_SIZE / 2) * (_initalCellIndex.y - cellIndex.y);           // Extra to cover the diagonal distance

                        // Taking the average as the anchor is at the midddle | Using cell-index for more accuracy
                        dragPos.x = _intitalCanvasPos.x + ((CELL_SIZE / 2) * (_initalCellIndex.y - cellIndex.y));
                        dragPos.y = _intitalCanvasPos.y - ((CELL_SIZE / 2) * (_initalCellIndex.y - cellIndex.y));
                        cellOffset.y *= -1;
                        _highlightImg.rotation = _diagAlignQuart;
                    }
                    // Assume previous position | This is just for not making the diagonal bar freak out
                    else
                        return;

                    // return;         //TEST
                    _prevCellIndex = cellIndex;

                    highLightSize.y = CELL_SIZE;
                    _highlightImg.sizeDelta = highLightSize;
                    _highlightImg.anchoredPosition = dragPos;
                    _highlightImg.anchoredPosition += cellOffset;
                }
            }
            else
            {
                // Invoke action to check if the player has selected any word
                if (_initalPosSet)
                    GameEvents.OnDragEnded?.Invoke(_initalCellIndex, _prevCellIndex);

                _initalPosSet = false;
            }
        }
    }
}