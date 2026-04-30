public class Modifier
{
    public int Duration;

    public Modifier( int Duration)
    {
        this.Duration = Duration;
    }

    public bool Tick()
    {
        if (Duration > 0) Duration--;
        return Duration == 0;
    }
}