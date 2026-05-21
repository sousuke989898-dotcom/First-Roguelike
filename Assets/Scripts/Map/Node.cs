using UnityEngine;

public class Node
{
    public Vector2Int Pos;

    public float G; //スタートからの距離
    public float H; //ゴールまでの予測距離
    public float F => G + H; //合計コスト

    public Node parent;

    public Node(Vector2Int pos) => Pos = pos;

    public void SetCost(float g, float h)
    {
        G = g;
        H = h;
    }

}