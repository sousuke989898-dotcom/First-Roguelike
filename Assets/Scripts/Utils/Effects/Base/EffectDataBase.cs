namespace Game.Effect
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(menuName = "ScriptableObjects/EffectDatabase")]
    public class EffectDatabase : ScriptableObject
    {
        public List<EffectData> allEffects;

        public EffectData Get(EffectType type)
        {
            return allEffects.Find(x => x.effectType == type);
        }
    }
}