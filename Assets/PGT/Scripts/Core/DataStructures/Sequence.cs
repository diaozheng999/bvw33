using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using PGT.Core.Func;
using UnityEngine;


namespace PGT.Core.DataStructures
{
    public interface Sequence<T> : IList<T> {
        Sequence<T> Rev();
        Sequence<T> Append(Sequence<T> other);

        Sequence<T> Filter(Func<T, bool> f);
        Sequence<T> FilterIndex(Func<int, bool> f);
        Sequence<T> FilterIdx(Func<int, T, bool> f);
        Sequence<T> FilterIdx(Func<Func.Tuple<int, T>, bool> f);
        Sequence<T> FilterIdx(TupledFunction<int, T, bool> f);
        Sequence<U> Map<U>(Func<T, U> f);
        Sequence<U> MapEagerly<U>(Func<T, U> f);
        Sequence<U> MapIndex<U>(Func<int, T, U> f);
        Sequence<U> MapIndex<U>(Func<Func.Tuple<int, T>, U> f);
        Sequence<U> MapIndex<U>(TupledFunction<int, T, U> f);
        Sequence<V> MapWith<U, V>(Sequence<U> other, Func<T, U, V> f);
        Sequence<V> MapWith<U, V>(Sequence<U> other, Func<Func.Tuple<T, U>, V> f);
        Sequence<V> MapWith<U, V>(Sequence<U> other, TupledFunction<T, U, V> f);
        Sequence<V> MapWithIndex<U, V>(Sequence<U> other, Func<int, T, U, V> f);
        Sequence<V> MapWithIndex<U, V>(Sequence<U> other, Func<Func.Tuple<int, T, U>, V> f);

        Coroutine RunAsync(MonoBehaviour mb, Func<T, object> f);

        Sequence<Func.Tuple<T, U>> ZipWith<U>(Sequence<U> other);

        Sequence<T> Inject(Sequence<Func.Tuple<int, T>> updates);
        Sequence<Sequence<T>> Implode(int segments);
        Sequence<T> Subseq(int start, int length);

        U Iter<U>(Func<U, T, U> f, U init);
        U Iter<U>(TupledFunction<U, T, U> f, U init);
        U Iter<U>(Func<Func.Tuple<U, T>, U> f, U init);
        Func.Tuple<Sequence<U>, U> Iterh<U>(Func<U, T, U> f, U init);
        Func.Tuple<Sequence<U>, U> Iterh<U>(Func<Func.Tuple<U, T>, U> f, U init);
        T Reduce(Func<T, T, T> f, T id);
        T Reduce(Func<Func.Tuple<T, T>, T> f, T id);
        T Reduce(TupledFunction<T, T, T> f, T id);

        Func.Tuple<Sequence<T>, T> Scan(Func<T, T, T> f, T init);
        Func.Tuple<Sequence<T>, T> Scan(Func<Func.Tuple<T, T>, T> f, T init);

        Sequence<T> ScanIncl(Func<T, T, T> f, T init);
        Sequence<T> ScanIncl(Func<Func.Tuple<T, T>, T> f, T init);
        Sequence<T> Memoize();

        Sequence<T> Take(int i);
        Sequence<T> Drop(int i);
    
        Func.Tuple<Sequence<T>, Sequence<T>, Option<T>> Partition3(Func<T, int> comparator);

        T[] ToArray();

    }



    /// <summary>
    /// Static and unparameterised class that offers utility functions that 
    /// deals with multiple sequences.
    /// </summary>
    public static class Sequence
    {
        public static Sequence<T> Tabulate<T>(int length, Func<int, T> f) =>
            new FuncSequence<T>(length, f);

        public static Sequence<int> Range(int length) =>
            Tabulate(length, Function.id);

        public static Sequence<T> Empty<T>() => 
            new EmptySequence<T>();

        public static Sequence<T> Singleton<T>(T item) => 
            new SingletonSequence<T>(item);

        public static Sequence<T> Unroll<T>(IEnumerable<T> states) =>
            new GeneratorSequence<T>(states);


        public static Sequence<T> UnrollN<T>(int n, IEnumerable<T> states) => 
            new GeneratorSequence<T>(n, states);

