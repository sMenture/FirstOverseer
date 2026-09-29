using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirstOverseer.Global.Other.Pool
{
    public class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
    {
        [SerializeField] private T _prefab;
        [SerializeField, Min(1)] private int _spawnCount = 50;

        private Queue<T> _pooledObjects = new Queue<T>();

        public int SpawnCount => _spawnCount;
        public int PoolCount => _pooledObjects.Count;
        public bool CanReturnDequeueElememt => _pooledObjects.Count > 0;

        public event Action AddElement;
        public event Action RemoveElement;

        private void Awake()
        {
            for (int i = 0; i < _spawnCount; i++)
            {
                var newPoolElement = Instantiate(_prefab, transform);
                newPoolElement.gameObject.SetActive(false);

                _pooledObjects.Enqueue(newPoolElement);
            }
        }

        public T PeekElement()
        {
            if (_pooledObjects.Count == 0)
                throw new ArgumentException("Pool is empty");

            return _pooledObjects.Peek();
        }

        public T GiveElement()
        {
            if (TryGiveElement() == false)
                throw new ArgumentException("Pool is empty");

            var firstPoolElement = _pooledObjects.Dequeue();

            firstPoolElement.gameObject.SetActive(true);

            AddElement?.Invoke();

            return firstPoolElement;
        }

        public bool TryGiveElement() => _pooledObjects.Count > 0;

        public void ReturnToPool(T element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            element.gameObject.SetActive(false);
            element.transform.SetParent(transform, false);

            _pooledObjects.Enqueue(element);

            RemoveElement?.Invoke();
        }
    }
}

