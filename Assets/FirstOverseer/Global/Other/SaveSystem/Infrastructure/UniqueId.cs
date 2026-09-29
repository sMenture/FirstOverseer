using System;
using UnityEngine;

namespace FirstOverseer.Global.SaveSystem
{
    public class UniqueId : MonoBehaviour
    {
        [SerializeField] private string _id = Guid.NewGuid().ToString();
        public string Id => _id;

        [ContextMenu("Generate New ID")]
        private void GenerateId() => _id = Guid.NewGuid().ToString();
    }
}