using System;
using System.Collections.Generic;
using UnityEngine;

namespace FirstOverseer.Global.Other.Architecture
{
    [CreateAssetMenu(menuName = "Global/Channels/" + nameof(LockChannelSO))]
    public class LockChannelSO : ScriptableObject
    {
        private readonly HashSet<object> _lockers = new HashSet<object>();

        public event Action<bool> OnLockStateChanged;

        public bool IsLocked => _lockers.Count > 0;

        public void AddLock(object source)
        {
            if (_lockers.Add(source) && _lockers.Count == 1)
                OnLockStateChanged?.Invoke(true);
        }

        public void RemoveLock(object source)
        {
            if (_lockers.Remove(source) && _lockers.Count == 0)
                OnLockStateChanged?.Invoke(false);
        }

        private void OnDisable() => _lockers.Clear();
    }
}