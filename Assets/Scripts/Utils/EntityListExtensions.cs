using System.Collections.Generic;

public static class EntityListExtensions
{
    public static Unit GetUnit(this List<Entity> entities)
    {
        if (entities != null && entities.Count > 0)
        {
            foreach (Entity entity in entities)
            {
                if (entity is Unit unit) return unit;
            }
        }
        return null;
    }

    public static IHasStatus GetHasStatus(this List<Entity> entities)
    {
        if (entities != null && entities.Count > 0)
        {
            foreach (Entity entity in entities)
            {
                if (entity is IHasStatus hasStatus) return hasStatus;
            }
        }
        return null;
    }
}


public static class EntityHashSetExtensions //todo 削除
{
    public static Unit GetUnit(this HashSet<Entity> entities)
    {
        if (entities != null && entities.Count > 0)
        {
            foreach (Entity entity in entities)
            {
                if (entity is Unit unit) return unit;
            }
        }
        return null;
    }

    public static IHasStatus GetHasStatus(this HashSet<Entity> entities)
    {
        if (entities != null && entities.Count > 0)
        {
            foreach (Entity entity in entities)
            {
                if (entity is IHasStatus hasStatus) return hasStatus;
            }
        }
        return null;
    }
}