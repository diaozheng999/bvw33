using System;
using System.Collections.Generic;
using PGT.Core.DataStructures;
using System.Threading.Tasks;

namespace PGT.Core.Func
{
    public static class Tuple
    {


        public static Dictionary<int, Dictionary<int, List<object>>> tup2;
        public static Dictionary<int, Dictionary<int, List<object>>> tup3;
        public static Dictionary<int, Dictionary<int, List<object>>> tup4;
        
        public static Tuple<A, B> _<A, B>(A car, B cdr)
        {
            return Tuple<A, B>._(car, cdr);
        }

        public static Tuple<A ,B, C> _<A, B, C>(A car, B cdr, C cpr)
        {
            return Tuple<A, B, C>._(car, cdr, cpr);
        }

        public static Tuple<A, B, C, D> _<A, B, C, D>(A car, B cdr, C cpr, D ctr)
        {
            return Tuple<A, B, C, D>._(car, cdr, cpr, ctr);
        }
        
        public static A car<A, B>(Tuple<A, B> tup){ return tup.car; }
        public static A car<A, B, C>(Tuple<A, B, C> tup) { return tup.car; }
        public static A car<A, B, C, D>(Tuple<A, B, C, D> tup) { return tup.car; }
        public static B cdr<A, B>(Tuple<A, B> tup) { return tup.cdr; }
        public static B cdr<A, B, C>(Tuple<A, B, C> tup) { return tup.cdr; }
        public static B cdr<A, B, C, D>(Tuple<A, B, C, D> tup) { return tup.cdr; }
        public static C cpr<A, B, C>(Tuple<A, B, C> tup) { return tup.cpr; }
        public static C cpr<A, B, C, D>(Tuple<A, B, C, D> tup) { return tup.cpr; }
        public static D ctr<A, B, C, D>(Tuple<A, B, C, D> tup) { return tup.ctr; }

        public static Tuple<A, B> cadr<A, B, C>(Tuple<A, B, C> tup) { return _(tup.car, tup.cdr); }
        public static Tuple<B, C> cdpr<A, B, C>(Tuple<A, B, C> tup) { return _(tup.cdr, tup.cpr); }
        public static Tuple<A, C> capr<A, B, C>(Tuple<A, B, C> tup) { return _(tup.car, tup.cpr); }
        public static Tuple<A, B> cadr<A, B, C, D>(Tuple<A, B, C, D> tup) { return _(tup.car, tup.cdr); }
        public static Tuple<B, C> cdpr<A, B, C, D>(Tuple<A, B, C, D> tup) { return _(tup.cdr, tup.cpr); }
        public static Tuple<C, D> cptr<A, B, C, D>(Tuple<A, B, C, D> tup) { return _(tup.cpr, tup.ctr); }
        public static Tuple<A, C> capr<A, B, C, D>(Tuple<A, B, C, D> tup) { return _(tup.car, tup.cpr); }
        public static Tuple<B, D> cdtr<A, B, C, D>(Tuple<A, B, C, D> tup) { return _(tup.cdr, tup.ctr); }
        public static Tuple<A, D> catr<A, B, C, D>(Tuple<A, B, C, D> tup) { return _(tup.car, tup.ctr); }

        public static Tuple<A, B, C> cadpr<A, B, C, D>(Tuple<A, B, C, D> tup) { return _(tup.car, tup.cdr, tup.cpr); }
        public static Tuple<B, C, D> cdptr<A, B, C, D>(Tuple<A, B, C, D> tup) { return _(tup.cdr, tup.cpr, tup.ctr); }
        public static Tuple<A, C, D> captr<A, B, C, D>(Tuple<A, B, C, D> tup) { return _(tup.car, tup.cpr, tup.ctr); }
        public static Tuple<A, B, D> cadtr<A, B, C, D>(Tuple<A, B, C, D> tup) { return _(tup.car, tup.cdr, tup.ctr); }

        public static Tuple<B, A> rev<A, B>(Tuple<A, B> tup) { return _(tup.cdr, tup.car); }


