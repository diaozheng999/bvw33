namespace PGT.Core.Func
{
    using System;
    using UnityEngine;
    
    public delegate void Lambda();
    
    public class TupledFunction<A, B, C>
    {
        public Func<A, B, C> _f;

        public TupledFunction(Func<A, B, C> f)
        {
            _f = f;
        }
        
        public C tcall(Tuple<A, B> tup)
        {
            return _f(tup.car, tup.cdr);
        }

        public static implicit operator Func<Tuple<A, B>, C>(TupledFunction<A, B, C> f)
        {
            return f.tcall;
        }
    }

    public class TupledFunction<A, B, C, D>
    {
        public Func<A, B, C, D> _f;

        public TupledFunction(Func<A, B, C, D> f)
        {
            _f = f;
        }

        public D tcall(Tuple<A, B, C> tup)
        {
            return _f(tup.car, tup.cdr, tup.cpr);
        }

        public static implicit operator Func<Tuple<A, B, C>, D>(TupledFunction<A, B, C, D> f)
        {
            return f.tcall;
        }
    }

    public class TupledFunction<A, B, C, D, E>
    {
        public Func<A, B, C, D, E> _f;

        public TupledFunction(Func<A, B, C, D, E> f)
        {
            _f = f;
        }

        public E tcall(Tuple<A, B, C, D> tup)
        {
            return _f(tup.car, tup.cdr, tup.cpr, tup.ctr);
        }

        public static implicit operator Func<Tuple<A, B, C, D>, E>(TupledFunction<A, B, C, D, E> f)
        {
            return f.tcall;
        }
    }
    

    public class DelayedFunction<A, B>
    {
        public Func<A, B> _f;
        A _param;

        public DelayedFunction(Func<A, B> f, A param)
        {
            _f = f;
            _param = param;
        }

        private B bind()
        {
            return _f(_param);
        }

        private void bind_ignore()
        {
            _f(_param);
        }

        public static implicit operator Func<B>(DelayedFunction<A, B> f)
        {
            return f.bind;
        }

        public static implicit operator Action(DelayedFunction<A, B> f)
        {
            return f.bind_ignore;
        }

        public static implicit operator Lambda(DelayedFunction<A, B> f)
        {
            return f.bind_ignore;
        }
    }

    public class CurriedFunction<A, B, C> : PartiallyAppliedCurriedFunction<A, B, C>
    {
        bool is_tup = false;
        TupledFunction<A, B, C> tupf;
        public CurriedFunction(Func<A, B, C> f): base(f) { }

        public CurriedFunction(TupledFunction<A, B, C> f) : base(f._f)
        {
            is_tup = true;
            tupf = f;
        }

        private PartiallyAppliedCurriedFunction<A, B, C> apply1(A param1)
        {
            this.param1 = param1;
            return this;
        }

        public TupledFunction<A, B, C> uncurry()
        {
            if (is_tup) return tupf;
            else return new TupledFunction<A, B, C>(_f);
        }

        public Func<A, B, C> uncurry1()
        {
            return _f;
        }

        public static implicit operator Func<A, PartiallyAppliedCurriedFunction<A, B, C>>(CurriedFunction<A, B, C> f)
        {
            return f.apply1;
        }

        public static explicit operator Func<A, Func<B, C>>(CurriedFunction<A, B, C> f)
        {
            return (A param1) => f.apply1(param1);
        }
    }

    public class PartiallyAppliedCurriedFunction<A, B, C> 
    {
        public Func<A, B, C> _f;
        protected A param1;
        protected PartiallyAppliedCurriedFunction(Func<A, B, C> f)
        {
            _f = f;
        }
        
        protected C apply(B param2) {
            return _f(param1, param2);
        }

        public static implicit operator Func<B, C>(PartiallyAppliedCurriedFunction<A, B, C> f)
        {
            return f.apply;
        }
    }

    public class UnitFunction
    {
        public Action f;

        public UnitFunction(Action f)
        {
            this.f = f;
        }

        public UnitFunction(Lambda f)
        {
            this.f = () => f();
        }

        private unit ret()
        {
            f();
            return null;
        }

        public static implicit operator Func<unit>(UnitFunction f)
        {
            return f.ret;
        }
    }

    public class UnitFunction<A>
    {
        public Action<A> f;

        public UnitFunction(Action<A> f)
        {
            this.f = f;
        }

        private unit ret(A param)
        {
            f(param);
            return null;
        }

        public static implicit operator Func<A, unit>(UnitFunction<A> f)
        {
            return f.ret;
        }
    }

    

    public class UnitFunction<A, B>
    {
        public Action<A, B> f;
        public Action<Tuple<A, B>> ftup;
        bool istup = false;

        public UnitFunction(Action<A, B> f)
        {
            this.f = f;
        }

        public UnitFunction(Action<Tuple<A, B>> f)
        {
            istup = true;
            ftup = f;
        }

        private unit ret(A param1, B param2)
        {
            if (istup)
            {
                ftup(Tuple._(param1, param2));
                return null;
            }
            f(param1, param2);
            return null;
        }

        public unit rett(Tuple<A, B> tupapply)
        {
            if (istup)
            {
                ftup(tupapply);
                return null;
            }
            f(tupapply.car, tupapply.cdr);
            return null;
        }

        public static implicit operator TupledFunction<A, B, unit>(UnitFunction<A, B> f)
        {
            return new TupledFunction<A, B, unit>(f.ret);
        }

        public static implicit operator Func<Tuple<A, B>, unit>(UnitFunction<A, B> f)
        {
            return f.rett;
        }
    }

    public static class Function
    {
        public static void noop() { }
        public static T id<T>(T x) { return x; }

        public static Func<A, C> o<A, B, C>(Func<A, B> f, Func<B, C> g)
        {
            return (A x) => g(f(x));
        }

        public static Func<A, B, D> o<A, B, C, D>(Func<A, B, C> f, Func<C, D> g)
        {
            return (A x, B y) => g(f(x, y));
        }


        public static CurriedFunction<A, B, C> curry<A, B, C>(Func<A, B, C> f)
        {
            return new CurriedFunction<A, B, C>(f);
        }

        public static Func<A, B, C> uncurry<A, B, C>(CurriedFunction<A, B, C> f)
        {
            return f._f;
        }

        public static Func<A, B, C> uncurry<A, B, C>(Func<A, Func<B, C>> f)
        {
            return (A a, B b) => f(a)(b);
        }

        public static TupledFunction<A, B, C> _tup<A, B, C>(Func<A, B, C> f)
        {
            return new TupledFunction<A, B, C>(f);
        }

        public static Func<A, B, C> _untup<A, B, C>(TupledFunction<A, B, C> f) => f._f;
        public static Func<A, B, C> _untup<A, B, C>(Func<Tuple<A, B>, C> f) => (A a, B b) => f(Tuple._(a, b));

        public static Func<Tuple<A, B, C>, D> _tup<A, B, C, D>(Func<A, B, C, D> f)
        {
            return (Tuple<A, B, C> tup) => f(tup.car, tup.cdr, tup.cpr);
        }
        public static Func<A, B, C, D> _untup<A, B, C, D>(Func<Tuple<A, B, C>, D> f) => (A a, B b, C c) => f(Tuple._(a, b, c));
        public static Func<Tuple<A, B, C, D>, E> _tup<A, B, C, D, E>(Func<A, B, C, D, E> f)
        {
            return (Tuple<A, B, C, D> tup) => f(tup.car, tup.cdr, tup.cpr, tup.ctr);
        }

        public static Func<A, B, C, D, E> _untup<A, B, C, D, E>(Func<Tuple<A, B, C, D>, E> f) => (A a, B b, C c, D d) => f(Tuple._(a, b, c, d));

        public static UnitFunction cast(Action f) { return new UnitFunction(f); }
        public static UnitFunction<A> cast<A>(Action<A> f) { return new UnitFunction<A>(f); }
        public static UnitFunction<A, B> cast<A,B>(Action<A, B> f) { return new UnitFunction<A, B>(f); }

        public static DelayedFunction<A, unit> delay<A>(Action<A> f, A param) { return new DelayedFunction<A, unit>(cast(f), param); }
        public static DelayedFunction<Tuple<A, B>, unit> delay<A, B>(Action<A, B> f, Tuple<A, B> param) {
            return new DelayedFunction<Tuple<A, B>, unit>(cast(f), param);
        }

        public static DelayedFunction<A, B> delay<A, B>(Func<A, B> f, A param) { return new DelayedFunction<A, B>(f, param); }
        public static DelayedFunction<Tuple<A, B>, C> delay<A, B, C>(Func<A, B, C> f, Tuple<A, B> param) { return new DelayedFunction<Tuple<A, B>, C>(_tup(f), param); }
        public static DelayedFunction<Tuple<A, B>, C> delay<A, B, C>(TupledFunction<A, B, C> f, Tuple<A, B> param) { return new DelayedFunction<Tuple<A, B>, C>(f, param); }
        public static DelayedFunction<Tuple<A, B>, C> delay<A, B, C>(Func<Tuple<A, B>, C> f, Tuple<A, B> param) { return new DelayedFunction<Tuple<A, B>, C>(f, param); }

        public static int iadd(int a, int b) { return a + b; }
        public static int imul(int a, int b) { return a * b; }
        public static int isub(int a, int b) { return a - b; }
        public static int idiv(int a, int b) { return a / b; }
        public static int imod(int a, int b) { return a % b; }
        public static int iand(int a, int b) { return a & b; }
        public static int ior (int a, int b) { return a | b; }
        public static int ixor(int a, int b) { return a ^ b; }

        public static float fadd(float a, float b) { return a + b; }
        public static float fmul(float a, float b) { return a * b; }
        public static float fsub(float a, float b) { return a - b; }
        public static float fdiv(float a, float b) { return a / b; }
        public static float fdtu(float t1, float t2) { return (t2 - t1) / Time.deltaTime; }
        public static float fdtfu(float t1, float t2) { return (t2 - t1) / Time.fixedDeltaTime; }

        public static float tfadd(Tuple<float, float> tup) { return tup.car * tup.cdr; }
        public static float tfmul(Tuple<float, float> tup) { return tup.car * tup.cdr; }
        public static float tfsub(Tuple<float, float> tup) { return tup.car - tup.cdr; }
        public static float tfdiv(Tuple<float, float> tup) { return tup.car / tup.cdr; }
        public static float tfdtu(Tuple<float, float> tup) { return (tup.cdr - tup.car) / Time.deltaTime; }
        public static float tfdtfu(Tuple<float, float> tup) { return (tup.cdr - tup.car) / Time.fixedDeltaTime; }

        public static Vector2 v2add(Vector2 a, Vector2 b) { return a + b; }
        public static Vector2 v2sub(Vector2 a, Vector2 b) { return a - b; }
        public static float v2sqd(Vector2 a, Vector2 b) { return Vector2.SqrMagnitude(a - b); }
        public static Vector2 v2dtu(Vector2 t1, Vector2 t2) { return (t2 - t1) / Time.deltaTime; }
        public static Vector2 v2dtfu(Vector2 t1, Vector2 t2) { return (t2 - t1) / Time.fixedDeltaTime; }

        public static Vector3 v3add(Vector3 a, Vector3 b) { return a + b; }
        public static Vector3 v3sub(Vector3 a, Vector3 b) { return a - b; }
        public static float v3sqd(Vector3 a, Vector3 b) { return Vector3.SqrMagnitude(a - b); }
        public static Vector3 v3dtu(Vector3 t1, Vector3 t2) { return (t2 - t1) / Time.deltaTime; }
        public static Vector3 v3dtfu(Vector3 t1, Vector3 t2) { return (t2 - t1) / Time.fixedDeltaTime; }


        public static C Invoke<A, B, C>(TupledFunction<A, B, C> f, A p1, B p2)
        {
            return f._f(p1, p2);
        }

        public static C Invoke<A, B, C>(CurriedFunction<A, B, C> f, A p1, B p2)
        {
            return f._f(p1, p2);
        }

        public static C Invoke<A, B, C>(Func<Tuple<A, B>, C> f, A p1, B p2)
        {
            return f(Tuple._(p1, p2));
        }

        public static C Invoke<A, B, C>(Func<A, Func<B, C>> f, A p1, B p2)
        {
            return f(p1)(p2);
        }

        public static C tcall<A, B, C>(Func<A, B, C> f, Tuple<A, B> tup)
        {
            return f(tup.car, tup.cdr);
        }

        public static D tcall <A, B, C, D>(Func<A, B, C, D> f, Tuple<A, B, C> tup)
        {
            return f(tup.car, tup.cdr, tup.cpr);
        }

        public static E tcall <A, B, C, D, E>(Func<A, B, C, D, E> f, Tuple<A, B, C, D> tup)
        {
            return f(tup.car, tup.cdr, tup.cpr, tup.ctr);
        }
    }

    public class Future
    {
        Lambda _f;
        Action<Lambda> _app;
        public Future(Lambda f, Action<Lambda> app)
        {
            _f = f;
            _app = app;
        }
        public Future(Lambda f)
        {
            _f = f;
            _app = null;
        }

        public void bind()
        {
            if (_app != null)
                _app.Invoke(_f);
            else
                UnityExecutionThread.instance.ExecuteInMainThread(_f);
        }

        public unit bind1()
        {
            bind();
            return null;
        }

        public void bind(Action<Lambda> app)
        {
            app.Invoke(_f);
        }

        public void bind(ExecutionOrder order)
        {
            UnityExecutionThread.instance.ExecuteInMainThread(_f, order);
        }
        public void bind(string order)
        {
            switch (order)
            {
                case "update":
                    bind(ExecutionOrder.Update);
                    return;
                case "fixedUpdate":
                    bind(ExecutionOrder.FixedUpdate);
                    return;
                case "coroutine":
                    bind(ExecutionOrder.Coroutine);
                    return;
                case "lateUpdate":
                    bind(ExecutionOrder.LateUpdate);
                    return;
                default:
                    bind(ExecutionOrder.Any);
                    return;
            }
        }

        internal void Invoke()
        {
            _f.Invoke();
        }

        public Future join(Future other)
        {
            return new Future(() =>
            {
                _f.Invoke();
                other.Invoke();
            });
        }

        public Future<T> join<T>(Future<T> other)
        {
            return new Future<T>(() =>
            {
                _f.Invoke();
                return other.Invoke();
            });
        }

        public override string ToString()
        {
            return "<Future (void) with function " + _f.ToString() + ">";
        }

        static void _id() { }

        public static Future id
        {
            get
            {
                return new Future(_id);
            }
        }

        
        public static implicit operator Action(Future f)
        {
            return f.bind;
        }

        public static implicit operator Lambda(Future f)
        {
            return f.bind;
        }

        public static implicit operator Func<unit>(Future f)
        {
            return f.bind1;
        }

        public static implicit operator Action<ExecutionOrder>(Future f)
        {
            return f.bind;
        }
    }

    public class Future<T>
    {
        protected Func<T> _f;
        protected Func<Func<T>, T> _app;

        public Future(Func<T> f, Func<Func<T>, T> app)
        {
            _f = f;
            _app = app;
        }

        public Future(Func<T> f)
        {
            _f = f;
            _app = null;
        }

        public T bind()
        {
            if (_app != null)
                return _app.Invoke(_f);
            
            else
                return UnityExecutionThread.instance.ExecuteInMainThread<T>(_f);
        }

        public Future<U> applyTo<U>(Continuation<T,U> cont)
        {
            return cont.apply(this);
        }

        public Future<U> applyTo<U>(Func<T,U> cont)
        {
            return (new Continuation<T,U>(cont)).apply(this);
        }

        public Future applyTo(Continuation<T> cont)
        {
            return cont.apply(this);
        }

        public Future applyTo(Action<T> cont)
        {
            return (new Continuation<T>(cont)).apply(this);
        }

        public Future<T> join(Future f)
        {
            return new Future<T>(() =>
            {
                T result = _f.Invoke();
                f.Invoke();
                return result;
            });
        }

        public Future<Tuple<T, U>> join<U>(Future<U> other)
        {
            return new Future<Tuple<T, U>>(
                () => Tuple._(_f.Invoke(), other.Invoke())
                );
        }

        public T bind(ExecutionOrder order)
        {
            return UnityExecutionThread.instance.ExecuteInMainThread<T>(_f, order);
        }

        public T bind(string order)
        {
            switch (order)
            {
                case "update":
                    return bind(ExecutionOrder.Update);
                case "fixedUpdate":
                    return bind(ExecutionOrder.FixedUpdate);
                case "coroutine":
                    return bind(ExecutionOrder.Coroutine);
                case "lateUpdate":
                    return bind(ExecutionOrder.LateUpdate);
                default:
                    return bind(ExecutionOrder.Any);
            }
        }

        public T bind(Func<Func<T>, T> app)
        {
            return app.Invoke(_f);
        }

        internal T Invoke()
        {
            return _f.Invoke();
        }

        public override string ToString()
        {
            return "<Future (" + typeof(T).Name + ") with function " + _f.ToString() + ">";
        }

        public static implicit operator Future<T>(Func<T> f)
        {
            return new Future<T>(f);
        }

        static T _id() { return default(T); }

        public static Future<T> id
        {
            get
            {
                return new Future<T>(_id);
            }
        }

        public static implicit operator Func<T>(Future<T> f)
        {
            return f.bind;
        }
    }

    public class Continuation<S, T>
    {
        Func<S, T> _f;

        public Continuation(Func<S,T> cont)
        {
            _f = cont;
        }

        public Future<T> apply(S param)
        {
            return new Future<T>(() => _f.Invoke(param));
        }

        public Future<T> apply(Future<S> param)
        {
            return new Future<T>(() =>
            {
                return _f.Invoke(param.Invoke());
            });
        }

        public T applybind(S param)
        {
            return apply(param).bind();
        }

        public Continuation<S, U> chain<U>(Continuation<T, U> c2)
        {
            return new Continuation<S, U>((S p) => c2._f(_f(p)));
        }

        public Continuation<S> chain(Continuation<T> c2)
        {
            return new Continuation<S>((S p) => c2._f(_f(p)));
        }

        public static implicit operator Func<S, T>(Continuation<S, T> c)
        {
            return c.applybind;
        }
    }

    public class Continuation<S>
    {
        internal Action<S> _f;

        public Continuation(Action<S> cont)
        {
            _f = cont;
        }
        public Future apply(S param)
        {
            return new Future(() => _f.Invoke(param));
        }

        public unit applybind1 (S param)
        {
            return apply(param).bind1();
        }

        public void applybind (S param)
        {
            apply(param).bind();
        }

        public Future apply(Future<S> param)
        {
            return new Future(() =>
            {
                _f.Invoke(param.Invoke());
            });
        }

        public static Continuation<object> Box<T>(Continuation<T> c)
        {
            return new Continuation<object>((object i) => c._f((T)i));
        }

        public static implicit operator Action<S>(Continuation<S> c)
        {
            return c.applybind;
        }

        public static implicit operator Func<S, unit>(Continuation<S> c)
        {
            return c.applybind1;
        }
    }
}