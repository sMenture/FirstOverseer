using System.Collections.Generic;
using UnityEngine;

namespace FirstOverseer.Global.SaveSystem
{
    public class SaveManager : MonoBehaviour
    {
        public static SaveManager Instance { get; private set; }

        private readonly HashSet<ISaveable> _registeredSaveables = new HashSet<ISaveable>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void Register(ISaveable saveable)
        {
            if (saveable != null && !string.IsNullOrEmpty(saveable.UniqueId))
                _registeredSaveables.Add(saveable);
        }

        public void Unregister(ISaveable saveable)
        {
            if (saveable != null)
                _registeredSaveables.Remove(saveable);
        }

        public void SaveGame()
        {
            foreach (var saveable in _registeredSaveables)
            {
                string id = saveable.UniqueId;
                object data = saveable.GetSaveData();
            }
        }
    }
}