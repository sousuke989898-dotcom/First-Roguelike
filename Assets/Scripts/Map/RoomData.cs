using UnityEngine;

namespace Game.GridMap
{
    [CreateAssetMenu(fileName = "NewPresetRoom", menuName = "Map/PresetRoomData")]
    public class RoomData : ScriptableObject
    {
        [SerializeField] private Texture2D _roomTexture;
        public Texture2D RoomTexture => _roomTexture;

        [SerializeField] private MasterColorPalette _palette;
        public MasterColorPalette Palette => _palette;
    }
}