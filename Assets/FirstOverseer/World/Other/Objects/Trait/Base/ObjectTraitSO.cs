using UnityEngine;

namespace FirstOverseer.World.Objects.Traits
{
    public abstract class ObjectTraitSO : ScriptableObject
    {
        public abstract ObjectTraitInstance CreateInstance();
    }

    public abstract class ObjectTraitInstance
    {
        public WorldObjectInstance Owner { get; private set; }
        public bool IsEnabled { get; set; } = true;

        public void Initialize(WorldObjectInstance owner)
        {
            Owner = owner;
            OnEnter();
            OnItialize();
        }

        protected virtual void OnItialize() { }

        public virtual void OnEnter() { }

        public virtual void OnExit() { }
    }
}