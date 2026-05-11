using UnityEngine;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

using static WordSearch.UniversalConstants;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RectTransform _highlightImg;
    [SerializeField] private RectTransform _gridContent;
    private Vector2 _intialTouchPos, _intitalCanvasPos;
    private bool _initalPosSet;
    private Vector2 _resRatio;

    // private const float CANVAS_HEIGHT = 1544;

    //              TEST
    [SerializeField] private RectTransform _mainCanvas;
    private float _canvasWidthOffset;

    private const int HIGHLIGHT_BASE_WIDTH = 42;
    private const float HIGHLIGHT_Y_SIZE = 48.75f;
    private const float HIGHLIGHT_X_OFFSET = 10f, HIGHLIGHT_Y_OFFSET = 11.1f, HIGHLIGHT_Y_OFF_MULT = 0.7f;         // Additional Offset to cover the beginning letter correctly
    private const int START_GRID_INDEX = 11;

    private const float CELL_X_OFFSET = 19.95f, CELL_Y_OFFSET = -19.86f;
    private const float CELL_SIZE_X = 45.1f, CELL_SIZE_Y = 48.2f;

    // private static Vector2 CanvasSize => new Vector2(720, 1280);
    // private static Vector2 CellSize => new Vector2(10, 10);

    void Start()
    {
        _resRatio.x = _mainCanvas.sizeDelta.x / Screen.width;
        _resRatio.y = _mainCanvas.sizeDelta.y / Screen.height;

        // _resRatio.x = Screen.width / _mainCanvas.sizeDelta.x ;
        // _resRatio.y = Screen.height /_mainCanvas.sizeDelta.y ;

        _canvasWidthOffset = _mainCanvas.sizeDelta.x / 2;
        Debug.Log($"Screen Resolution: {Screen.width} | {Screen.height} | {_canvasWidthOffset}");

        UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();

        // FillCirclesWhole();
    }

    void LateUpdate()
    {
        // TestTouch();
        TestTouch2();
    }

    private Vector2 screenPos;          // DEBUG
    private Vector2 debugScreenPos;
    private Vector2 debugScreenOffsetPos;

    // [SerializeField] private float debugHighLightYOffset = 11.1f;
    // [SerializeField] private float debugHiYOffMult = 0.7f;              //
    // [SerializeField] private int startTextGrid = 11;
    // [SerializeField] private float debugTouchYOffset = 11;
    private void TestTouch()
    {
        if (Touch.activeTouches.Count != 0)
        {
            // Vector2 screenPos;
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
                debugScreenPos = _intitalCanvasPos;

                _intitalCanvasPos.x += _canvasWidthOffset;
                // Snap to Grid Cells
                _intitalCanvasPos.x = Mathf.Floor(_intitalCanvasPos.x / INDENT_VAL_INCREMENT) * INDENT_VAL_INCREMENT
                            - HIGHLIGHT_X_OFFSET;

#if VERTICAL_HIGHLIGHT_DEBUG
                _intitalCanvasPos.y -= debugTouchYOffset;
#else
                // _intitalCanvasPos.y -= HIGHLIGHT_Y_OFFSET;
#endif

                int yIndex = Mathf.CeilToInt(_intitalCanvasPos.y / HIGHLIGHT_Y_SIZE);             // Kill me

#if VERTICAL_HIGHLIGHT_DEBUG
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
                finalPos.x = Mathf.Floor(finalPos.x / INDENT_VAL_INCREMENT) * INDENT_VAL_INCREMENT
                            - HIGHLIGHT_X_OFFSET;
                // _highlightImg.anchoredPosition = finalPos;
                // Debug.Log($"Drag finalPos: {finalPos}");

                // Vector2 finalSize = _highlightImg.sizeDelta;
                // finalSize.x = (diff * _resRatio.x) + (HIGHLIGHT_BASE_WIDTH / 2);
                // _highlightImg.sizeDelta = finalSize;
            }
        }
        else
            _initalPosSet = false;
    }

    [SerializeField] private Vector2 debugCellSize = new Vector2(45.1f, 48.2f);
    [SerializeField] private Vector2 debugCellOffset = new Vector2(19.95f, -23.5f);
    private Vector2 debugCellSnapPos;
    private Vector2Int debugCellIndex;
    private void TestTouch2()
    {
        if (Touch.activeTouches.Count != 0)
        {
            // Vector2 screenPos;
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
                debugScreenPos = _intitalCanvasPos;

                debugScreenOffsetPos.x = debugScreenPos.x + (_gridContent.sizeDelta.x / 2) - (HIGHLIGHT_BASE_WIDTH / 2);
                debugScreenOffsetPos.y = debugScreenPos.y - (_gridContent.sizeDelta.y / 2) + (HIGHLIGHT_BASE_WIDTH / 2);

                _intitalCanvasPos = debugScreenOffsetPos;

                // Snap to Grid Cells
                debugCellSnapPos = _intitalCanvasPos;
                debugCellIndex.x = Mathf.FloorToInt(_intitalCanvasPos.x / debugCellSize.x);
                debugCellSnapPos.x = debugCellIndex.x * debugCellSize.x + debugCellOffset.x;

                debugCellIndex.y = Mathf.FloorToInt((_intitalCanvasPos.y * -1) / debugCellSize.y);      // Already know that this will go from minus to plus
                debugCellSnapPos.y = debugCellIndex.y * debugCellSize.y * -1 + debugCellOffset.y;

                // int yIndex = Mathf.CeilToInt(_intitalCanvasPos.y / HIGHLIGHT_Y_SIZE);             // Kill me

                // _intitalCanvasPos.y = yIndex * HIGHLIGHT_Y_SIZE
                //             - HIGHLIGHT_Y_OFFSET - (HIGHLIGHT_Y_OFF_MULT * HIGHLIGHT_Y_OFF_MULT
                //             * (START_GRID_INDEX + yIndex));          // Yeah this sucks

                // Debug.Log($"HighLight Y: {yIndex}");

                _highlightImg.anchoredPosition = debugCellSnapPos;
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
