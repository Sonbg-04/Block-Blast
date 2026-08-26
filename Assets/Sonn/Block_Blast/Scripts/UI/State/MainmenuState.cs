namespace Sonn.BlockBlast
{
    public class MainmenuState : UIState
    {
        public override UIType UIType => UIType.Mainmenu;

        public override void Enter()
        {
            base.Enter();
            UIManager.Ins.MainmenuUI.Show();
        }
        public override void Exit()
        {
            base.Exit();
            UIManager.Ins.MainmenuUI.Hide();
        }
    }
}