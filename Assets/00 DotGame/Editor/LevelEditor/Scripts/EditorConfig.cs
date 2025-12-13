using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DotPuzzle.Editor
{
    public class EditorConfig
    {
        public static int GridSize = 20;

        public static int LevelSizeRatio = 60;

        public static int CanvasWidth = 2500;

        public static int CanvasHeight = 2000;

        public static int OffsetX = 700;

        public static int OffsetY = 700;

        public static int LevelWidth = 10;

        public static int LevelHeight = 10;

        public static int Rows = LevelHeight + OffsetY * 2;

        public static int Columns = LevelWidth + OffsetX * 2;

        public static Vector2 ConvertIndexToPosition(int col, int row)
        {
            return new Vector2((col) * GridSize + OffsetX,
                (row) * GridSize + OffsetY);
        }

        public static Vector2Int ConvertPositionToIndex(Vector2 position)
        {
            int col = Mathf.RoundToInt((position.x - GridSize / 2.0f) / GridSize) - OffsetX;
            int row = Rows - Mathf.RoundToInt((position.y - GridSize / 2.0f) / GridSize) - OffsetY - 1;

            return new Vector2Int(col, row);
        }

        public static Vector2 ConvertIndexToNodePosition(int col, int row)
        {
            return new Vector2((col + OffsetX) * GridSize + GridSize / 2.0f,
                (Rows - row - OffsetY) * GridSize - GridSize / 2.0f);
        }
    }
}

