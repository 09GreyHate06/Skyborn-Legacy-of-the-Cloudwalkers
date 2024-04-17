
namespace SLOTC.Utils.StateMachine
{
    public interface IState
    {
        string GetID();
        void OnEnter();
        void OnUpdate(float deltaTime);
        void OnExit();
    }
}
