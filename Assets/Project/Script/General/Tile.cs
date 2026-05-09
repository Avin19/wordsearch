using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace WordSearch
{

    public enum TileStatus : byte { Empty, Filled, Cross, Hint }

    public class Tile : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler
    {
        public int X { get; private set; }
        public int Y { get; private set; }
        public TileStatus CurrentStatus { get; set; }

        [SerializeField] private Color EmptyColor = Color.white;
        [SerializeField] private Color FilledColor = Color.black;
        [SerializeField] private Color CrossColor = Color.gray;
        [SerializeField] private Color HintColor = Color.yellow;
        private Image _tileImage;

        private GridManager _gridManagerRef;

        public void Initialize(int x, int y, GridManager manager)
        {
            _tileImage = GetComponent<Image>();

            X = x;
            Y = y;
            _gridManagerRef = manager;
            // CurrentStatus = TileStatus.Empty;
            UpdateStatus(TileStatus.Empty);

            gameObject.SetActive(true);
        }

        public void Reset()
        {
            CurrentStatus = TileStatus.Empty;
            _tileImage.color = EmptyColor;
        }

        public void UpdateStatus(TileStatus status)
        {
            switch (status)
            {
                case TileStatus.Empty: _tileImage.color = EmptyColor; break;
                case TileStatus.Filled: _tileImage.color = FilledColor; break;
                case TileStatus.Cross: _tileImage.color = CrossColor; break;
                case TileStatus.Hint: _tileImage.color = HintColor; return;
            }
            CurrentStatus = status;
        }

        // TODO: We can implement a circle or a square sprite assigned in the background
        // If the player started/ended the drag then semi circle or else square
        public void OnPointerDown(PointerEventData eventData)
        {
#if !UNITY_STANDALONE
            // For simplicity: Left click fills, Right click crosses
            TileStatus inputStatus = (eventData.button == PointerEventData.InputButton.Left)
                ? TileStatus.Filled : TileStatus.Cross;
            _gridManagerRef.ProcessTileClick(X, Y, inputStatus);
#else
            _gridManagerRef.ProcessTileClick(X, Y, CurrentStatus);
#endif
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
#if !UNITY_STANDALONE
            // Drag-to-fill logic (Hypercasual polish)
            if (Input.GetMouseButton(0))
                _gridManagerRef.ProcessTileClick(X, Y, TileStatus.Filled);
            else if (Input.GetMouseButton(1))
                _gridManagerRef.ProcessTileClick(X, Y, TileStatus.Cross);
#else
            if (Input.GetMouseButton(0))
                _gridManagerRef.ProcessTileClick(X, Y, CurrentStatus);
#endif
        }
    }
}