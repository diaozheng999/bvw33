using System;
using System.Collections.Generic;
using UnityEngine;
using PGT.Core.Func;


namespace PGT.Core.DataStructures
{
    public class TDTreeNode<T>
    {
        public Vector3 coords;
        public T value;
        public TDTreeNode<T> left;
        public TDTreeNode<T> right;
        public TDTreeNode<T> parent;

        public string ToStringFormatted(int tabIndex)
        {
            return (new string(' ', tabIndex))+"coord="+coords.ToString()+", value="+value.ToString()+"\n"+
                (new string(' ', tabIndex))+" left: " +
                    (left == null ? "<null>" : ("\n" + left.ToStringFormatted(tabIndex+6))) +
                    "\n"+(new string(' ', tabIndex))+" right: "+
                    (right == null ? "<null>" : ("\n" + right.ToStringFormatted(tabIndex+6))) + "\n";
        }
    }

    public class TDTree<T>
    {
        TDTreeNode<T> root;
        public int Count;
        public TDTree(Sequence<Func.Tuple<Vector3, T>> points)
        {
            Count = 0;
            root = buildTree(points, 0);
        }

        public TDTreeNode<T> buildTree(Sequence<Func.Tuple<Vector3, T>> points,  int dim)
        {
            if (points.Count == 0)
            {
                return null;
            }
            var node = new TDTreeNode<T>();
            Count++;

            float median;
            Func<Func.Tuple<Vector3, T>, int> splitFn = null;

            switch (dim)
            {
                case 0:
                    median = Sequence.Median(Sequence.Unzip(points).car.MapEagerly((Vector3 v) => v.x));
                    splitFn = (Func.Tuple<Vector3, T> point) => point.car.x.CompareTo(median);
                    break;

                case 1:
                    median = Sequence.Median(Sequence.Unzip(points).car.MapEagerly((Vector3 v) => v.y));
                    splitFn = (Func.Tuple<Vector3, T> point) => point.car.y.CompareTo(median);
                    break;

                default:
                    median = Sequence.Median(Sequence.Unzip(points).car.MapEagerly((Vector3 v) => v.z));
                    splitFn = (Func.Tuple<Vector3, T> point) => point.car.z.CompareTo(median);
                    break;
            }
            

            var tup = points.Partition3(splitFn);
            if (tup.cpr.IsNone()) throw new Exception("Median not found. Huh?");

            var val = tup.cpr.Value();
            node.value = val.cdr;
            node.coords = val.car;
            node.left = null;
            node.right = null;
            node.parent = null;

            if (tup.car.Count > 0)
            {
                node.left = buildTree(tup.car, (dim + 1) % 3);
                node.left.parent = node;
            }

            if(tup.cdr.Count > 0)
            {
                node.right = buildTree(tup.cdr, (dim + 1) % 3);
                node.right.parent = node;
            }
            return node;
        }


        TDTreeNode<T> getCurrentBest(Vector3 pos, TDTreeNode<T> node, int dim)
        {
            if (node == null) return null;
            if (node.left == null && node.right == null) return node;

            if (node.left == null && node.right != null) return getCurrentBest(pos, node.right, (dim + 1) % 3);
            if (node.left != null && node.right == null) return getCurrentBest(pos, node.left, (dim + 1) % 3);

            float val;
            Func<Vector3, int> splitFn = null;
            
            switch (dim)
            {
                case 0:
                    val = node.coords.x;
                    splitFn = (Vector3 point) => point.x.CompareTo(val);
                    break;

                case 1:
                    val = node.coords.y;
                    splitFn = (Vector3 point) => point.y.CompareTo(val);
                    break;

                default:
                    val = node.coords.z;
                    splitFn = (Vector3 point) => point.z.CompareTo(val);
                    break;
            }

            if (splitFn(pos) < 0)
            {
                return getCurrentBest(pos, node.left, (dim + 1) % 3);
            }

            return getCurrentBest(pos, node.right, (dim + 1) % 3);
        }

        public T nearestNeighbour(Vector3 pos)
        {
            var currentBest = getCurrentBest(pos, root, 0);
            if (currentBest == null) return default(T);

            float sqdist = Vector3.SqrMagnitude(pos - currentBest.coords);

            for(var i = currentBest; i!=null; i = i.parent)
            {
                float d = Vector3.SqrMagnitude(pos - i.coords);
                if(d < sqdist)
                {
                    currentBest = i;
                    sqdist = d;
                }
            }

            return currentBest.value;
        }

        public override string ToString()
        {
            if (root == null) return "<empty tree>";
            return root.ToStringFormatted(0);
        }

    }
}
