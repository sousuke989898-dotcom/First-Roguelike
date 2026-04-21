using UnityEngine;

/// <summary>
/// 座標を持つオブジェクト
/// </summary>
public class Entity : MonoBehaviour
{
    public const float PositionOffSet = 0.5f;

    /// <summary>
    /// Entityの絶対座標
    /// </summary>
    public Vector2Int Pos {get; protected set;}
    public Vector2Int OldPos {get; protected set;}

    public virtual void InitEntity(Vector2Int pos)
    {
        Pos = pos;
        OldPos = pos;
        SetTransform(pos);

        MapManager.Instance.Data.AddEntity(this);
    }


    protected virtual void OnDestroy()
    {
        MapManager.Instance.Data.RemoveEntity(this);
    }

    /// <summary>
    /// 絶対座標で移動する
    /// </summary>
    /// <param name="worldPos">絶対座標</param>
    /// <returns>移動成功なら真、失敗なら偽</returns>
    protected virtual bool SetPos(Vector2Int worldPos)
    {
        if (MapManager.Instance.Data.CanMove(worldPos))
        {
            OldPos = Pos;
            Pos = worldPos;
            MapManager.Instance.Data.MoveEntity(this, OldPos, Pos);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 相対座標で移動する
    /// </summary>
    /// <param name="pos">移動量</param>
    /// <returns>移動成功なら真、失敗なら偽</returns>
    protected bool MovePos(Vector2Int pos) => SetPos(Pos + pos);

    /// <summary>
    /// OffSetを適用してGameObjectを実際に動かす
    /// </summary>
    /// <param name="vector2"></param>
    protected void SetTransform(Vector2 vector2)
    {
        transform.position = new(vector2.x + PositionOffSet,vector2.y + PositionOffSet,0);
    }


}