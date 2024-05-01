
namespace SLOTC.Utils.StateMachine
{
    public interface IState
    {
        bool CanExit { get; set; }
        string GetID();
        void OnEnter();
        void OnUpdate(float deltaTime);
        void OnExit();
    }
}
