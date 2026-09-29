using FirstOverseer.Global.Other.Architecture;
using FirstOverseer.Global.Player.Input;
using FirstOverseer.World.Equipment;
using FirstOverseer.World.Inventory;
using FirstOverseer.World.Objects;
using FirstOverseer.World.Objects.Traits.Equipment.Ability;
using System.Collections.Generic;
using UnityEngine;

namespace FirstOverseer.World.Player.Inventory
{
    public class PlayerInventory : MonoBehaviour
    {
        [SerializeField] private EquipmentManager _equipmentManager;
        [SerializeField] private InputReader _inputReader;
        [SerializeField] private StorageUI _storageUI;
        [SerializeField] private LockGroupSO _UILock;


        [Header("Base Inventory Settings")]
        [SerializeField] private Vector2Int _baseInventorySize = new Vector2Int(3, 4);
        [field: SerializeField] public string StorageNameKey { get; private set; } = "base_storage";

        public StorageContainer BaseStorage { get; private set; }

        public bool IsOpen { get; private set; }


        private void Awake() => BaseStorage = new StorageContainer(StorageNameKey, _baseInventorySize.x, _baseInventorySize.y);

        private void Start() => CloseInventory();

        private void OnEnable() => _inputReader.Inventory += HandleIventorySwitch;

        private void OnDisable() => _inputReader.Inventory -= HandleIventorySwitch;

        private void HandleIventorySwitch()
        {
            IsOpen = !IsOpen;

            if (IsOpen)
            {
                _UILock.ApplyAll(this);

                OpenInventory();
            }
            else
            {
                _UILock.ReleaseAll(this);

                CloseInventory();
            }
        }

        public void OpenInventory()
        {
            IsOpen = true;

            if (_storageUI == null)
                return;

            List<StorageContainer> activeStorages = GetAllActiveStorages();
            _storageUI.RenderStorages(activeStorages);
        }

        public void CloseInventory()
        {
            IsOpen = false;

            if (_storageUI == null)
                return;

            _storageUI.Hide();
        }

        public List<StorageContainer> GetAllActiveStorages()
        {
            List<StorageContainer> storages = new List<StorageContainer>();
            storages.Add(BaseStorage);

            if (_equipmentManager != null)
                foreach (var pair in _equipmentManager.EquippedTraits)
                {
                    var equipmentTrait = pair.Value;

                    foreach (var ability in equipmentTrait.Abilities)
                        if (ability is StorageAbilityInstance storageAbility)
                            if (storageAbility.Container != null)
                                storages.Add(storageAbility.Container);
                }

            return storages;
        }


        public bool TryTransferItem(InventoryObjectInstance item, StorageContainer source, StorageContainer destination, int destX, int destY)
        {
            if (destination.CanPlaceAt(item, destX, destY) == false)
                return false;

            source.RemoveItem(item);
            destination.PlaceItem(item, destX, destY);
            return true;
        }
    }
}