using System;
using System.Collections.Generic;
using UnityEngine;

namespace PGT.Core.DataStructures
{
    public class Vector3CompareX : IComparer<Vector3>
    {
        public int Compare(Vector3 x, Vector3 y)
        {
            return x.x.CompareTo(y.x);
        }
    }

    public class Vector3CompareY : IComparer<Vector3>
    {
        public int Compare(Vector3 x, Vector3 y)
        {
            return x.y.CompareTo(y.y);
        }
    }

    public class Vector3CompareZ : IComparer<Vector3>
    {
        public int Compare(Vector3 x, Vector3 y)
        {
            return x.z.CompareTo(y.z);
        }
    }

    public class Vector2CompareX : IComparer<Vector2>
    {
        public int Compare(Vector2 x, Vector2 y)
        {
            return x.x.CompareTo(y.x);
        }
    }

    public class Vector2CompareY : IComparer<Vector2>
    {
        public int Compare(Vector2 x, Vector2 y)
        {
            return x.y.CompareTo(y.y);
        }
    }
}
