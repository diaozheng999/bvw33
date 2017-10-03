using System;

namespace PGT.Core.Func
{
    /// <summary>
    /// A unit datatype, similar to ML-like languages
    /// </summary>
    public class unit : IComparable
    {
        /// <summary>
        /// unit
        /// </summary>
        public static unit _ = new unit();

        public static Func<unit, T> _f<T>(Func<T> f)
        {
            return (unit _) => f();
        }

        private unit() { }
        public override int GetHashCode()
        {
            return 0;
        }

        public override string ToString()
        {
            return "(unit)";
        }

        public override bool Equals(object obj)
        {
            return obj == null || obj.GetType() == typeof(unit);
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
