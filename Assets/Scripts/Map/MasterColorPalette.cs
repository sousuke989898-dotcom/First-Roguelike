using System.Collections.Generic;
using UnityEngine;

namespace Game.GridMap
{
    [CreateAssetMenu(fileName = "MasterColorPalette", menuName = "Map/MasterColorPalette")]
    public class MasterColorPalette : ScriptableObject
    {
        [SerializeField] private List<ColorMapping> _mappings;
        public List<ColorMapping> Mappings => _mappings;
    }
}