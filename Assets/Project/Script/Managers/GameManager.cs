// #define DEBUG_CELL_PLACEMENT
#define DEBUG_DRAG
// #define DEBUG_CELL_PLACEMENT_OLD

using UnityEngine;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

using static WordSearch.UniversalConstants;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RectTransform _highlightImg;
    [SerializeField] private RectTransform _gridContent;
    private Vector2 _intialTouchPos, _intitalCanvasPos;
    private Vector2Int _initalCellIndex, _prevCellIndex;
    private bool _initalPosSet;
    private Vector2 _resRatio;

    // private const float CANVAS_HEIGHT = 1544;

    //              TEST
    [SerializeField] private RectTransform _mainCanvas;
    private float _canvasWidthOffset;

    // private const float HIGHLIGHT_BASE_SIZE = 45f;
    private const float HIGHLIGHT_Y_SIZE = 48.75f;
    private const float HIGHLIGHT_X_OFFSET = 10f, HIGHLIGHT_Y_OFFSET = 11.1f, HIGHLIGHT_Y_OFF_MULT = 0.7f;         // Additional Offset to cover the beginning letter correctly
    private const int START_GRID_INDEX = 11;

    private const float CELL_OFFSET_X = 19.76f, CELL_OFFSET_Y = -21.47f;            // -21.47 for top-left | 216.03 for left-center
    private const float CELL_SIZE_X = 45.1f;
    private const float HIGHLIGHT_EX_OFFSET = 0.35f;


    // private static Vector2 CanvasSize => new Vector2(720, 1280);
    // private static Vector2 CellSize => new Vector2(10, 10);

    private const int GRID_X_OFFSET = -13, GRID_Y_OFFSET = 17;
    private const float DRAG_BOUND_OFFSET = 15;
    private const int DIAG_DRAG_OFFSET = 20, TOUCH_BOUNDS_OFFSET = 5;

    private readonly Quaternion _vertAlignQuart = new Quaternion(0f, 0f, 0.7071068f, 0.7071068f);
    private readonly Quaternion _diagAlignQuart = new Quaternion(0f, 0f, -0.3826834f, 0.9238796f);
    private readonly Vector2Int _topLeftBound = new Vector2Int(-335, 247);
    private readonly Vector2Int _bottomRightBound = new Vector2Int(79, -167);

    // TOP_LEFT_INDEX: [-7, 5], BOTTOM_RIGHT_INDEX: [2, -4];

    void Start()
    {
        _resRatio.x = _mainCanvas.sizeDelta.x / Screen.width;
        _resRatio.y = _mainCanvas.sizeDelta.y / Screen.height;

        // _resRatio.x = Screen.width / _mainCanvas.sizeDelta.x ;
        // _resRatio.y = Screen.height /_mainCanvas.sizeDelta.y ;

        _canvasWidthOffset = _mainCanvas.sizeDelta.x / 2;
        Debug.Log($"Screen Resolution: {Screen.width} | {Screen.height} | _canvasWidthOffset: {_canvasWidthOffset}\n" +
                    $" _mainCanvas Res: {_mainCanvas.sizeDelta}");

        UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();

        // FillCirclesWhole();
    }

    void LateUpdate()
    {
        // TestTouch();
        // TestTouch2();
        TestTouch3();
    }

#if DEBUG_CELL_PLACEMENT_OLD
    private Vector2 screenPos;          // DEBUG
    private Vector2 debugScreenPos;
    private Vector2 debugScreenOffsetPos;

    [SerializeField] private float debugHighLightYOffset = 11.1f;
    [SerializeField] private float debugHiYOffMult = 0.7f;              //
    [SerializeField] private int startTextGrid = 11;
    [SerializeField] private float debugTouchYOffset = 11;
