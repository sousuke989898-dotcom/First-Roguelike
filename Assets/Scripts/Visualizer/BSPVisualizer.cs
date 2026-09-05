using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using Game.GridMap;
#endif

public class BspVisualizer : MonoBehaviour
{
    public static BspVisualizer Instance { get; private set; }

    [Header("表示トグル")]
    [SerializeField] private bool showSections = true;
    [SerializeField] private bool showRooms = true;
    [SerializeField] private bool showDoors = true;
    [SerializeField] private bool showTreeConnections = true;

    private SectionNode rootNode;

    private void Awake() => Instance = this;

    public void RegisterRootNode(SectionNode node) => rootNode = node;

    private void OnDrawGizmos()
    {
        if (rootNode == null) return;
        DrawNodeGizmos(rootNode, 0);
    }

    private void DrawNodeGizmos(SectionNode node, int depth)
    {
        if (node == null) return;

        if (showSections)
        {
            // --- 深さに応じた青色の計算 ---
            // 深さ 0（全体）は薄い水色、深さが増すにつれて濃い紺色に変化
            float maxExpectedDepth = 6f; // 濃さの変化基準となる最大深さ
            float t = Mathf.Clamp01(depth / maxExpectedDepth);

            Color lightBlue = new Color(0.5f, 0.8f, 1.0f); // 浅い階層の色（明るい水色）
            Color darkBlue  = new Color(0.02f, 0.1f, 0.45f); // 深い階層の色（濃い紺色）
            Color baseColor = Color.Lerp(lightBlue, darkBlue, t);

            Vector3 center = new Vector3(node.area.x + node.area.width / 2f, node.area.y + node.area.height / 2f, 0);
            Vector3 size = new Vector3(node.area.width, node.area.height, 0.01f);

            // 1. 面の描画（半透明の青）
            Gizmos.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.2f);
            Gizmos.DrawCube(center, size);

            // 2. 枠線の描画（濃い青で境界線をくっきり表示）
            Gizmos.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.9f);
            Gizmos.DrawWireCube(center, size);
        }

        // 部屋（Room）の面描画（区別しやすいよう緑色で重ねる）
        if (showRooms && node.IsLeaf && node.roomRect.width > 0)
        {
            Vector3 rCenter = new Vector3(node.roomRect.x + node.roomRect.width / 2f, node.roomRect.y + node.roomRect.height / 2f, -0.01f);
            Vector3 rSize = new Vector3(node.roomRect.width, node.roomRect.height, 0.01f);

            Gizmos.color = new Color(0f, 1f, 0.4f, 0.2f);
            Gizmos.DrawCube(rCenter, rSize);
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(rCenter, rSize);
        }

        // ドアの描画
        if (showDoors && node.IsLeaf)
        {
            Gizmos.color = Color.red;
            foreach (var door in node.doorPositions.Values)
            {
                Gizmos.DrawSphere(new Vector3(door.x + 0.5f, door.y + 0.5f, -0.02f), 0.2f);
            }
        }

        DrawNodeGizmos(node.left, depth + 1);
        DrawNodeGizmos(node.right, depth + 1);
    }
}