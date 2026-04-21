using UnityEngine;

public class Node
{
    public Vector2Int Pos;

    public int G; //スタートからの距離
    public int H; //ゴールまでの予測距離
    public int F => G + H; //合計コスト

    public Node parent;

    public Node(Vector2Int pos) => Pos = pos;

    public void SetCost(int g, int h)
    {
        G = g;
        H = h;
    }

}