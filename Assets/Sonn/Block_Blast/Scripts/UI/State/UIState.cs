namespace Sonn.BlockBlast
{
    public abstract class UIState
    {
        public abstract UIType UIType { get; }
        public virtual void Enter() { }
        public virtual void Exit() { }
    }
}