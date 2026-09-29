using FirstOverseer.World.Opportunities;
using UnityEngine;

namespace FirstOverseer.World.Objects
{
    public class WorldObject : MonoBehaviour, IDamageable, IInteractable
    {
        [SerializeField] private WorldObjectSO _defaultData;
        public WorldObjectInstance Instance { get; private set; }

        public string InteractionDescriptionKey => "promt.pickup";

        public bool CanInteract => true;

        private void Awake()
        {
            if (Instance == null && _defaultData != null)
                Initialize(_defaultData.CreateInstance());
        }

        public void Initialize(WorldObjectInstance instance)
        {
            Instance = instance;
        }

        public void Interact() => Instance.Interact();

        public void TakeDamage(float value) => Instance.TakeDamage(value);

        public void Heal(float vaalue) => Instance.Heal(vaalue);


    }
}