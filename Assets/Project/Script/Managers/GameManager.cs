using UnityEngine;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

using static WordSearch.UniversalConstants;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RectTransform _highlightImg;
    private Vector2 _intialTouchPos;
    private bool _initalPosSet;
    private Vector2 _resRatio;

    // private const float CANVAS_HEIGHT = 1544;

    //              TEST
    [SerializeField] private RectTransform _mainCanvas;
    private float _canvasWidthOffset;

    private const int HIGHLIGHT_BASE_WIDTH = 45;
    private const float HIGHLIGHT_Y_SIZE = 48.75f;
    private const float HIGHLIGHT_X_OFFSET = 10f, HIGHLIGHT_Y_OFFSET = 11.1f, HIGHLIGHT_Y_OFF_MULT = 0.7f;         // Additional Offset to cover the beginning letter correctly
    private const int START_GRID_INDEX = 11;

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

        FillVerticalBars();
    }

    void LateUpdate()
    {
        TestTouch();
    }

    private Vector2 screenPos;          // DEBUG
    private Vector2 debugScreenPos, canvasPoint;

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

                Vector2 canvasPoint;
                // Convert Screen Point to Canvas Local Space
                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    _mainCanvas,
                    screenPos,
                    null, // Use null for Screen Space - Overlay
                    out canvasPoint
                );
                // canvasPoint.x += _canvasWidthOffset - (INDENT_VAL_INCREMENT / 2);
                debugScreenPos = canvasPoint;

                canvasPoint.x += _canvasWidthOffset;
                // Snap to Grid Cells
                canvasPoint.x = Mathf.Floor(canvasPoint.x / INDENT_VAL_INCREMENT) * INDENT_VAL_INCREMENT
                            - HIGHLIGHT_X_OFFSET;

#if VERTICAL_HIGHLIGHT_DEBUG
                canvasPoint.y -= debugTouchYOffset;
#else
                canvasPoint.y -= HIGHLIGHT_Y_OFFSET;
#endif

                int yIndex = Mathf.CeilToInt(canvasPoint.y / HIGHLIGHT_Y_SIZE);             // Kill me

#if VERTICAL_HIGHLIGHT_DEBUG
                canvasPoint.y = yIndex * HIGHLIGHT_Y_SIZE
                            - debugHighLightYOffset - (debugHiYOffMult * debugHiYOffMult * (startTextGrid + yIndex));          // Yeah this sucks
#else
                canvasPoint.y = yIndex * HIGHLIGHT_Y_SIZE
                            - HIGHLIGHT_Y_OFFSET - (HIGHLIGHT_Y_OFF_MULT * HIGHLIGHT_Y_OFF_MULT
                            * (START_GRID_INDEX + yIndex));          // Yeah this sucks
#endif

                Debug.Log($"HighLight Y: {yIndex}");

                // This should be done to placethe highlight in center of click, but since already offsetting on top so removed
                // canvasPoint.x -= (INDENT_VAL_INCREMENT / 2);

                _highlightImg.anchoredPosition = canvasPoint;
            }
            else
            {
                float diff = screenPos.x - _intialTouchPos.x;

                Vector2 finalSize = _highlightImg.sizeDelta;
                finalSize.x = (diff * _resRatio.x) + (HIGHLIGHT_BASE_WIDTH / 2);
                _highlightImg.sizeDelta = finalSize;
            }
        }
        else
            _initalPosSet = false;
    }

    [SerializeField] private RectTransform _testBarImg;
    [SerializeField] private RectTransform _testBarContainer;
    private void FillVerticalBars()
    {
        Vector2 finalPos = Vector2.zero;
        // finalPos.y = HIGHLIGHT_Y_OFFSET;
        Vector2 finalSize = _testBarImg.sizeDelta;
        for (int i = 0; i < 26; i++)
        {
            RectTransform imgBar = Instantiate(_testBarImg, _testBarContainer);

            finalPos.y -= HIGHLIGHT_Y_SIZE;
            finalSize.y = HIGHLIGHT_Y_SIZE;

            imgBar.anchoredPosition = finalPos;
            imgBar.sizeDelta = finalSize;
            imgBar.gameObject.SetActive(true);
        }
    }
}
