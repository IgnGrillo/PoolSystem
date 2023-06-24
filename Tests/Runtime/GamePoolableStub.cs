using System;
using Features.PoolSystem.Runtime;

namespace Features.PoolSystem.Tests.Runtime
{
    public class GamePoolableStub : IGamePoolable
    {
        public Action ReturnToPool { get; set; }
        public void EnableInstance() { }
        public void DisableInstance() { }
    }
}