namespace MiniGolf
{
    public interface IArrayDisplayable<T>
    {
        protected T[] Values { get; }
        public T this[int index] => Values[index];
        public int Length => Values.Length;
    }
}