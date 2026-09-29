using UnityEngine;
using UnityEngine.UI;

namespace FirstOverseer.World.Player.Inventory
{
    public class StoragePlacementPreviewUI : MonoBehaviour
    {
        [SerializeField] private RectTransform _previewRect;
        [SerializeField] private Image _previewImage;

        private void Awake() => Disable();

        public void Enable() => _previewRect.gameObject.SetActive(true);
        public void Disable() => _previewRect.gameObject.SetActive(false);

        public void SetColor(Color color) => _previewImage.color  = color;

        public void SetSize(Vector2 size) => _previewRect.sizeDelta = size;
        public void ShowPreview(Vector3 position, bool canPlace) => _previewRect.position = position;
    }
}
