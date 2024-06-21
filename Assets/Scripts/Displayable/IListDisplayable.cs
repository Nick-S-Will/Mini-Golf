using System.Collections.Generic;

namespace Displayable
{
    public interface IListDisplayable<T>
    {
        protected IList<T> Values { get; }
        public T this[int index] => Values[index];
        public int Length => Values.Count;
    }
}