        public static Sequence<T> UnrollN<T>(ICollection<T> states) =>
            new GeneratorSequence<T>(states);


        public static Sequence<T> List<T>(List<T> list) =>
            new ListSequence<T>(list);

        /// <summary>
        /// Unzips a sequence lazily.
        /// <summary>
        public static Func.Tuple<Sequence<T>, Sequence<U>> Unzip<T, U>(
            Sequence<Func.Tuple<T, U>> seq
        )
        {
            return Func.Tuple._(
                seq.Map(Func.Tuple<T, U>.Car),
                seq.Map(Func.Tuple<T, U>.Cdr)
            );
        }
        public static Sequence<Func.Tuple<T, U>> Pairwise<T, U>(
            Sequence<T> s1, Sequence<U> s2
        ){
            return Sequence.Flatten(s1.Map((T fst) => s2.Map((U snd) => Func.Tuple._(fst, snd))));
        }



        /// <summary>
        /// Unzips a sequence eagerly. Note that the sequence is traversed 
        /// twice. Therefore, any side-effects present within the sequence
        /// will be evaluated twice.
        /// </summary>
        public static Func.Tuple<Sequence<T>, Sequence<U>> UnzipEagerly<T, U>(Sequence<Func.Tuple<T, U>> seq)
        {
            return Func.Tuple._(
                seq.Map(Func.Tuple<T, U>.Car).Memoize(),
                seq.Map(Func.Tuple<T, U>.Cdr).Memoize()
            );
        }

        /// <summary>
        /// Finds the median of a (comparable) sequence. This algorithm
        /// has an expected runtime of O(n).
        /// </summary>
        public static T Median<T>(Sequence<T> seq) where T : IComparable<T>
        {
            int i = seq.Count;
            return QuickSelect(seq, (i+1) / 2);
        }

        public static Sequence<T> QuickSort<T>(Sequence<T> seq) where T : IComparable<T>
        {
            if(seq.Count == 1) return seq;

            var _rng = new System.Random();
            var i = Mathf.FloorToInt((float)_rng.NextDouble() * seq.Count);
            while (i == seq.Count)
            {
                i = Mathf.FloorToInt((float)_rng.NextDouble() * seq.Count);
            }

            var mid = seq[i];
            var tup = seq.Partition3(mid.CompareTo);
            if(tup.cpr.IsSome()){
                return Sequence.Flatten(Sequence.Tabulate(3, (int j) => {
                    switch(j){
                        case 0: return Sequence.QuickSort(tup.car);
                        case 1: return Sequence.Singleton(tup.cpr.Value());
                        case 2: return Sequence.QuickSort(tup.cdr);
                        default: return Sequence.Empty<T>();
                    }
                }));
            } else {
                return Sequence.QuickSort(tup.car).Append(Sequence.QuickSort(tup.cdr));
            }
        }

        /// <summary>
        /// Performs the QuickSelect algorithm on a (comparable) sequence. 
        /// </summary>
        static T QuickSelect<T>(Sequence<T> seq, int k) where T : IComparable<T>
        {
            if(seq.Count == 1 && k == 0)
            {
                return seq[0];
            }

            var _rng = new System.Random();

            var i = Mathf.FloorToInt((float)_rng.NextDouble() * seq.Count);
            while (i == seq.Count)
            {
                i = Mathf.FloorToInt((float)_rng.NextDouble() * seq.Count);
            }


            var mid = seq[i];
            var tup = seq.Partition3(mid.CompareTo);
            
            var lCount = tup.car.Count;
            if (lCount == k - 1) return mid;
            if (lCount >= k) return QuickSelect(tup.car, k);
            if (tup.cpr.IsSome()) return QuickSelect(tup.cdr, k - lCount - 1);
            return QuickSelect(tup.cdr, k - lCount);
        }

        public static Sequence<T> Array<T>(T[] seq) => new ArraySequence<T>(seq);

        public static Sequence<T> Flatten<T>(Sequence<Sequence<T>> seq){
            return seq.Reduce((Sequence<T> s, Sequence<T> y) => s.Append(y), Sequence.Empty<T>());
        }

        public static Sequence<float> OfVector3OneExtended(Vector3 value, int len=3) => 
            Tabulate(len, (int i) => {
                switch(i){
                    case 0: return value.x;
                    case 1: return value.y;
                    case 2: return value.z;
                    default: return 1;
                }
            });

