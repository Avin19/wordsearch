using UnityEngine;

using Touch = UnityEngine.InputSystem.EnhancedTouch.Touch;

public class GameManager : MonoBehaviour
{
    [SerializeField] private RectTransform _highlightImg;
    private Vector2 _intialTouchPos;
    private bool _initalPosSet;
    private float _resRatio;

    private const float CANVAS_HEIGHT = 1280;

    void Start()
    {
        _resRatio = CANVAS_HEIGHT / Screen.height;

        UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
    }

    void LateUpdate()
    {
    }

    private Vector2 screenPos;          // DEBUG
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
                _highlightImg.anchoredPosition = screenPos * _resRatio;
            }
            else
            {
                float diff = screenPos.x - _intialTouchPos.x;

                Vector2 finalSize = _highlightImg.sizeDelta;
                finalSize.x = diff * _resRatio;
                _highlightImg.sizeDelta = finalSize;
            }
        }
        else
            _initalPosSet = false;
    }
}
