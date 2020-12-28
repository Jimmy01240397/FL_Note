using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FL_Note
{
	public class MyList<T> : List<T>
	{
        public event Action<T, int> OnAdd;
        public event Action<T, int> OnRemove;
        public new void Add(T item)
        {
            base.Add(item);
            OnAdd?.Invoke(item, this.Count - 1);
        }

        public new void Remove(T item)
        {
            int data = IndexOf(item);
            base.RemoveAt(data);
            OnRemove?.Invoke(item, data);
        }

        public new void RemoveAt(int index)
        {
            T data = this[index];
            base.RemoveAt(index);
            OnRemove?.Invoke(data, index);
        }
    }
}