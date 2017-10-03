using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PGT.Core.Func
{
    public static class Option
    {

        public static Option<T> Some<T>(T value)
        {
            return Option<T>.Some(value);
        }

        public static Option<T> None<T>()
        {
            return Option<T>.None();
        }

    }


    public struct Option<T>
    {
        bool isSome;
        T value;

        private Option(bool isSome, T value)
        {
            this.isSome = isSome;
            this.value = value;
        }

        public static Option<T> Some(T value)
        {
            return new Option<T>(true, value);
        }

        public static Option<T> None()
        {
            return new Option<T>(false, default(T));
        }

        public bool IsSome()
        {
            return isSome;
        }

        public bool IsNone()
        {
            return !isSome;
        }

        public T Value()
        {
            if(!isSome) throw new System.ArgumentException("Value is None.");
            return value;
        }

        public bool ValEq(T val)
        {
            return isSome && value.Equals(val);
        }

        public Option<U> MapSome<U>(Func<T, U> f)
        {
            if (!isSome) return Option<U>.None();
            else return Option<U>.Some(f(value));
        }

        public override string ToString()
        {
            if (isSome) return "<Some " + value.ToString() + ">";
            else return "<None>";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Option<T>)) return false;
            var k = (Option<T>) obj;
            if (isSome != k.isSome) return false;
            return value.Equals(k.value);
        }

        public override int GetHashCode()
        {
            if (!isSome) return 0;
            return value.GetHashCode();
        }
    }
}
