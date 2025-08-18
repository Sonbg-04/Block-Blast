using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Sonn.BlockBlast
{
    [CustomEditor(typeof(ShapeData), false)]
    [CanEditMultipleObjects]
    [System.Serializable]
    public class ShapeDataDrawer : Editor
    {
        private ShapeData ShapeDataIns => target as ShapeData;

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            ClearGridBtn();
            EditorGUILayout.Space();

            DrawColumnsInputFields();
            EditorGUILayout.Space();

            if (ShapeDataIns.Grid != null &&
                ShapeDataIns.Rows > 0 &&
                ShapeDataIns.Columns > 0)
            {
                DrawGridTable();
            }    

            serializedObject.ApplyModifiedProperties();
            
            if (GUI.changed)
            {
                EditorUtility.SetDirty(ShapeDataIns);
            }
        }
        private void ClearGridBtn()
        {
            if (GUILayout.Button("Clear Grid"))
            {
                ShapeDataIns.Clear();
            }    
        }
        private void DrawColumnsInputFields()
        {
            var columsTemp = ShapeDataIns.Columns;
            var rowsTemp = ShapeDataIns.Rows;

            ShapeDataIns.Columns = EditorGUILayout.IntField("Columns", columsTemp);
            ShapeDataIns.Rows = EditorGUILayout.IntField("Rows", rowsTemp);

            if ((ShapeDataIns.Columns != columsTemp || ShapeDataIns.Rows != rowsTemp) &&
                ShapeDataIns.Columns > 0 && ShapeDataIns.Rows > 0)
            {
                ShapeDataIns.CreateNewGrid();
            }



        }
        private void DrawGridTable()
        {
            var tableStyle = new GUIStyle("box")
            {
                padding = new RectOffset(10, 10, 10, 10)
            };
            tableStyle.margin.left = 32;

            var headerColumnStyle = new GUIStyle
            {
                fixedWidth = 65,
                alignment = TextAnchor.MiddleCenter
            };

            var headerRowStyle = new GUIStyle
            {
                fixedHeight = 25,
                alignment = TextAnchor.MiddleCenter
            };

            var dataFieldStyle = new GUIStyle(EditorStyles.miniButtonMid);
            dataFieldStyle.normal.background = Texture2D.grayTexture;
            dataFieldStyle.onNormal.background = Texture2D.whiteTexture;

            for (int x = 0; x < ShapeDataIns.Rows; x++)
            {
                EditorGUILayout.BeginHorizontal(headerColumnStyle);
                for (int y = 0;  y < ShapeDataIns.Columns; y++)
                {
                    EditorGUILayout.BeginHorizontal(headerRowStyle);
                    var checkShapeData = ShapeDataIns.Grid[x].Column[y];
                    var data  = EditorGUILayout.Toggle(checkShapeData, dataFieldStyle);
                    
                    ShapeDataIns.Grid[x].Column[y] = data;
                    EditorGUILayout.EndHorizontal();
                }    
                EditorGUILayout.EndHorizontal();
            }
        }
    }
}
