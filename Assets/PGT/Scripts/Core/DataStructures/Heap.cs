using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGT.Core.DataStructures
{
    /// <summary>
    /// A simple min-binary-heap implementation
    /// </summary>
    /// <typeparam name="Priority">priority (requires ordering)</typeparam>
    /// <typeparam name="Value">value</typeparam>
    class Heap<Priority, Value> : IEnumerable<KeyValuePair<Priority,Value>> where Priority : IComparable
    {
        int count = 0;
        int capacity = 1;
        int min_capacity = 8;

        Value[] values;
        Priority[] priorities;

        public int Count
        {
            get
            {
                return count;
            }
        }

        public Heap(int capacity=8)
        {   
            count = 0;
            this.capacity = capacity < min_capacity ? min_capacity : capacity;

            values = new Value[capacity];
            priorities = new Priority[capacity];
        }

        public void Insert(Value v, Priority p)
        {
            values[count] = v;
            priorities[count] = p;

            BubbleUp(count++);

            //amortized resize
            if(count == capacity)
                resize(capacity * 2);
        }

        public KeyValuePair<Priority, Value> Peek()
        {
            return new KeyValuePair<Priority, Value>(priorities[0], values[0]);
        }

        public KeyValuePair<Priority, Value> DeleteMin()
        {
            KeyValuePair<Priority, Value> kvp = Peek();

            priorities[0] = priorities[count-1];
            values[0] = values[count - 1];
            count--;
            BubbleDown(0);

            if(capacity > min_capacity && capacity / count > 4)
            {
                resize(capacity / 2);
            }

            return kvp;
        }

        void BubbleUp(int node)
        {
            if (node == 0) return;
            int parent = Parent(node);
            if (priorities[node].CompareTo(priorities[parent]) < 0)
            {
                swap(node, parent);
                BubbleUp(parent);
            }
        }


        void BubbleDown(int node)
        {
            //leaf left child by definition is smaller than right child
            if (LeftChild(node) >= count) return;

            //find min child
            int minChild = RightChild(node);
            if (RightChild(node) >= count || priorities[LeftChild(node)].CompareTo(priorities[RightChild(node)])<0) 
                    minChild = LeftChild(node);

            if (priorities[node].CompareTo(priorities[minChild]) > 0)
            {
                swap(node, minChild);
                BubbleDown(minChild);
            }
        }


        int LeftChild(int node)
        {
            return node * 2 + 1;
        }

        int RightChild(int node)
        {
            return node * 2 + 2;
        }
        int Parent(int node)
        {
            return (node - 1) / 2;
        }

        void resize(int newSize)
        {
            Array.Resize(ref values, newSize);
            Array.Resize(ref priorities, newSize);
            capacity = newSize;
        }

        void swap(int i, int j)
        {
            Value tempv = values[i];
            values[i] = values[j];
            values[j] = tempv;

            Priority tempp = priorities[i];
            priorities[i] = priorities[j];
            priorities[j] = tempp;
        }


        public IEnumerator<KeyValuePair<Priority,Value>> GetEnumerator()
        {
            for(int i = 0; i < count; i++)
            {
                yield return new KeyValuePair<Priority, Value>(priorities[i], values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Clear(bool releaseItems = false)
        {
            count = 0;
            if (releaseItems)
            {
                for(int i=0; i<capacity; i++)
                {
                    values[i] = default(Value);
                    priorities[i] = default(Priority);
                }
            }
        }
    }
}
