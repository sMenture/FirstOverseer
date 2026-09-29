using UnityEngine;

namespace FirstOverseer.World.Objects
{
    [CreateAssetMenu(menuName = "World/Objects/Item Rarity")]
    public class ItemRaritySO : ScriptableObject
    {
        [field: SerializeField] public string Id { get; private set; }
        [field: SerializeField] public string NameKey { get; private set; }

        [Header("Visual")]
        [field: SerializeField] public Color BackgroundColor { get; private set; } = Color.white;
    }
}