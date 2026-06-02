using System.Collections.Generic;
using UnityEngine;

namespace Game.GridMap
{
    [CreateAssetMenu(fileName = "NewPresetRoom", menuName = "Map/PresetRoomData")]
    public class RoomData : ScriptableObject
    {
        [SerializeField] private Texture2D _roomTexture;
        public Texture2D RoomTexture => _roomTexture;

        [SerializeField] private List<ColorMapping> _colorMappings;
        public List<ColorMapping> ColorMappings => _colorMappings;
    }
}