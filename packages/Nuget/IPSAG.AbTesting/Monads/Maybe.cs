using JetBrains.Annotations;
using static System.ArgumentNullException;

namespace IPSAG.AbTesting.Monads
{
    [PublicAPI]
    public sealed class Maybe<T>
    {
        internal bool HasItem { get; }
        internal T? Item { get; }
        internal Exception? Exception { get; }

        [NotNull]
        public static Maybe<T> None => new();

        [NotNull]
        public static Maybe<T> Error(Exception exception) => new(exception);

        [Pure]
        [NotNull]
        public static Maybe<T> Some(T value)
        {
            return new Maybe<T>(value);
        }

        private Maybe(Exception? exception = null)
        {
            HasItem = false;
            Exception = exception;
        }

        private Maybe(T item)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            HasItem = true;
            Item = item;
        }

        [Pure]
        [NotNull]
        public Maybe<TResult> Select<TResult>(
            [NotNull] Func<T, TResult> selector)
        {
            ThrowIfNull(selector);

            return HasItem ? new Maybe<TResult>(selector(Item!)) : new Maybe<TResult>();
        }

        [Pure]
        [ItemNotNull]
        public async Task<Maybe<TResult>> SelectAsync<TResult>(
            [NotNull] Func<T, Task<TResult>> selector)
        {
            ThrowIfNull(selector);

            return HasItem ? new Maybe<TResult>(
                await selector(Item!).ConfigureAwait(false)) : new Maybe<TResult>();
        }

        [Pure]
        [NotNull]
        public Maybe<TResult> SelectMany<TResult>(
            [NotNull] Func<T, Maybe<TResult>> selector)
        {
            ThrowIfNull(selector);

            return HasItem ? selector(Item!) : new Maybe<TResult>();
        }

        [Pure]
        [ItemNotNull]
        public async Task<Maybe<TResult>> SelectManyAsync<TResult>(
            [NotNull] Func<T, Task<Maybe<TResult>>> selector)
        {
            ThrowIfNull(selector);

            return HasItem ? await selector(Item!).ConfigureAwait(false) : new Maybe<TResult>();
        }

        [Pure]
        [NotNull]
        public TResult MatchResult<TResult>(
            [NotNull] TResult nothing,
            [NotNull] Func<T, TResult> just)
        {
            if (nothing == null) throw new ArgumentNullException(nameof(nothing));
            ThrowIfNull(just);

            return HasItem ? just(Item!) : nothing;
        }

        [Pure]
        [ItemNotNull]
        public async Task<TResult> MatchResultAsync<TResult>(
            [NotNull] TResult nothing,
            [NotNull] Func<T, Task<TResult>> just)
        {
            if (nothing == null) throw new ArgumentNullException(nameof(nothing));
            ThrowIfNull(just);

            return HasItem ? await just(Item!).ConfigureAwait(false) : nothing;
        }

        [Pure]
        [NotNull]
        public TResult MatchResult<TResult>(
            [NotNull] TResult nothing,
            [NotNull] TResult just)
        {
            if (nothing == null) throw new ArgumentNullException(nameof(nothing));
            if (just == null) throw new ArgumentNullException(nameof(just));

            return HasItem ? just : nothing;
        }

        [Pure]
        [NotNull]
        public Func<TResult> Bind<TResult>(
            [NotNull] Func<T, TResult> func,
            [NotNull] T arg)
        {
            ThrowIfNull(func);
            ThrowIfNull(arg);

            return () => func(arg);
        }

        public void Match(
            [NotNull] Action<T> just,
            [NotNull] Action nothing)
        {
            ThrowIfNull(just);
            ThrowIfNull(nothing);

            if (HasItem)
            {
                just.Invoke(Item!);
            }
            else
            {
                nothing.Invoke();
            }
        }

        public async Task MatchAsync(
            [NotNull] Func<Task> just,
            [NotNull] Action nothing)
        {
            ThrowIfNull(just);
            ThrowIfNull(nothing);

            if (HasItem)
            {
                await just.Invoke().ConfigureAwait(false);
            }
            else
            {
                nothing.Invoke();
            }
        }

        public async Task MatchAsync(
            [NotNull] Func<T, Task> just,
            [NotNull] Action nothing)
        {
            ThrowIfNull(just);
            ThrowIfNull(nothing);

            if (HasItem)
            {
                await just.Invoke(Item!).ConfigureAwait(false);
            }
            else
            {
                nothing.Invoke();
            }
        }

        [Pure]
        [ItemNotNull]
        public async Task<TResult> MatchAsync<TResult>(
            [NotNull] Func<T, Task<TResult>> just,
            [NotNull] Func<Task<TResult>> nothing)
        {
            ThrowIfNull(just);
            ThrowIfNull(nothing);

            if (HasItem)
            {
                return await just.Invoke(Item!).ConfigureAwait(false);
            }

            return await nothing.Invoke().ConfigureAwait(false);
        }

        public void Match(
            [NotNull] Action just,
            [NotNull] Action nothing)
        {
            ThrowIfNull(just);
            ThrowIfNull(nothing);

            if (HasItem)
            {
                just.Invoke();
            }
            else
            {
                nothing.Invoke();
            }
        }

        [Pure]
        [NotNull]
        public TResult Match<TResult>(
            [NotNull] Func<T, TResult> onJust,
            [NotNull] Func<TResult> onNothing)
        {
            ThrowIfNull(onJust);
            ThrowIfNull(onNothing);

            return HasItem ? onJust(Item!) : onNothing();
        }

        [Pure]
        [NotNull]
        public TResult Match<TResult>(
            [NotNull] Func<T, TResult> onJust,
            [NotNull] Func<Exception, TResult> onNothing)
        {
            ThrowIfNull(onJust);
            ThrowIfNull(onNothing);

            return HasItem ? onJust(Item!) : onNothing(Exception!);
        }

        [Pure]
        public override bool Equals([CanBeNull] object? obj)
        {
            if (obj is not Maybe<T> other)
                return false;

            return Equals(Item, other.Item);
        }


        [Pure]
        public override int GetHashCode()
        {
            return HasItem ? Item!.GetHashCode() : 0;
        }
    }
}
