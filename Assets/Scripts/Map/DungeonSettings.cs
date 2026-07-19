using System.Collections.Generic;
using Game.GridMap;
using UnityEngine;

[CreateAssetMenu(fileName = "NewDungeonSettings", menuName = "Dungeon/Settings")]
public class DungeonSettings : ScriptableObject
{
    [Header("基本サイズ")]
    public int mapSizeX = 60;
    public int mapSizeY = 40;

    [Header("BSP分割設定")]
    public int roomCount = 6;
    public int minSectionSize = 6;
    public bool useOverlap = false;

    [Header("分割位置の制限")]
    [Range(0.1f, 0.45f)] public float partingMargin = 0.4f;

    [Header("部屋の設定")]
    public RoomDataset roomDataset;
}