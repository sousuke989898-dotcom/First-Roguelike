using System.Collections.Generic;
using UnityEngine;
using Game.GridMap;

public class BspVisualizer : MonoBehaviour
{
    // ★Singleton（どこからでも1行でアクセスできるようにする仕組み）
    public static BspVisualizer Instance { get; private set; }

    [Header("デバッグ表示切り替え")]
    [SerializeField] private DungeonSettings _settings;
    [SerializeField] private bool _showDebugGizmos = false;

    private List<Section> _activeSections = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            enabled = false;
            Debug.LogError($"{this}が複数存在しています。");
        }
    }

    [ContextMenu("BSP生成を実行")]
    public void RunBsp()
    {
        if (_settings == null)
        {
            Debug.LogError("DungeonSettingsがセットされていません!");
            return;
        }
        // 設定ファイルを渡して実行
        _activeSections = MapGenerator.DebugGenerateSections(_settings);
    }

    /// <summary>
    /// マップ生成器から、実際に使われたSectionのリストを受け取って記憶する
    /// </summary>
    public void RegisterSections(List<Section> sections)
    {
        _activeSections = new List<Section>(sections);
    }

    // Unityが画面を描画するときに自動で呼ばれる
    private void OnDrawGizmos()
    {
        // ★チェックボックスがオフ、またはデータがなければ何も描画しない（完全不可視）
        if (!_showDebugGizmos || _activeSections == null || _activeSections.Count == 0) return;

        foreach (var section in _activeSections)
        {
            float r = Mathf.Abs(Mathf.Sin(section.X * 12.9898f + section.Y * 78.233f)) % 1.0f;
            float g = Mathf.Abs(Mathf.Sin(section.X * 45.164f + section.Y * 98.143f)) % 1.0f;
            float b = Mathf.Abs(Mathf.Sin(section.X * 87.421f + section.Y * 12.547f)) % 1.0f;
            
            Gizmos.color = new Color(r, g, b, 0.3f);
            
            Vector3 center = new Vector3(section.X + section.Width / 2f, section.Y + section.Height / 2f, 0);
            Vector3 size = new Vector3(section.Width, section.Height, 0.1f);
            Gizmos.DrawCube(center, size);

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(center, size);
        }
    }
}