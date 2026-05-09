using UnityEngine;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

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

    private const int HIGHLIGHT_BASE_WIDTH = 42;

    void Start()
    {
        _resRatio.x = _mainCanvas.sizeDelta.x / Screen.width;
        _resRatio.y = _mainCanvas.sizeDelta.y / Screen.height;

        // _resRatio.x = Screen.width / _mainCanvas.sizeDelta.x ;
        // _resRatio.y = Screen.height /_mainCanvas.sizeDelta.y ;

        _canvasWidthOffset = _mainCanvas.sizeDelta.x / 2;
        Debug.Log($"Screen Resolution: {Screen.width} | {Screen.height} | {_canvasWidthOffset}");

        UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
    }

    void LateUpdate()
    {
        TestTouch();
    }

    private Vector2 screenPos;          // DEBUG
    // private Vector2 debugScreenPos, canvasPoint;
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
                canvasPoint.x += _canvasWidthOffset - (HIGHLIGHT_BASE_WIDTH / 2);
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
}
