using System;
using System.Collections.Generic;
using System.Linq;

namespace Features.PoolSystem.Runtime
{
    public class GamePool<T> : IGamePool<T> where T : class, IGamePoolable
    {
        private readonly Func<T> _creationMethod;
        private readonly Action<T> _removalMethod;
        private readonly Action<T> _onCreationCallback;
        private readonly Action<T> _onReturnCallback;
        private readonly List<T> _openInstances;
        private readonly List<T> _closedInstances;

        public GamePool(Func<T> creationMethod, Action<T> removalMethod = null,
                        Action<T> onCreationCallback = null, Action<T> onReturnCallback = null,
                        int initialAmount = 10)
        {
            _creationMethod = creationMethod;
            _removalMethod = removalMethod;
            _onCreationCallback = onCreationCallback;
            _onReturnCallback = onReturnCallback;
            _openInstances = new List<T>();
            _closedInstances = new List<T>();
            InstantiateInitialInstances(initialAmount);
        }

        public T GetInstance()
        {
            if (_openInstances.Count == 0)
                AddNewInstance();

            return GetFirstInstanceFromOpen();
        }

        public void Clear()
        {
            foreach (var instance in _openInstances.Concat(_closedInstances))
                _removalMethod?.Invoke(instance);

            _openInstances.Clear();
            _closedInstances.Clear();
        }

        private void InstantiateInitialInstances(int initialAmount)
        {
            for (var i = 0; i < initialAmount; i++)
                AddNewInstance();
        }

        private void AddNewInstance()
        {
            var gamePoolable = _creationMethod.Invoke();
            gamePoolable.ReturnToPool += () => ReturnInstanceToPool(gamePoolable);
            _openInstances.Add(gamePoolable);
            _onCreationCallback?.Invoke(gamePoolable);
        }

        private void ReturnInstanceToPool(IGamePoolable instanceToReturn)
        {
            var gamePoolable = instanceToReturn as T;
            ResetReturnToPoolSubscriptions(instanceToReturn);
            _closedInstances.Remove(gamePoolable);
            _openInstances.Add(gamePoolable);
            _onReturnCallback?.Invoke(gamePoolable);
        }

        private void ResetReturnToPoolSubscriptions(IGamePoolable instanceToReturn)
        {
            instanceToReturn.ReturnToPool = null;
            instanceToReturn.ReturnToPool += () => ReturnInstanceToPool(instanceToReturn);
        }

        private T GetFirstInstanceFromOpen()
        {
            var first = _openInstances.First();
            _openInstances.Remove(first);
            _closedInstances.Add(first);
            return first;
        }
    }
}