        public static Sequence<U> MapArray<T, U>(T[] array, Func<T, U> f) =>
            new FuncSequence<U>(array.Length, (int i) => f(array[i]));
        

        public static T ReduceArray<T>(T[] array, T id, Func<T, T, T> f)
        {
            T accum = id;
            int len = array.Length;
            for (int i = 0; i < len; i++)
            {
                accum = f(accum, array[i]);
            }
            return accum;
        }

        public static U MapReduceArray<T, U>(T[] array, Func<T, U> map_fn, U id, Func<U, U, U> red_fn)
        {
            U accum = id;
            int len = array.Length;
            for(int i=0; i < len; i++)
            {
                accum = red_fn(accum, map_fn(array[i]));
            }
            return accum;
        }

        public static unit ApplyArray<T>(T[] array, Action<T> fn)
        {
            foreach (T e in array)
            {
                fn(e);
            }
            return null;
        }

        public static unit ApplyArray<T>(T[] array, Func<T, unit> fn)
        {
            foreach(T e in array)
            {
                fn(e);
            }
            return null;
        }
    }

    public class ArraySequence<T> : BaseSequence<T> {
        T[] items;
        public ArraySequence(T[] _items){
            items = _items;
        }

        public override T this[int i]{
            get { return items[i]; }
            set { throw new System.Exception(); }
        }
        public override Sequence<T> Memoize(){
            return this;
        }

        public override int Count { get {return items.Length; } }
    }

    public class ListSequence<T> : BaseSequence<T> {
        List<T> items;
        public ListSequence(List<T> _items){
            items = _items;
        }

        public override T this[int i]{
            get { return items[i]; }
            set { throw new System.Exception(); }
        }

        public override Sequence<T> Memoize(){
            return this;
        }

        public override int Count { get { return items.Count; } }
    }

    public class SliceSequence<T> : BaseSequence<T> {
        Sequence<T> base_seq;
        int length;
        int start;
        public SliceSequence(Sequence<T> base_seq, int start, int length) {
            this.length = length;
            this.start = start;
            this.base_seq = base_seq;
        }

        public override T this[int i] {
            get {
                if(i >= length) throw new IndexOutOfRangeException();
                return base_seq[i + start];   
            }
            set { throw new System.Exception(); }
        }

        public override Sequence<T> Subseq(int new_start, int new_length){
            if(new_start + new_length > length) throw new InvalidOperationException();
            return new SliceSequence<T>(base_seq, new_start + start, new_length);
        }

        public override int Count { get { return length; } }
    }

    public class EmptySequence<T> : BaseSequence<T>{
        public override int Count { get {return 0;} }

        public override T this[int i]{
            get { throw new IndexOutOfRangeException(); }
            set { throw new System.Exception(); }
        }
    }

    public class SingletonSequence<T> : BaseSequence<T>{
        T item;

        public SingletonSequence(T item){
            this.item = item;
        }

        public override int Count { get {return 1; } }

        public override T this[int i]{
            get { 
                if (i==0) return item;
                throw new IndexOutOfRangeException(); 
            }
            set { throw new System.Exception(); }
        }
    }

    public class FuncSequence<T> : BaseSequence<T> {
        Func<int, T> gen_fn;
        int length;
        public FuncSequence(int len, Func<int, T> generator){
            gen_fn = generator;
            length = len;
        }

        public override T this[int i] { 
            get { return gen_fn(i); }
            set { throw new System.Exception(); }
        }

        public override int Count { get {return length;} }
    }

    public class GeneratorSequence<T> : BaseSequence<T> {
        IEnumerable<T> gen_fn;
        Option<int> length;
        List<T> memoized;
        int unrolled_until = 0;

        IEnumerator<T> unroll_state;

        public GeneratorSequence(ICollection<T> item){
            gen_fn = item;
            unroll_state = item.GetEnumerator();
            length = Option.Some(item.Count);
        }

        public GeneratorSequence(int n, IEnumerable<T> item){
            gen_fn = item;
            unroll_state = item.GetEnumerator();
            length = Option.Some(n);
        }

