namespace MiniGolf
{
    public interface IContainer<T>
    {
        protected T[] Values { get; }
        public T this[int index] => Values[index];
        public int Length => Values.Length;
    }
}