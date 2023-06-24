using System;

namespace Features.PoolSystem.Runtime
{
    public interface IGamePoolable
    {
        Action ReturnToPool { get; set; }
        void EnableInstance();
        void DisableInstance();
    }
}