        public GeneratorSequence(IEnumerable<T> generator){
            gen_fn = generator;
            unroll_state = generator.GetEnumerator();
        }


        bool Unroll(){
            var success = unroll_state.MoveNext();
            if(success){
                if(memoized == null) memoized = new List<T>();
                memoized.Add(unroll_state.Current);
                ++ unrolled_until;
            }
            return success;
        }

        void UnrollAll(){
            while (Unroll()) ;
        }

        public override int Count {
            get {
                if(length.IsSome()){
                    return length.Value();
                }
                UnrollAll();
                length = Option.Some(unrolled_until);
                return unrolled_until;
            }
        }

        public override T this[int i] {
            get {
                while(unrolled_until <= i){
                    Unroll();
                }
                return memoized[i];
            }
            set {
                throw new System.Exception();
            }
        }

        public override Sequence<T> Memoize(){
            UnrollAll();
            return new ListSequence<T>(memoized);
        }
    }

    public abstract class BaseSequence<T> : Sequence<T>{
        
        public const int GRANULARITY = 5000;
        int _nthreads = -1;
        protected int nthreads { get {
            if (_nthreads < 0) _nthreads = Count / GRANULARITY + 1;
            return _nthreads;
        }}


        public abstract T this[int i] { get; set; }
        public abstract int Count { get; }
        public bool IsReadOnly { get {return true;} }


        public virtual Sequence<T> Rev() => 
            Sequence.Tabulate(Count, (int i) => this[Count - i - 1]);

        public virtual Sequence<T> FilterIndex(Func<int, bool> predicate) =>
            Sequence.Unroll(_filterid_gen(predicate));

        public virtual Sequence<T> Filter(Func<T, bool> predicate) =>
            Sequence.Unroll(_filter_gen(predicate));

        public virtual Sequence<T> FilterIdx(Func<int, T, bool> predicate) =>
            Sequence.Unroll(_filteridx_gen(predicate));

        public Sequence<T> FilterIdx(TupledFunction<int, T, bool> predicate) =>
            FilterIdx(predicate._f);

        public Sequence<T> FilterIdx(Func<Func.Tuple<int, T>, bool> predicate) =>
            FilterIdx(Function._untup(predicate));

        public virtual Sequence<U> Map<U>(Func<T, U> f) => 
            Sequence.Tabulate(Count, (int i) => f(this[i]));

        public virtual Sequence<U> MapEagerly<U>(Func<T, U> f) =>
            Map(f).Memoize();

        public virtual Sequence<U> MapIndex<U>(Func<int, T, U> f) => 
            Sequence.Tabulate(Count, (int i) => f(i, this[i]));

        public Sequence<U> MapIndex<U>(Func<Func.Tuple<int, T>, U> f) => MapIndex(Function._untup(f));
        
        public Sequence<U> MapIndex<U>(TupledFunction<int, T, U> f) => MapIndex(f._f);

        public virtual Sequence<V> MapWith<U, V>(Sequence<U> other, Func<T, U, V> f){
            if(Count != other.Count) throw new IndexOutOfRangeException("Sequence length mismatch.");
            return Sequence.Tabulate(Count, (int i) => f(this[i], other[i]));
        }

        public Sequence<V> MapWith<U, V>(Sequence<U> other, TupledFunction<T, U, V> f)
            => MapWith(other, f._f);

        public Sequence<V> MapWith<U, V>(Sequence<U> other, Func<Func.Tuple<T, U>, V> f)
            => MapWith(other, Function._untup(f));


        public virtual Sequence<V> MapWithIndex<U, V>(Sequence<U> other, Func<int, T, U, V> f) {
            if(Count != other.Count) throw new IndexOutOfRangeException("Sequence length mismatch.");
            return Sequence.Tabulate(Count, (int i) => f(i, this[i], other[i]));
        }

        public Sequence<V> MapWithIndex<U, V>(Sequence<U> other, Func<Func.Tuple<int, T, U>, V> f)
            => MapWithIndex(other, Function._untup(f));
    
        public Sequence<Func.Tuple<T, U>> ZipWith<U>(Sequence<U> other){
            var _mycount = Count;
            var _othercount = other.Count;
            var len = _mycount < _othercount ? _mycount : _othercount;
            return Sequence.Tabulate(len, (int i) => Func.Tuple._(this[i], other[i]));
        }


