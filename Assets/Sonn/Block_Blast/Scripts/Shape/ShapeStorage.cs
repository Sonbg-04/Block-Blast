using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Sonn.BlockBlast
{
    public class ShapeStorage : MonoBehaviour
    {
        public List<GameObject> spawnSlots;
        public GameObject shapePrefab;
        public List<ShapeData> shapeDatas;
        public List<Sprite> spriteList;

        private int m_placeCount = 0;
        private List<Shape> m_currentShapes;

        private void Awake()
        {
            m_currentShapes = new();
        }
        void Start()
        {
            SpawnShapes();
        }
        private void SpawnShapes()
        {
            m_placeCount = 0;
            m_currentShapes.Clear();

            var shuffledShapes = new List<ShapeData>(shapeDatas);

            for (int i = 0; i < shuffledShapes.Count; i++)
            {
                var temp = Random.Range(i, shuffledShapes.Count);
                (shuffledShapes[i], shuffledShapes[temp]) = (shuffledShapes[temp], shuffledShapes[i]);
            }

            var shuffledSprites = new List<Sprite>(spriteList);
            for (int i = 0; i < shuffledSprites.Count; i++)
            {
                var temp = Random.Range(i, shuffledSprites.Count);
                (shuffledSprites[i], shuffledSprites[temp]) = (shuffledSprites[temp], shuffledSprites[i]);
            }

            var count = Mathf.Min(spawnSlots.Count, shuffledShapes.Count, shuffledSprites.Count);

            for (int i = 0; i < count; i++)
            {
                var newShape = Instantiate(shapePrefab, Vector3.zero, Quaternion.identity);
                newShape.transform.SetParent(spawnSlots[i].transform, false);

                var shape = newShape.GetComponentInChildren<Shape>();
                shape.CreateShape(shuffledShapes[i]);
                m_currentShapes.Add(shape);

                var spriteRandom = shuffledSprites[i];

                var rd = newShape.GetComponentsInChildren<SpriteRenderer>();
                foreach (var r in rd)
                {
                    r.sprite = spriteRandom;
                }

                shape.HoverSprite = spriteRandom;
                shape.OnPlaced = OnShapePlaced;
            }
        }
        private void OnShapePlaced(Shape s)
        {
            m_placeCount++;
            if (m_placeCount >= spawnSlots.Count)
            {
                SpawnShapes();
            }
        }
    }
}