#endif

    private void TestTouch()
    {
        if (Touch.activeTouches.Count != 0)
        {
#if !DEBUG_CELL_PLACEMENT_OLD
            Vector2 screenPos;
#endif
            screenPos = Touch.activeTouches[0].screenPosition;
            if (!_initalPosSet)
            {
                _initalPosSet = true;

                // Offset to the bottom as the pivot/position of image is set to middle-left
                // _screenPos.x -= (Screen.width / 2);
                _intialTouchPos = screenPos;
                screenPos.y -= (Screen.height / 2);

                // debugScreenPos = _highlightImg.anchoredPosition = screenPos;
                // _highlightImg.anchoredPosition = screenPos * _resRatio;

                // Convert Screen Point to Canvas Local Space
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _gridContent,
                    screenPos,
                    null, // Use null for Screen Space - Overlay
                    out _intitalCanvasPos
                );
                // canvasPoint.x += _canvasWidthOffset - (INDENT_VAL_INCREMENT / 2);
#if DEBUG_CELL_PLACEMENT_OLD
                debugScreenPos = _intitalCanvasPos;
#endif

                _intitalCanvasPos.x += _canvasWidthOffset;
                // Snap to Grid Cells
                _intitalCanvasPos.x = Mathf.Floor(_intitalCanvasPos.x / CELL_SIZE) * CELL_SIZE
                            - HIGHLIGHT_X_OFFSET;

#if DEBUG_CELL_PLACEMENT_OLD
                _intitalCanvasPos.y -= debugTouchYOffset;
#else
                // _intitalCanvasPos.y -= HIGHLIGHT_Y_OFFSET;
#endif

                int yIndex = Mathf.CeilToInt(_intitalCanvasPos.y / HIGHLIGHT_Y_SIZE);             // Kill me

#if DEBUG_CELL_PLACEMENT_OLD
                _intitalCanvasPos.y = yIndex * HIGHLIGHT_Y_SIZE
                            - debugHighLightYOffset - (debugHiYOffMult * debugHiYOffMult * (startTextGrid + yIndex));          // Yeah this sucks
#else
                // _intitalCanvasPos.y = yIndex * HIGHLIGHT_Y_SIZE
                //             - HIGHLIGHT_Y_OFFSET - (HIGHLIGHT_Y_OFF_MULT * HIGHLIGHT_Y_OFF_MULT
                //             * (START_GRID_INDEX + yIndex));          // Yeah this sucks
#endif

                // Debug.Log($"HighLight Y: {yIndex}");

                // This should be done to placethe highlight in center of click, but since already offsetting on top so removed
                // canvasPoint.x -= (INDENT_VAL_INCREMENT / 2);

                _highlightImg.anchoredPosition = _intitalCanvasPos;
            }
            else
            {
                float diff = screenPos.x - _intialTouchPos.x;

                Vector2 finalPos = _intitalCanvasPos;
                finalPos.x += (diff * _resRatio.x);
                finalPos.x = Mathf.Floor(finalPos.x / CELL_SIZE) * CELL_SIZE
                            - HIGHLIGHT_X_OFFSET;
                // _highlightImg.anchoredPosition = finalPos;
                // Debug.Log($"Drag finalPos: {finalPos}");

                // Vector2 finalSize = _highlightImg.sizeDelta;
                // finalSize.x = (diff * _resRatio.x) + (HIGHLIGHT_BASE_SIZE / 2);
                // _highlightImg.sizeDelta = finalSize;
            }
        }
        else
            _initalPosSet = false;
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

    private void TestTouch2()
    {
        if (Touch.activeTouches.Count != 0)
        {
            Vector2 screenTouchPos;
            screenTouchPos = Touch.activeTouches[0].screenPosition;
            if (!_initalPosSet)
            {
                _initalPosSet = true;

                // Offset to the bottom as the pivot/position of image is set to middle-lef
                _intialTouchPos = screenTouchPos;
                // screenPos.y -= (Screen.height / 2);

                // Convert Screen Point to Canvas Local Space
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _gridContent,
                    // _mainCanvas,
                    screenTouchPos,
                    null, // Use null for Screen Space - Overlay
                    out _intitalCanvasPos
                );

                // Snap to Grid Cells
#if DEBUG_CELL_PLACEMENT
                debugScreenPos = _intitalCanvasPos;

                debugScreenOffsetPos.x = debugScreenPos.x + (_gridContent.sizeDelta.x / 2) - (HIGHLIGHT_BASE_SIZE / 2);
                debugScreenOffsetPos.y = debugScreenPos.y - (_gridContent.sizeDelta.y / 2) + (HIGHLIGHT_BASE_SIZE / 2);

                _intitalCanvasPos = debugScreenOffsetPos;

                _prevCellIndex.x = Mathf.FloorToInt(_intitalCanvasPos.x / debugCellSize.x);
                cellSnapPos.x = _prevCellIndex.x * debugCellSize.x + debugCellOffset.x;

                _prevCellIndex.y = Mathf.FloorToInt((_intitalCanvasPos.y * -1) / debugCellSize.y);      // Already know that this will go from minus to plus
                cellSnapPos.y = _prevCellIndex.y * debugCellSize.y * -1 + debugCellOffset.y;
#else
                _intitalCanvasPos.x = _intitalCanvasPos.x + (_gridContent.sizeDelta.x / 2) - (CELL_SIZE_X / 2);
                _intitalCanvasPos.y = _intitalCanvasPos.y - (_gridContent.sizeDelta.y / 2) + (CELL_SIZE_X / 2);

                Vector2 cellSnapPos = _intitalCanvasPos;
                _prevCellIndex.x = Mathf.FloorToInt(_intitalCanvasPos.x / CELL_SIZE_X);
                cellSnapPos.x = _prevCellIndex.x * CELL_SIZE_X + CELL_OFFSET_X;

                _prevCellIndex.y = Mathf.FloorToInt((_intitalCanvasPos.y * -1) / CELL_SIZE_X);
                cellSnapPos.y = _prevCellIndex.y * (CELL_SIZE_X * -1) + CELL_OFFSET_Y;
#endif
                // Debug.Log($"Initial | _prevCellIndex: {_prevCellIndex}");

                _intitalCanvasPos = cellSnapPos;
                _initalCellIndex = _prevCellIndex;
                _highlightImg.anchoredPosition = _intitalCanvasPos;

                // Reset from previous selection
                _highlightImg.sizeDelta = new Vector2(CELL_SIZE_X, CELL_SIZE_X);
                _highlightImg.localRotation = Quaternion.identity;
                _highlightImg.anchoredPosition = _intitalCanvasPos;
            }
            else
            {
                // return;          //  TEST

#if !DEBUG_DRAG
                Vector2 dragDiff = screenPos - _intialTouchPos;
                Vector2 dragPos;
                Vector2Int cellIndex = Vector2Int.zero;
#else
                dragDiff = screenTouchPos - _intialTouchPos;
#endif

                dragPos = _intitalCanvasPos;
                dragPos.x += (dragDiff.x * _resRatio.x);
                cellIndex.x = Mathf.FloorToInt(dragPos.x / CELL_SIZE_X);
                dragPos.x = cellIndex.x * CELL_SIZE_X + CELL_OFFSET_X;

                dragPos.y += (dragDiff.y * _resRatio.y);
                // Offsetting range from [200:-200] to [0:-400] so as to start from the top | This is for LEFT-CENTER
                // Offsetting range from [-21:-450] to [0:-471] so as to start from the top | This is for TOP-LEFT
                dragPos.y -= CELL_OFFSET_Y;
                cellIndex.y = Mathf.CeilToInt((dragPos.y * -1) / CELL_SIZE_X);
                dragPos.y = cellIndex.y * (CELL_SIZE_X * -1) + CELL_OFFSET_Y;
                // Debug.Log($"_prevCellIndex: {_prevCellIndex} | cellIndex: {cellIndex} | dragPos: {dragPos}");

                // Check if the cell index has changed
                if (_prevCellIndex.x != cellIndex.x || _prevCellIndex.y != cellIndex.y)
                {
                    // Check whether horizontal or vertical
                    Vector2 highLightSize = _highlightImg.sizeDelta;

                    // Player is dragging vertical
                    if (_initalCellIndex.x == cellIndex.x)
                    {
                        // Debug.Log($"Vertical");
                        highLightSize.x = ((cellIndex.y - _initalCellIndex.y) * CELL_SIZE_X) + HIGHLIGHT_EX_OFFSET * (cellIndex.y + 1);

                        // Adjustment for vertical alignment
                        _highlightImg.localRotation = _vertAlignQuart;
                        _highlightImg.anchoredPosition = new Vector2(_intitalCanvasPos.x + CELL_SIZE_X, _intitalCanvasPos.y);
                    }
                    // Player is dragging horizontal
                    else
                    {
                        // Debug.Log($"Horizontal");
                        highLightSize.x = ((cellIndex.x - _initalCellIndex.x) * CELL_SIZE_X) + HIGHLIGHT_EX_OFFSET * (cellIndex.x + 1);

                        _highlightImg.localRotation = Quaternion.identity;
                        _highlightImg.anchoredPosition = _intitalCanvasPos;
                    }

                    // highLightSize.y = (cellIndex.y * CELL_SIZE_X); //+ HIGHLIGHT_EX_OFFSET * (cellIndex.y + 1);

                    highLightSize.x = Mathf.Max(highLightSize.x, CELL_SIZE_X);          // Clamp to minimum cell size
                    _highlightImg.sizeDelta = highLightSize;
                    _prevCellIndex = cellIndex;

                    // _highlightImg.anchoredPosition = dragPos;
                }
                // Debug.Log($"Drag finalPos: {dragPos}");

                // Vector2 finalSize = _highlightImg.sizeDelta;
                // finalSize.x = (diff * _resRatio.x) + (HIGHLIGHT_BASE_SIZE / 2);
                // _highlightImg.sizeDelta = finalSize;
            }
        }
        else
            _initalPosSet = false;
    }

    private void TestTouch3()
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
            _initalPosSet = false;
        }
    }

    [SerializeField] private RectTransform _testBarImg;
    [SerializeField] private RectTransform _testBarContainer;
    private void FillCirclesWhole()
    {
        Vector2 finalPos = new Vector2(0f, 0f);
        // finalPos.y = HIGHLIGHT_Y_OFFSET;
        Vector2 finalSize = _testBarImg.sizeDelta;
        for (int x = 0; x < 11; x++)
        {
            for (int y = 0; y < 15; y++)
            {
                RectTransform imgBar = Instantiate(_testBarImg, _testBarContainer);

                finalPos.y = HIGHLIGHT_Y_SIZE * -y;
                finalSize.y = HIGHLIGHT_Y_SIZE;

                imgBar.anchoredPosition = finalPos;
                imgBar.sizeDelta = finalSize;
                imgBar.gameObject.SetActive(true);
            }
            finalPos.x += HIGHLIGHT_Y_SIZE;
        }
    }
}
