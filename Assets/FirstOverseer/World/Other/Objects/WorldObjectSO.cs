using FirstOverseer.World.Objects.Traits;
using System;
using UnityEngine;

namespace FirstOverseer.World.Objects
{
    [CreateAssetMenu(menuName = "World/Objects/" + nameof(WorldObjectSO))]
    public class WorldObjectSO : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string NameKey { get; private set; }
        [field: SerializeField] public WorldObject Prefab { get; private set; }

        [field: Header("UI Visual")]
        [field: SerializeField] public Sprite Sprite { get; private set; }
        [field: SerializeField] public ItemRaritySO ItemRaritySO { get; private set; }

        [field:  Header("Other")]
        [field: SerializeField] public ObjectTraitSO[] Traits { get; private set; }

        public WorldObjectInstance CreateInstance()
        {
            for (int i = 0; i < Traits.Length; i++)
                if (Traits[i] is InventoryItemTraitSO)
                    return new InventoryObjectInstance(this);

            return new WorldObjectInstance(this);
        }

    }

    [Serializable]
    public class WorldObjectInstance
    {
        public readonly ObjectTraitInstance[] _traits;
        public readonly WorldObjectSO Data;

        public event Action OnInteraction;
        public event Action<float> OnHeal;
        public event Action<float> OnTakeDamage;

        public WorldObjectInstance(WorldObjectSO data)
        {
            Data = data;
            var dataTraits = Data.Traits;


            _traits = new ObjectTraitInstance[dataTraits.Length];

            for (int i = 0; i < dataTraits.Length; i++)
            {
                var trait = dataTraits[i];

                if (trait == null)
                    throw new ArgumentOutOfRangeException(nameof(ObjectTraitSO) + $" [{i}]");

                _traits[i] = trait.CreateInstance();
                _traits[i].Initialize(this);
            }
        }

        public void Initialize()
        {
            OnInitialize();

        }

        protected virtual void OnInitialize() { }

        public void Interact() => OnInteraction?.Invoke();

        public void TakeDamage(float damageAmount) => OnTakeDamage?.Invoke(damageAmount);

        public void Heal(float healAmount) => OnHeal?.Invoke(healAmount);

        public T GetTrait<T>() where T : class
        {
            if (_traits == null)
                return null;

            for (int i = 0; i < _traits.Length; i++)
                if (_traits[i] is T typedTrait)
                    return typedTrait;

            return null;
        }
    }
}