namespace Game.Effect
{
    public static class EffectFactory
    {
        /// <summary>
        /// EffectDataからEffectを生成する
        /// </summary>
        public static Effect CreateEffect(EffectData data)
        {
            return data.effectType switch
            {
                EffectType.Heal   => new HealEffect(data),
                EffectType.Poison => new PoisonEffect(data),
                EffectType.AddAtk => new AddAtkEffect(data),
                _ => null
            };
        }

    }
}