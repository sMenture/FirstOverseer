using UnityEngine;


namespace FirstOverseer.World.Player.Inventory
{
    [CreateAssetMenu(menuName = "Global/UI/" + nameof(InventoryThemeSO))]
    public class InventoryThemeSO : ScriptableObject
    {
        [field: SerializeField] public Color WarningColor { get; private set; } = Color.red;
        [field: SerializeField] public Color AccentColor { get; private set; } = Color.green;

        [field: SerializeField, Range(0f, 1f)] public float InventoryAlpha { get; private set; } = 1f;
    }
}