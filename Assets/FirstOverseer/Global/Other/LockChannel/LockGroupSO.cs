using UnityEngine;

namespace FirstOverseer.Global.Other.Architecture
{
    [CreateAssetMenu(menuName = "Global/Channels/" + nameof(LockGroupSO))]
    public class LockGroupSO : ScriptableObject
    {
        [SerializeField] private LockChannelSO[] _lockChannels;

        public void ApplyAll(object source)
        {
            if (_lockChannels == null)
                return;

            for (int i = 0; i < _lockChannels.Length; i++)
                if (_lockChannels[i] != null)
                    _lockChannels[i].AddLock(source);
        }

        public void ReleaseAll(object source)
        {
            if (_lockChannels == null)
                return;

            for (int i = 0; i < _lockChannels.Length; i++)
                if (_lockChannels[i] != null)
                    _lockChannels[i].RemoveLock(source);
        }
    }
}