        public static Tuple<B, B> map<A, B>(Func<A, B> f, Tuple<A, A> tup) { return _(f(tup.car), f(tup.cdr)); }
        public static Tuple<B, B, B> map<A, B>(Func<A, B> f, Tuple<A, A, A> tup) { return _(f(tup.car), f(tup.cdr), f(tup.cpr)); }
        public static Tuple<B, B, B, B> map<A, B>(Func<A, B> f, Tuple<A, A, A, A> tup) { return _(f(tup.car), f(tup.cdr), f(tup.cpr), f(tup.ctr)); }
        public static Tuple<B, C> map_car<A, B, C>(Func<A, B> f, Tuple<A, C> tup) { return _(f(tup.car), tup.cdr); }
        public static Tuple<C, B> map_cdr<A, B, C>(Func<A, B> f, Tuple<C, A> tup) { return _(tup.car, f(tup.cdr)); }
        public static Tuple<B, C, D> map_car<A, B, C, D>(Func<A, B> f, Tuple<A, C, D> tup) { return _(f(tup.car), tup.cdr, tup.cpr); }
        public static Tuple<C, B, D> map_cdr<A, B, C, D>(Func<A, B> f, Tuple<C, A, D> tup) { return _(tup.car, f(tup.cdr), tup.cpr); }
        public static Tuple<C, D, B> map_cpr<A, B, C, D>(Func<A, B> f, Tuple<C, D, A> tup) { return _(tup.car, tup.cdr, f(tup.cpr)); }
        public static Tuple<B, C, D, E> map_car<A, B, C, D, E>(Func<A, B> f, Tuple<A, C, D, E> tup) { return _(f(tup.car), tup.cdr, tup.cpr, tup.ctr); }
        public static Tuple<C, B, D, E> map_cdr<A, B, C, D, E>(Func<A, B> f, Tuple<C, A, D, E> tup) { return _(tup.car, f(tup.cdr), tup.cpr, tup.ctr); }
        public static Tuple<C, D, B, E> map_cpr<A, B, C, D, E>(Func<A, B> f, Tuple<C, D, A, E> tup) { return _(tup.car, tup.cdr, f(tup.cpr), tup.ctr); }
        public static Tuple<C, D, E, B> map_ctr<A, B, C, D, E>(Func<A, B> f, Tuple<C, D, E, A> tup) { return _(tup.car, tup.cdr, tup.cpr, f(tup.ctr)); }
        
        public static Tuple<C, D> par<A, B, C, D>(Tuple<Func<A, C>, Func<B, D>> tup1, Tuple<A, B> tup2) { return _(tup1.car(tup2.car), tup1.cdr(tup2.cdr)); }
        public static Tuple<B, D, F> par<A, B, C, D, E, F>(Tuple<Func<A, B>, Func<C, D>, Func<E, F>> tup1, Tuple<A, C, E> tup2)
        {
            return _(tup1.car(tup2.car), tup1.cdr(tup2.cdr), tup1.cpr(tup2.cpr));
        }
        public static Tuple<B, D, F, H> par<A, B, C, D, E, F, G, H>(Tuple<Func<A, B>, Func<C, D>, Func<E, F>, Func<G, H>> tup1, Tuple<A, C, E, G> tup2)
        {
            return _(tup1.car(tup2.car), tup1.cdr(tup2.cdr), tup1.cpr(tup2.cpr), tup1.ctr(tup2.ctr));
        }



        public static Tuple<C, D> par1<A, B, C, D>(Tuple<Func<A, C>, Func<B, D>> tup1, Tuple<A, B> tup2)
        {

            var r1 = new Task<C>(() => tup1.car(tup2.car));
            var r2 = new Task<D>(() => tup1.cdr(tup2.cdr));

            r1.Start();
            r2.Start();

            r1.Wait();
            r2.Wait();

            return _(r1.Result, r2.Result);
        }

        public static Tuple<B, D, F> par1<A, B, C, D, E, F>(Tuple<Func<A, B>, Func<C, D>, Func<E, F>> tup1, Tuple<A, C, E> tup2)
        {
            var r1 = new Task<B>(() => tup1.car(tup2.car));
            var r2 = new Task<D>(() => tup1.cdr(tup2.cdr));
            var r3 = new Task<F>(() => tup1.cpr(tup2.cpr));

            r1.Start();
            r2.Start();
            r3.Start();

            r1.Wait();
            r2.Wait();
            r3.Wait();

            return _(r1.Result, r2.Result, r3.Result);
        }

        public static Tuple<B, D, F, H> par1<A, B, C, D, E, F, G, H>(Tuple<Func<A, B>, Func<C, D>, Func<E, F>, Func<G, H>> tup1, Tuple<A, C, E, G> tup2)
        {
            var r1 = new Task<B>(() => tup1.car(tup2.car));
            var r2 = new Task<D>(() => tup1.cdr(tup2.cdr));
            var r3 = new Task<F>(() => tup1.cpr(tup2.cpr));
            var r4 = new Task<H>(() => tup1.ctr(tup2.ctr));

            r1.Start();
            r2.Start();
            r3.Start();
            r4.Start();

            r1.Wait();
            r2.Wait();
            r3.Wait();
            r4.Wait();

            return _(r1.Result, r2.Result, r3.Result, r4.Result);
        }


