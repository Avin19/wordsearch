using System;
using UnityEngine;

namespace WordSearch
{
    public static class GameEvents
    {
        public static Action<string> OnLevelGenerated;
        public static Action<Vector2Int, Vector2Int> OnDragEnded;
        public static Action<int> OnCorrectSelection;
    }
}