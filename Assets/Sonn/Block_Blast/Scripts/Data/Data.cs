using System;
using System.Collections.Generic;

namespace Sonn.BlockBlast
{
    [Serializable]
    public class GameSaveData
    {
        public int score;
        public int comboCount;
        public float lastClearTime;
        public List<OccupiedCellData> occupiedCells = new();
        public List<int> currentShapeIndices = new();  
        public List<int> currentSpriteIndices = new();
    }

    [Serializable]
    public class OccupiedCellData
    {
        public int row;
        public int col;
        public int spriteIndex;
    }
}