        public static void Reinit()
        {
            tup2.Clear();
            tup3.Clear();
            tup4.Clear();
        }
    }

    /// <summary>
    /// A struct representing a two-tuple
    /// </summary>
    /// <typeparam name="A"></typeparam>
    /// <typeparam name="B"></typeparam>
    public struct Tuple<A, B> 
    {
        public A car { get; private set; }
        public B cdr { get; private set; }
        

        private Tuple(A _car, B _cdr)
        {
            car = _car;
            cdr = _cdr;
        }

        public static A Car (Tuple<A, B> tup)
        {
            return tup.car;
        }

        public static B Cdr (Tuple<A, B> tup)
        {
            return tup.cdr;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            Tuple<A, B> b = (Tuple<A, B>)obj;
            return car.Equals(b.car) && cdr.Equals(b.cdr);
        }

        public override int GetHashCode()
        {
            var _car = EqualityComparer<A>.Default.Equals(car, default(A)) ? 0 : car.GetHashCode();
            var _cdr = EqualityComparer<B>.Default.Equals(cdr, default(B)) ? 0 : cdr.GetHashCode();
            return _car * 31 + _cdr;
        }

        public override string ToString()
        {
            return "("+car.ToString()+", "+cdr.ToString()+")";
        }

#if NET_4_6
        public static implicit operator System.Tuple<A, B>(Tuple<A, B> tup){
            return new System.Tuple<A, B>(tup.car, tup.cdr);
        }
#endif

        public static Tuple<A, B> _(A car, B cdr)
        {
            var car_hash = EqualityComparer<A>.Default.Equals(car, default(A)) ? 0 : car.GetHashCode();
            var cdr_hash = EqualityComparer<B>.Default.Equals(cdr, default(B)) ? 0 : cdr.GetHashCode();

            if (Tuple.tup2 == null) Tuple.tup2 = new Dictionary<int, Dictionary<int, List<object>>>();
            if (!Tuple.tup2.ContainsKey(car_hash)) Tuple.tup2[car_hash] = new Dictionary<int, List<object>>();
            if (!Tuple.tup2[car_hash].ContainsKey(cdr_hash)) Tuple.tup2[car_hash][cdr_hash] = new List<object>();

            foreach (var item in Tuple.tup2[car_hash][cdr_hash])
            {
                if (item is Tuple<A, B>)
                {
                    var _item = (Tuple<A, B>)item;
                    if (EqualityComparer<A>.Default.Equals(_item.car, car) && EqualityComparer<B>.Default.Equals(_item.cdr, cdr))
                    {
                        return _item;
                    }
                }
            }

            var n_tup = new Tuple<A, B>(car, cdr);
            Tuple.tup2[car_hash][cdr_hash].Add(n_tup);
            return n_tup;
        }

        public static void Clean()
        {
            Tuple.tup2.Clear();
        }
    }

    public struct Tuple<A, B, C>
    {
        public A car { get; private set; }
        public B cdr { get; private set; }
        public C cpr { get; private set; }

        private Tuple(A _car, B _cdr, C _cpr) 
        {
            car = _car;
            cdr = _cdr;
            cpr = _cpr;
        }

        public static A Car(Tuple<A, B, C> tup)
        {
            return tup.car;
        }

        public static B Cdr(Tuple<A, B, C> tup)
        {
            return tup.cdr;
        }

        public static C Cpr(Tuple<A, B, C> tup)
        {
            return tup.cpr;
        }

        public override bool Equals(object obj)
        {
            Tuple<A, B, C> b = (Tuple<A, B,C>)obj;
            return car.Equals(b.car) && cdr.Equals(b.cdr) && cpr.Equals(b.cpr);
        }
        public override int GetHashCode()
        {

            var _car = EqualityComparer<A>.Default.Equals(car, default(A)) ? 0 : car.GetHashCode();
            var _cdr = EqualityComparer<B>.Default.Equals(cdr, default(B)) ? 0 : cdr.GetHashCode();
            var _cpr = EqualityComparer<C>.Default.Equals(cpr, default(C)) ? 0 : cdr.GetHashCode();
            return (_cpr * 31 + _car) * 31 + _cdr;
        }
        public override string ToString()
        {
            return "(" + car.ToString() + ", " + cdr.ToString() + ", " + cpr.ToString() + ")";
        }



