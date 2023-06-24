using System;
using Features.PoolSystem.Runtime;
using NUnit.Framework;

namespace Features.PoolSystem.Tests.Runtime
{
    public class GamePoolShould
    {
        [Test]
        public void ReturnAnInstanceOfTheDesiredTypeWhenGetInstance()
        {
            var gamePool = WhenCreatingAGamePoolWith(creationMethod: () => new GamePoolableStub());
            var instance = gamePool.GetInstance();
            ThenTypeOfInstanceIsPoolableMock(instance.GetType());
        }

        [Test]
        public void CallCreationMethodWhenCreatingNewInstance()
        {
            const int amountToInstantiate = 5;
            var creationMethodCounter = 0;
            WhenCreatingAGamePoolWith(() =>
            {
                creationMethodCounter++;
                return new GamePoolableStub();
            }, initialAmount: amountToInstantiate);
            ThenCreationMethodWasCalledWithEveryInstantiation(amountToInstantiate, creationMethodCounter);
        }

        [Test]
        public void DoNotCallCreationMethodWhenCreatingNewInstanceWithNoInitialAmount()
        {
            var creationMethodCounter = 0;
            WhenCreatingAGamePoolWith(() =>
            {
                creationMethodCounter++;
                return new GamePoolableStub();
            }, initialAmount: 0);
            ThenCreationMethodWasCalledZeroTimes(creationMethodCounter);
        }

        [Test]
        public void CallCreationMethodWhenCreatingNewInstanceOnAnEmptyGamePool()
        {
            const int amountToInstantiate = 0;
            var creationMethodCounter = 0;
            var gamePool = GivenAGamePoolWith(() =>
            {
                creationMethodCounter++;
                return new GamePoolableStub();
            }, initialAmount: amountToInstantiate);
            WhenGettingAnInstanceOfPool(gamePool);
            ThenCreationMethodWasCalledOneTime(creationMethodCounter);
        }

        [Test]
        public void CallOnCreationWhenCreatingNewInstance()
        {
            const int amountToInstantiate = 1;
            var onCreationCallbackCounter = 0;
            WhenCreatingAGamePoolWith(() => new GamePoolableStub(),
                    onCreationCallback: _ => onCreationCallbackCounter++,
                    initialAmount: amountToInstantiate);
            ThenCallOnCreationCallback(onCreationCallbackCounter);
        }

        [Test]
        public void CallRemovalMethodWhenClearingPool()
        {
            const int amountToInstantiate = 1;
            var removalMethodCounter = 0;
            var gamePool = GivenAGamePoolWith(() => new GamePoolableStub(),
                    removalMethod: _ => removalMethodCounter++,
                    initialAmount: 1);
            WhenClearingPool(gamePool);
            ThenRemovalMethodWasCalledWhenClearingPool(amountToInstantiate, removalMethodCounter);
        }

        [Test]
        public void DoNotCallRemovalMethodWhenClearingAnEmptyPool()
        {
            var removalMethodCounter = 0;
            var gamePool = GivenAGamePoolWith(() => new GamePoolableStub(),
                    removalMethod: _ => removalMethodCounter++,
                    initialAmount: 0);
            WhenClearingPool(gamePool);
            ThenRemovalMethodWasCalledZeroTimes(removalMethodCounter);
        }

        [Test]
        public void CallOnReturnCallbackWhenReturningInstanceToPool()
        {
            var removalMethodCounter = 0;
            var gamePool = GivenAGamePoolWith(() => new GamePoolableStub(),
                    onReturnCallback: _ => removalMethodCounter++);
            var instance = GivenAnInstanceOfPool(gamePool);
            WhenReturningInstanceToPool(instance);
            ThenOnReturnCallbackWasCalledWhenReturnInstanceToPool(1, removalMethodCounter);
        }

        private static GamePool<GamePoolableStub> GivenAGamePoolWith(Func<GamePoolableStub> creationMethod,
                                                                     Action<GamePoolableStub> removalMethod = null,
                                                                     Action<GamePoolableStub> onCreationCallback = null,
                                                                     Action<GamePoolableStub> onReturnCallback = null,
                                                                     int initialAmount = 10)
        {
            return new GamePool<GamePoolableStub>(creationMethod,
                    removalMethod,
                    onCreationCallback,
                    onReturnCallback,
                    initialAmount);
        }

        private static GamePoolableStub GivenAnInstanceOfPool(IGamePool<GamePoolableStub> gamePool) => 
                gamePool.GetInstance();

        private static GamePool<GamePoolableStub> WhenCreatingAGamePoolWith(Func<GamePoolableStub> creationMethod,
                                                                            Action<GamePoolableStub> removalMethod =
                                                                                    null,
                                                                            Action<GamePoolableStub>
                                                                                    onCreationCallback = null,
                                                                            Action<GamePoolableStub> onReturnCallback =
                                                                                    null,
                                                                            int initialAmount = 10)
        {
            return new GamePool<GamePoolableStub>(creationMethod,
                    removalMethod,
                    onCreationCallback,
                    onReturnCallback,
                    initialAmount);
        }

        private static void WhenGettingAnInstanceOfPool(IGamePool<GamePoolableStub> gamePool) =>
                gamePool.GetInstance();

        private static void WhenClearingPool(IGamePool<GamePoolableStub> gamePool) =>
                gamePool.Clear();

        private static void WhenReturningInstanceToPool(IGamePoolable instance) => 
                instance.ReturnToPool();

        private static void ThenTypeOfInstanceIsPoolableMock(Type instanceType) =>
                Assert.AreEqual(typeof(GamePoolableStub), instanceType);

        private static void ThenCreationMethodWasCalledWithEveryInstantiation(
                int amountToInstantiate, int creationMethodCounter) =>
                Assert.AreEqual(amountToInstantiate, creationMethodCounter);

        private static void ThenCreationMethodWasCalledZeroTimes(int creationMethodCounter) =>
                Assert.AreEqual(0, creationMethodCounter);

        private static void ThenCreationMethodWasCalledOneTime(int creationMethodCounter) =>
                Assert.AreEqual(1, creationMethodCounter);

        private static void ThenCallOnCreationCallback(int onCreationCallbackCounter) =>
                Assert.AreEqual(1, onCreationCallbackCounter);

        private static void ThenRemovalMethodWasCalledWhenClearingPool(int amountToInstantiate,
                                                                       int creationMethodCounter) =>
                Assert.AreEqual(amountToInstantiate, creationMethodCounter);
        
        private static void ThenRemovalMethodWasCalledZeroTimes(int creationMethodCounter) =>
                Assert.AreEqual(0, creationMethodCounter);
        
        private static void ThenOnReturnCallbackWasCalledWhenReturnInstanceToPool(int amountOfInstancesReturned,
                                                                       int onReturnCallbackCounter) =>
                Assert.AreEqual(amountOfInstancesReturned, onReturnCallbackCounter);
    }
}