using System;
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

    public event Action<Entity> OnStartMove;
    public event Action<Entity> OnEndMove;
    public event Action<Entity> OnSetPosition;


    public virtual void InitEntity(Vector2Int pos)
    {
        Pos = pos;
        OldPos = pos;
        SetTransform(pos);

        MapManager.Instance.Data.AddEntity(this); //要検討
    }


    /// <summary>
    /// 座標を設定する
    /// </summary>
    /// <param name="absPos">絶対座標</param>
    /// <returns>移動成功なら真、失敗なら偽</returns>
    protected bool SetPos(Vector2Int absPos)
    {
        if (MapManager.Instance.Data.CanMove(absPos))
        {
            OldPos = Pos;
            Pos = absPos;
            OnSetPosition?.Invoke(this); //必ず座標を変更した後に呼び出す
            return true;
        }
        return false;
    }

    /// <summary>
    /// 座標に移動する
    /// </summary>
    /// <param name="absPos"></param>
    /// <returns></returns>
    protected bool StartMove(Vector2Int absPos)
    {
        bool moved = SetPos(absPos);
        if (moved)OnStartMove?.Invoke(this);
        return moved;
    }

    protected void EndMove()
    {
        OnEndMove?.Invoke(this);
    }

    /// <summary>
    /// OffSetを適用してGameObjectを実際に動かす
    /// </summary>
    /// <param name="vector2"></param>
    protected void SetTransform(Vector2 vector2)
    {
        transform.position = new(vector2.x + PositionOffSet,vector2.y + PositionOffSet,0);
    }

    protected virtual void OnDestroy()
    {
        if (MapManager.Instance != null && MapManager.Instance.Data != null)
        {
            MapManager.Instance.Data.RemoveEntity(this);
        }

        OnSetPosition = null;
        OnStartMove = null;
        OnEndMove = null;
    }


}