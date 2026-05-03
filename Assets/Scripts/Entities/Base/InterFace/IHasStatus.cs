public interface IHasStatus
{
    Status Status{get;}
    int TakeDamage(Status attackerStatus);
}