namespace Sonn.BlockBlast
{
    public class GameplayState : UIState
    {
        public override UIType UIType => UIType.Gameplay;

        public override void Enter()
        {
            base.Enter();
            UIManager.Ins.GameplayUI.Show();
        }
        public override void Exit()
        {
            base.Exit();
            UIManager.Ins.GameplayUI.Hide();
        }
    }
}

