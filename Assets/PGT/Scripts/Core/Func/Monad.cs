
namespace PGT.Core.Func {

    public abstract class Monad<T> {
        protected abstract T Return();
        protected abstract Monad<U> Bind<U>();

    }

}