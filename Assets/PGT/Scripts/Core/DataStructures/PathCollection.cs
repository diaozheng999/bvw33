using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGT.Core.DataStructures
{
    public class PathCollection : IList<int>
    {
        int[] path;
        int count;

        public PathCollection(int capacity)
        {
            path = new int[capacity];
            count = 0;
        }
        
        public int this[int index]
        {
            get
            {
                return path[index];
            }

            set
            {
                path[index] = value;
            }
        }

        public int Count
        {
            get
            {
                return count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return true;
            }
        }

        public void Add(int item)
        {
            path[count++] = item;
        }

        public void Clear()
        {
            count = 0;
        }

        public bool Contains(int item)
        {
            for(int i=0; i<count; i++)
            {
                if (item == path[i]) return true;
            }
            return false;
        }

        public void CopyTo(int[] array, int arrayIndex)
        {
            Array.Copy(path, 0, array, arrayIndex, count);
        }

        public IEnumerator<int> GetEnumerator()
        {
            for(int i=count-1; i>=0; i++)
            {
                yield return path[i];
            }
        }

        public int IndexOf(int item)
        {
            for(int i=0; i<count; i++)
            {
                if (item == path[i]) return i;
            }
            return -1;
        }

        public void Insert(int index, int item)
        {
            for (int i = count++; i > index; i--)
            {
                path[i] = path[i-1];
            }
            path[index] = item;
        }

        public bool Remove(int item)
        {
            int i = IndexOf(item);
            if (i < 0) return false;
            RemoveAt(i);
            return true;
        }

        public void RemoveAt(int index)
        {
            if (count == 0) return;
            count--;
            for(int i=index; i<count; i++)
            {
                path[i] = path[i + 1];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}