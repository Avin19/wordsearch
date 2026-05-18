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

    private const float VERTICAL_ALIGNMENT = -90f, VERT_ALIGN_QUARTERNION = 0.7071068f;

    // private static Vector2 CanvasSize => new Vector2(720, 1280);
    // private static Vector2 CellSize => new Vector2(10, 10);

    private const int GRID_X_OFFSET = -13, GRID_Y_OFFSET = 17;
    private const int CELL_INDEX_X_OFFSET = 7, CELL_INDEX_Y_OFFSET = 4;            // X: [-7 : 2] | Y: [5 : -4]
    private const int GRID_SIZE = 10;

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
    private Vector2 dragDiff;
    private Vector2 dragPos;
    private Vector2Int cellIndex;
    private Vector2 highLightSize;
#endif

    private void TestTouch2()
    {
        if (Touch.activeTouches.Count != 0)
        {
            Vector2 screenPos;
            screenPos = Touch.activeTouches[0].screenPosition;
            if (!_initalPosSet)
            {
                _initalPosSet = true;

                // Offset to the bottom as the pivot/position of image is set to middle-lef
                _intialTouchPos = screenPos;
                // screenPos.y -= (Screen.height / 2);

                // Convert Screen Point to Canvas Local Space
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _gridContent,
                    // _mainCanvas,
                    screenPos,
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
                Vector2Int cellIndex;
#else
                dragDiff = screenPos - _intialTouchPos;
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
                        _highlightImg.localRotation = new Quaternion(0, 0, -VERT_ALIGN_QUARTERNION, VERT_ALIGN_QUARTERNION);
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
            Vector2 screenPos = Touch.activeTouches[0].screenPosition;
            if (!_initalPosSet)
            {
                _initalPosSet = true;

                _intialTouchPos = screenPos;

                _intitalCanvasPos = _intialTouchPos;
                _intitalCanvasPos.x = ((_intialTouchPos.x / Screen.width) * _mainCanvas.sizeDelta.x) - (_mainCanvas.sizeDelta.x / 2);
                // This is not opposite as the touchi goes from bottom-left to top-right
                _intitalCanvasPos.y = ((_intialTouchPos.y / Screen.height) * _mainCanvas.sizeDelta.y) - (_mainCanvas.sizeDelta.y / 2);

                // Offset to adjust grid start pos
                _intitalCanvasPos.x -= GRID_X_OFFSET;
                _intitalCanvasPos.y -= GRID_Y_OFFSET;

                _prevCellIndex.x = Mathf.FloorToInt(_intitalCanvasPos.x / CELL_SIZE);
                _prevCellIndex.y = Mathf.CeilToInt(_intitalCanvasPos.y / CELL_SIZE);

                // Shift to grid pos
                _intitalCanvasPos.x = (_prevCellIndex.x * CELL_SIZE) + GRID_X_OFFSET;
                _intitalCanvasPos.y = (_prevCellIndex.y * CELL_SIZE) + GRID_Y_OFFSET;

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
                Vector2Int cellIndex;
#endif
                dragPos.x = ((screenPos.x / Screen.width) * _mainCanvas.sizeDelta.x) - (_mainCanvas.sizeDelta.x / 2);
                dragPos.y = ((screenPos.y / Screen.height) * _mainCanvas.sizeDelta.y) - (_mainCanvas.sizeDelta.y / 2);

                // Offset to adjust grid start pos
                dragPos.x -= GRID_X_OFFSET;
                dragPos.y -= GRID_Y_OFFSET;

                cellIndex.x = Mathf.FloorToInt(dragPos.x / CELL_SIZE);
                cellIndex.y = Mathf.CeilToInt(dragPos.y / CELL_SIZE);

                // Shift to grid pos
                dragPos.x = (cellIndex.x * CELL_SIZE) + GRID_X_OFFSET;
                dragPos.y = (cellIndex.y * CELL_SIZE) + GRID_Y_OFFSET;

                //          HORIZONTAL PLACEMENT
                // Offset cell-index as it goes from [-7 : 2] to [0 : 10] | Also index-offset
                highLightSize.x = CELL_SIZE * (cellIndex.x + 1 + CELL_INDEX_X_OFFSET);
                highLightSize.y = CELL_SIZE;
                _highlightImg.sizeDelta = highLightSize;

                //          VERTICAL PLACEMENT
                // Offset cell-index as it goes from [5 : -4] to [10 : 0] and then inverse | Also index-offset
                // highLightSize.x = CELL_SIZE * (GRID_SIZE - (cellIndex.y + 1 + CELL_INDEX_Y_OFFSET));
                // _highlightImg.sizeDelta = highLightSize;


                // Taking the average as the anchor is at the midddle
                //          HORIZONTAL PLACEMENT
                dragPos.x = (_intitalCanvasPos.x + dragPos.x) / 2;

                //          VERTICAL PLACEMENT
                // dragPos.x = _intitalCanvasPos.x;
                // dragPos.y = (_intitalCanvasPos.y - dragPos.y) / 2;

                _highlightImg.anchoredPosition = dragPos;
                Vector2 cellOffset = new Vector2(CELL_SIZE / 2, CELL_SIZE / -2);
                _highlightImg.anchoredPosition += cellOffset;
            }
        }
        else
            _initalPosSet = false;
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
