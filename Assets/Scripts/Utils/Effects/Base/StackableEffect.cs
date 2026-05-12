namespace Game.Effect
{
    public interface IStackableEffect
    {
        public int Stack {get;}
        public int MaxStack {get;}

        public void AddStack(int stack);
    }

    public interface IModEffect
    {
        public RangeModifier Modifier {get;}
        public void OnApply(Status status);
        public void OnRemove(Status status);
    }

    public interface ITicableEffect
    {
        public int Duration {get;}


        public bool Tick(Status status);
    }

}