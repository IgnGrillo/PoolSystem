namespace Features.PoolSystem.Runtime
{
    public interface IGamePool<T> where T : IGamePoolable
    {
        T GetInstance();
        void Clear();
    }
}