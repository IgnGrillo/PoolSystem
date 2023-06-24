# Crispy Pool System

*Simple implementation of a Pooling system in Unity.* 

**Overview**

This Implementation is heavily inspired by two Unity classes: ObjectPool and GenericPool, while also adding some Factory pattern functionalities to facilitate the creation of instances.

You can start using the Pool System as soon as you create a class that inherits from **IGamePoolable** and create an instance of a **GamePool** of that Type.

**Implementation Example**

We will be declaring a class that acts as the general manager of all the pools in the game.

First, we create a new class that inherits from IGamePoolable, as the Pool will only work with IGamePoolable objects.

    public class Guard : IGamePoolable
    {
        public Action ReturnToPool { get; set; }
		
        public void EnableInstance()
        {
            //Logic
        }

        public void DisableObject()
        {
            //Logic
        }
    }
	
If you find yourself needing to implement a Pool of GameObjects, you can create a new class that Implements IGamePoolable and Inherits from MonoBehaviour at the same time.
	
	public abstract class MonoGamePoolable : MonoBehaviour, IGamePoolable
    {
        public Action ReturnToPool { get; set; }
        
        public virtual void EnableInstance() => gameObject.SetActive(true);
        public virtual void DisableInstance() => gameObject.SetActive(false);
    }

Secondly, we create the Interface IPoolRepresentation. This will be implemented by the class GuardPoolRepresentation, thus creating a class that represents a pool of Type T.
	
	public interface IPoolRepresentation
	{
		public bool IsOfType(Type type);
		public IGamePoolable GetItemFromPool();
	}
	
	public class GuardPoolRepresentation : IPoolRepresentation
    {
        private readonly Guard _guardInstance;
        private readonly GamePool<Guard> _pool;
        private readonly Type _guardType;

        public GuardPoolRepresentation(Guard guardInstance)
        {
            _guardInstance = guardInstance;
            _pool = new GamePool<Guard>(CreationMethod, DestroyMethod,
                    OnCreationMethod, OnReturnMethod,
                    initialAmount: 5);
            _guardType = typeof(Guard);
        }

        public bool IsOfType(Type type) =>
                _guardType == type;

        private Guard CreationMethod() =>
                Object.Instantiate(_guardInstance);

        private void DestroyMethod(Guard obj) =>
                Object.Destroy(obj);

        private void OnCreationMethod(Guard obj) =>
                obj.EnableInstance();

        private void OnReturnMethod(Guard obj) =>
                obj.DisableObject();

        public IGamePoolable GetItemFromPool() =>
                _pool.GetInstance();
    }
	
Now we finish by creating the PoolManager class, thus creating a quick and easy way to access or return an instance of any given pool
	
	public class PoolManager : MonoBehaviour
	{
		[SerializeField] private Guard _guardInstance;
		
		private List<IPoolRepresentation> _pools;

		private void Start() =>
				InitializePools();

		private void InitializePools() =>
				_pools = new List<IPoolRepresentation> { new GuardPoolRepresentation(_guardInstance)};

		public T GetInstanceOf<T>() where T : class =>
				GetPoolRepresentation<T>().GetItemFromPool() as T;

		private IPoolRepresentation GetPoolRepresentation<T>() where T : class =>
				_pools.First(item => item.IsOfType(typeof(T)));
	}