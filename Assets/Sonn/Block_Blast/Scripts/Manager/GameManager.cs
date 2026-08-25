using UnityEngine;

namespace Sonn.BlockBlast
{
    public class GameManager : MonoBehaviour, ISingleton
    {
        public static GameManager Ins;

        public bool IsGameOver { get; private set; }

        private void Awake()
        {
            MakeSingleton();
        }
        public void MakeSingleton()
        {
            Ins = this;
        }
        public void HandleShapePlaced(Shape shape)
        {
            GridManager.Ins.CheckAndClearLines();
            ShapeManager.Ins.OnShapePlaced(shape);
            CheckGameOver();
        }    
        private void CheckGameOver()
        {
            if (IsGameOver)
            {
                return;
            }
            if (GridManager.Ins.HasAnyValidMove(ShapeManager.Ins.CurrentShapes))
            {
                return;
            }
            IsGameOver = true;
            Debug.Log("Game over...!");
        }    
    }
}
