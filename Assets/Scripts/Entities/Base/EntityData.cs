using UnityEngine;

public class EntityData : ScriptableObject
{
    public string Name;
    public Sprite Icon;

    [Header("システム設定")]
    public bool isBlockingDefault = false;
}