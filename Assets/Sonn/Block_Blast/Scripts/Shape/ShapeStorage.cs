using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BlockBlast
{
    public class ShapeStorage : MonoBehaviour
    {
        public List<Shape> shapeList;
        public List<ShapeData> shapeDatas;

        void Start()
        {
            foreach (var shape in shapeList)
            {
                var shapeIndex = Random.Range(0, shapeDatas.Count);
                shape.CreateShape(shapeDatas[shapeIndex]);
            }
        }

    }
}

