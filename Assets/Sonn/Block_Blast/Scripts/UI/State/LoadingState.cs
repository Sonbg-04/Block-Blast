namespace Sonn.BlockBlast
{
    public class LoadingState : UIState
    {
        public override UIType UIType => UIType.Loading;

        public override void Enter()
        {
            base.Enter();
            UIManager.Ins.LoadingUI.Show();
        }
        public override void Exit()
        {
            base.Exit();
            UIManager.Ins.LoadingUI.Hide();
        }
    }
}
