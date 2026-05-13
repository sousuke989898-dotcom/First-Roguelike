using System.Collections;
using UnityEngine;

public class UnitAnim
{
    private Transform transform;
    private SpriteRenderer spriteRenderer;
    private float animTime = 0.1f;

    private const float OffSet = 0.5f;

    public UnitAnim(Transform transform, SpriteRenderer spriteRenderer)
    {
        this.transform = transform;
        this.spriteRenderer = spriteRenderer;
    }

    public IEnumerator MoveAnimCoroutine(Vector2Int startPos, Vector2Int endPos)
    {
        float elapsed = 0f;
        while (elapsed < animTime)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / animTime);
            transform.position = Vector2.Lerp(startPos, endPos, progress) + new Vector2(OffSet, OffSet);
            yield return null;
        }
        transform.position = endPos + new Vector2(OffSet, OffSet);
    }

    public IEnumerator AttackAnimationCoroutine(Vector2Int startPos, Vector2Int targetPos)
    {
        float elapsed = 0f;

        while (elapsed < animTime)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / animTime);
            float weight = Mathf.Sin(progress * Mathf.PI) * 0.2f; //todo マジックナンバーの調整
            transform.position = startPos + ((Vector2)(targetPos - startPos) * weight);
            yield return null;
        }

        transform.position = startPos + new Vector2(OffSet, OffSet);
    }

    public IEnumerator DieAnimationCoroutine()
    {
        float elapsed = 0f;
        Color c = spriteRenderer.color;

        while (elapsed < animTime)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / animTime);
            spriteRenderer.color = new Color(c.r, c.g, c.b, 1.0f - progress);
            yield return null;
        }
    }
}