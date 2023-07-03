namespace BattleServer.Game
{
    public interface IBattleProgress
    {
        void Update();
        bool IsProgress();
        bool HasWork();
        void EnqueueAction<T>(T action);
    }
}