        public virtual Sequence<T> Inject(Sequence<Func.Tuple<int, T>> updates) =>
            Sequence.Tabulate(Count, (int i) =>
                updates.Iter((T elem, Func.Tuple<int, T> update) => update.car == i ? update.cdr : elem, this[i])
            );
        
        public virtual Sequence<T> Subseq(int start, int length) =>
            new SliceSequence<T>(this, start, length);


        public virtual U Iter<U>(Func<U, T, U> f, U init) {
            U it = init;
            foreach(var item in this){
                it = f(it, item);
            }
            return it;
        }

        public U Iter<U>(Func<Func.Tuple<U, T>, U> f, U init) => Iter(Function._untup(f), init);

        public U Iter<U>(TupledFunction<U, T, U> f, U init) => Iter(f._f, init);


        IEnumerable<T> _filter_gen(Func<T, bool> predicate) {
            foreach(var i in this){
                if(predicate(i)) yield return i;
            }
        }

        IEnumerable<T> _filteridx_gen(Func<int, T, bool> predicate) {
            var len = Count;
            for(int i=0; i<len; ++i){
                var cached = this[i];
                if(predicate(i, cached)) yield return cached;
            }
        }

        IEnumerable<T> _filterid_gen(Func<int, bool> predicate){
            var len = Count;
            for(int i=0; i < len; ++i){
                if(predicate(i)) yield return this[i];
            }
        }

        public virtual Sequence<T> Memoize()
        {
            var length = Count;
            var memoized = new T[length];

            //Multithreaded memoize function
            if(nthreads <= 1)
            {
                for(int i=0; i<length; i++)
                {
                    memoized[i] = this[i];
                }
                return new ArraySequence<T>(memoized);
            }

            if(nthreads >= GRANULARITY)
            {
                Debug.LogWarning("Creating too many threads. Consider using Seq of Seqs");
            }

            Parallel.For(0, (length+GRANULARITY-1)/GRANULARITY, (int i) => {
                int start = i * GRANULARITY;
                int l = (i + 1) * GRANULARITY;
                int j = length < l ? length : l;
                for(int k = start; k < j; k++)
                {
                    memoized[i] = this[i];
                }
            });

            return Sequence.Array(memoized);
        }

        public virtual Sequence<Sequence<T>> Implode(int subseqlength = GRANULARITY)
        {
            int length = Count;
            int nlength = (length + (subseqlength - 1)) / subseqlength + 1;
            return Sequence.Tabulate(nlength, (int i) =>
            {
                var sstart = i * subseqlength;
                var slength = (sstart + subseqlength) > length ? (length - sstart) : subseqlength;
                return this.Subseq(sstart, slength);
            });
        }

    
        public virtual Sequence<T> Append(Sequence<T> other)
        {
            var c = Count;
            return Sequence.Tabulate(c + other.Count, (int i) => i < c ? this[i] : other[i - c]);
        }

        public unit Apply(Func<T, unit> f)
        {
            var length = Count;
            for(int i=0; i<length; i++)
            {
                f(this[i]);
            }
            return null;
        }

        public unit Apply(Action<T> f)
        {
            var length = Count;
            for (int i = 0; i < length; i++)
            {
                f(this[i]);
            }
            return null;
        }

        public int IndexOf(T item)
        {
            throw new NotImplementedException();
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotImplementedException();
        }

