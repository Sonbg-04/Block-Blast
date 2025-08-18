using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BlockBlast
{
    [CreateAssetMenu]
    [System.Serializable]
    public class ShapeData : ScriptableObject
    {
        [System.Serializable]
        public class Row
        {
            public bool[] Column;
            private int m_size = 0;
            public Row() { }
            public Row(int size)
            {
                CreateRow(size);
            }
            public void CreateRow(int size)
            {
                m_size = size;
                Column = new bool[m_size];
                ClearRow();
            }
            public void ClearRow()
            {
                for (int i = 0; i < m_size; i++)
                {
                    Column[i] = false;
                }
            }
        }

        public int Columns = 0;
        public int Rows = 0;
        public Row[] Grid;

        public void Clear()
        {
            for (int x = 0; x < Rows; x++)
            {
                Grid[x].ClearRow();
            }    
        }    
        public void CreateNewGrid()
        {
            Grid = new Row[Rows];
            for (int x = 0; x < Rows; x++)
            {
                Grid[x] = new Row(Columns);
            }    
        }    
    }
}
