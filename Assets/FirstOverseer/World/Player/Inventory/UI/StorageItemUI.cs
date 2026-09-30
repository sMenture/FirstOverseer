using FirstOverseer.World.Objects;
using UnityEngine;
using UnityEngine.UI;

namespace FirstOverseer.World.Player.Inventory
{
    public class StorageItemUI : MonoBehaviour
    {
        [field: SerializeField] public RectTransform RectTransform { get; private set; }

        [SerializeField] private Image _background;
        [SerializeField] private Image _icon;

        public void ApplyVisual(WorldObjectInstance instance)
        {
            var rarity = instance.Data.ItemRaritySO;

            _icon.sprite = instance.Data.Sprite;

            if (rarity != null)
                _background.color = rarity.BackgroundColor;
            else
                _background.color = Color.clear;

            var c = _icon.color;
            c.a = 1f;
            _icon.color = c;
        }

        public void ApplyDraggingVisual()
        {
            _background.color = Color.clear;

            var c = _icon.color;
            c.a = 0.3f;
            _icon.color = c;
        }

        public void RestoreVisual()
        {
            var c = _icon.color;
            c.a = 1f;
            _icon.color = c;
        }
    }
}