        public void Add(T item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public virtual Func.Tuple<Sequence<T>, Sequence<T>, Option<T>> Partition3(Func<T, int> comparator)
        {
            /// TODO: multithreaded partitioning

            var length = Count;

            List<T> left = new List<T>();
            List<T> right = new List<T>();
            Option<T> mid = Option<T>.None();

            for (int i = 0; i < length; i++)
            {
                var v = this[i];
                var cmp = comparator(v);
                if (cmp < 0)
                {
                    left.Add(v);
                }
                else if(cmp==0 && mid.IsNone())
                {
                    mid = Option<T>.Some(v);
                }
                else
                {
                    right.Add(v);
                }
            }

            Sequence<T> _left = Sequence.List(left);
            Sequence<T> _right = Sequence.List(right);

            return Func.Tuple._(_left, _right, mid);
        }

        public virtual T Reduce(Func<T, T, T> f, T id)
        {
            var length = Count;
            if(nthreads <= 1)
            {
                var acc = id;
                for(int i=0; i<length; i++)
                {
                    acc = f(acc, this[i]);
                }
                return acc;
            }
            return Implode().Map((Sequence<T> seq) => seq.Reduce(f, id)).Memoize().Reduce(f, id);
        }

        public T Reduce(TupledFunction<T, T, T> f, T id) => Reduce(f._f, id);

        public T Reduce(Func<Func.Tuple<T, T>, T> f, T id) => Reduce(Function._untup(f), id);

        public IEnumerator<T> GetEnumerator()
        {
            var length = Count;
            for(int i=0; i<length; i++)
            {
                yield return this[i];
            }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public virtual T[] ToArray()
        {
            var length = Count;
            T[] arr = new T[length];
            for(int i=0; i<length; i++)
            {
                arr[i] = this[i];
            }
            return arr;
        }

        public override string ToString()
        {
            var ret = "Sequence of Type " + typeof(T).ToString() + " (" + Count + " item(s))\n<";
            
            if(Count > 5)
            {
                for(int i=0; i<5; i++)
                {
                    ret += this[i].ToString() + ", ";
                }
                return ret + "...>";
            }

            for(int i=0; i<Count; i++)
            {
                if (i != 0)
                {
                    ret += ", " + this[i].ToString();
                }else
                {
                    ret += this[i].ToString();
                }

            }
            return ret + ">";
        }

        public virtual Func.Tuple<Sequence<U>, U> Iterh<U>(Func<U, T, U> f, U init){
            var len = Count;
            var it = init;
            var its = new U[len];
            for(int i=0; i<len; ++i){
                its[i] = it;
                it = f(it, this[i]);
            }
            return Func.Tuple._(Sequence.Array(its), it);
        }

        public Func.Tuple<Sequence<U>, U> Iterh<U>(Func<Func.Tuple<U, T>, U> f, U init) =>
            Iterh(Function._untup(f), init);

        public virtual Func.Tuple<Sequence<T>, T> Scan(Func<T, T, T> f, T init){
            var len = Count;
            switch(len){
                case 0:
                    return Func.Tuple._(Sequence.Empty<T>(), init);
                case 1:
                    return Func.Tuple._(Sequence.Singleton(init), this[0]);
                default:
                    
                    var contract = Sequence.Tabulate(
                        (len + 1) / 2,
                        (int i) => (i == len / 2) ? this[2*i] : f(this[2*i], this[2*i+1])
                    ).Scan(f, init);

                    var c_seq = contract.car.Memoize();

                    return Func.Tuple._(
                        Sequence.Tabulate(
                            len,
                            (int i) => (i % 2 == 0) ? c_seq[i/2] : f(c_seq[i/2], this[i-1])
                        ),  
                        contract.cdr
                    );
            }
        }

        public virtual Sequence<T> ScanIncl(Func<T, T, T> f, T init) {
            var scanned = Scan(f, init);
            return scanned.car.Append(Sequence.Singleton(scanned.cdr)).Drop(1).Memoize();
        }

        public virtual Sequence<T> ScanIncl(Func<Func.Tuple<T, T>, T> f, T init) {
            var scanned = Scan(f, init);
            return scanned.car.Append(Sequence.Singleton(scanned.cdr)).Drop(1).Memoize();
        }

        public Func.Tuple<Sequence<T>, T> Scan(Func<Func.Tuple<T, T>, T> f, T init) =>
            Scan(Function._untup(f), init);

        public virtual bool Contains(T item) => Filter((T i) => i.Equals(item)).Count > 0;

        public virtual void CopyTo(T[] array, int arrayIndex){
            var len = Count;
            for(int i=0; i<len; ++i){
                array[arrayIndex+i] = this[i];
            }
        }

        public virtual Coroutine RunAsync(MonoBehaviour mb, Func<T, object> f){
            return mb.StartCoroutine(Map(f).GetEnumerator());
        }

        public virtual Sequence<T> Take(int i) => Subseq(0, i);

        public virtual Sequence<T> Drop(int i) => Subseq(i, Count-i);
    }
}
