namespace ABM.Model
{
    public interface IMergable<T>
    {
        void Merge(T reference);
    }
}