        public static Tuple<A, B, C> _(A car, B cdr, C cpr)
        {
            var fst_hash = Tuple<A, B>._(car, cdr).GetHashCode();
            var cpr_hash = cpr.GetHashCode();

            if (Tuple.tup3 == null) Tuple.tup3 = new Dictionary<int, Dictionary<int, List<object>>>();
            if (!Tuple.tup3.ContainsKey(fst_hash)) Tuple.tup3[fst_hash] = new Dictionary<int, List<object>>();
            if (!Tuple.tup3[fst_hash].ContainsKey(cpr_hash)) Tuple.tup3[fst_hash][cpr_hash] = new List<object>();

            foreach (var item in Tuple.tup3[fst_hash][cpr_hash])
            {
                if (item is Tuple<A, B, C>)
                {
                    var _item = (Tuple<A, B, C>)item;
                    if (EqualityComparer<A>.Default.Equals(_item.car, car) 
                        && EqualityComparer<B>.Default.Equals(_item.cdr, cdr)
                        && EqualityComparer<C>.Default.Equals(_item.cpr, cpr))
                    {
                        return _item;
                    }
                }
            }

            var n_tup = new Tuple<A, B, C>(car, cdr, cpr);
            Tuple.tup3[fst_hash][cpr_hash].Add(n_tup);
            return n_tup;
        }

        public static void Clean()
        {
            Tuple.tup3.Clear();
        }

    }
    public struct Tuple<A, B, C, D>
    {
        public A car { get; private set; }
        public B cdr { get; private set; }
        public C cpr { get; private set; }
        public D ctr { get; private set; }

        private Tuple(A _car, B _cdr, C _cpr, D _ctr)
        {
            car = _car;
            cdr = _cdr;
            cpr = _cpr;
            ctr = _ctr;
        }

        public static A Car(Tuple<A, B, C, D> tup)
        {
            return tup.car;
        }

        public static B Cdr(Tuple<A, B, C, D> tup)
        {
            return tup.cdr;
        }

        public static C Cpr(Tuple<A, B, C, D> tup)
        {
            return tup.cpr;
        }

        public static D Ctr(Tuple<A, B, C, D> tup)
        {
            return tup.ctr;
        }

        public override bool Equals(object obj)
        {
            Tuple<A, B, C, D> b = (Tuple<A, B, C, D>)obj;
            return car.Equals(b.car) && cdr.Equals(b.cdr) && cpr.Equals(b.cpr) && ctr.Equals(b.ctr);
            
        }
        public override int GetHashCode()
        {
            var _car = EqualityComparer<A>.Default.Equals(car, default(A)) ? 0 : car.GetHashCode();
            var _cdr = EqualityComparer<B>.Default.Equals(cdr, default(B)) ? 0 : cdr.GetHashCode();
            var _cpr = EqualityComparer<C>.Default.Equals(cpr, default(C)) ? 0 : cdr.GetHashCode();
            var _ctr = EqualityComparer<D>.Default.Equals(ctr, default(D)) ? 0 : ctr.GetHashCode();
            return ((_ctr * 31 + _cpr) * 31 + _car) * 31 + _cdr;
        }
        public override string ToString()
        {
            return "(" + car.ToString() + ", " + cdr.ToString() + ", " + cpr.ToString() + ", " + ctr.ToString() + ")";
        }

        public static Tuple<A, B, C, D> _(A car, B cdr, C cpr, D ctr)
        {
            var fst_hash = Tuple<A, B>._(car, cdr).GetHashCode();
            var snd_hash = Tuple<C, D>._(cpr, ctr).GetHashCode();

            if (Tuple.tup4 == null) Tuple.tup4 = new Dictionary<int, Dictionary<int, List<object>>>();
            if (!Tuple.tup4.ContainsKey(fst_hash)) Tuple.tup4[fst_hash] = new Dictionary<int, List<object>>();
            if (!Tuple.tup4[fst_hash].ContainsKey(snd_hash)) Tuple.tup4[fst_hash][snd_hash] = new List<object>();

            foreach (var item in Tuple.tup4[fst_hash][snd_hash])
            {
                if (item is Tuple<A, B, C, D>)
                {
                    var _item = (Tuple<A, B, C, D>)item;
                    if (EqualityComparer<A>.Default.Equals(_item.car, car)
                        && EqualityComparer<B>.Default.Equals(_item.cdr, cdr)
                        && EqualityComparer<C>.Default.Equals(_item.cpr, cpr)
                        && EqualityComparer<D>.Default.Equals(_item.ctr, ctr))
                    {
                        return _item;
                    }
                }
            }

            var n_tup = new Tuple<A, B, C, D>(car, cdr, cpr, ctr);
            Tuple.tup4[fst_hash][snd_hash].Add(n_tup);
            return n_tup;
        }

        public static void Clean()
        {
            Tuple.tup4.Clear();
        }
    }
}
