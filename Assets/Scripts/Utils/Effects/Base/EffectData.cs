namespace Game.Effect
{
    using System.Collections.Generic;
    using UnityEngine;
    [CreateAssetMenu(menuName = "ScriptableObjects/EffectData")]
    public class EffectData : ScriptableObject
    {
        // 共通
        public EffectType effectType;
        public int defaultDuration = 5;
        public int defaultStack = 1;
        public int defaultMaxStack = 10;

        // ModEffect用
        public IntRange baseRange;
        public ModifierType modType;

        // 固定値のEffect用
        public float Power = 0.01f;

        //UI
        public Sprite icon;


        public string effectName;

        public string description;

    }

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