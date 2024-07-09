using JetBrains.Annotations;
using static System.ArgumentNullException;

namespace IPSAG.AbTesting.Monads
{
    [PublicAPI]
    public sealed class Either<TL, TR>
    {
        private readonly TL? LeftItem;

        private readonly TR? RightItem;

        private readonly bool IsLeft;

        [Pure]
        [NotNull]
        public static Either<TL, TR> Left(
            [NotNull] TL value)
        {
            ThrowIfNull(value);

            return new Either<TL, TR>(value);
        }

        [Pure]
        [NotNull]
        public static Either<TL, TR> Right(
            [NotNull] TR value)
        {
            ThrowIfNull(value);

            return new Either<TL, TR>(value);
        }

        private Either(TL left)
        {
            LeftItem = left;
            IsLeft = true;
        }

        private Either(TR right)
        {
            RightItem = right;
            IsLeft = false;
        }

        [Pure]
        [NotNull]
        public Either<TL, TR> Select(
            [NotNull] Func<TR, Either<TL, TR>> selector)
        {
            ThrowIfNull(selector);

            return IsLeft ? Left(LeftItem!) : selector(RightItem!);
        }

        [Pure]
        [NotNull]
        public Either<TL, TResult> Select<TResult>([NotNull] Func<TR, TResult> selector)
        {
            ThrowIfNull(selector);

            return IsLeft ? Either<TL, TResult>.Left(LeftItem!) : Either<TL, TResult>.Right(selector(RightItem!));
        }

        [Pure]
        [ItemNotNull]
        public async Task<Either<TL, TResult>> SelectAsync<TResult>([NotNull] Func<TR, Task<TResult>> selector)
        {
            ThrowIfNull(selector);

            return !IsLeft ?
                Either<TL, TResult>.Right(
                    await selector(RightItem!).ConfigureAwait(false)) : Either<TL, TResult>.Left(LeftItem!);
        }

        [Pure]
        [NotNull]
        public Either<TL, VR> SelectMany<UR, VR>(
            [NotNull] Func<TR, Either<TL, UR>> selector,
            [NotNull] Func<TR, UR, VR> projector)
        {
            ThrowIfNull(selector);
            ThrowIfNull(projector);

            if (IsLeft)
                return Either<TL, VR>.Left(LeftItem!);

            var res = selector(RightItem!);

            return res.IsLeft ? Either<TL, VR>.Left(res.LeftItem!) : Either<TL, VR>.Right(projector(RightItem!, res.RightItem!));
        }

        [Pure]
        [NotNull]
        public Either<TL, TR> SelectMany(
            [NotNull] Func<TR, Either<TL, TR>> selector)
        {
            ThrowIfNull(selector);

            return IsLeft ? Left(LeftItem!) : selector(RightItem!);
        }

        [Pure]
        [NotNull]
        public Either<TL, TR> SelectMany(
            [NotNull] Func<Either<TL, TR>> selector)
        {
            ThrowIfNull(selector);

            return IsLeft ? Left(LeftItem!) : selector();
        }

        [Pure]
        [ItemNotNull]
        public async Task<Either<TL, TR>> SelectManyAsync(
            [NotNull] Func<Task<Either<TL, TR>>> selector)
        {
            ThrowIfNull(selector);

            return IsLeft ? Left(LeftItem!) : await selector().ConfigureAwait(false);
        }

        [Pure]
        [ItemNotNull]
        public async Task<Either<TL, TR>> SelectManyAsync(
            [NotNull] Func<TR, Task<Either<TL, TR>>> selector)
        {
            ThrowIfNull(selector);

            return IsLeft ? Left(LeftItem!) : await selector(RightItem!).ConfigureAwait(false);
        }

        [Pure]
        [NotNull]
        public TResult Match<TResult>(
            [NotNull] Func<TResult> onRight,
            [NotNull] Func<TResult> onLeft)
        {
            ThrowIfNull(onRight);
            ThrowIfNull(onLeft);

            return IsLeft ? onLeft() : onRight();
        }

        [Pure]
        [NotNull]
        public TResult Match<TResult>(
            [NotNull] Func<TR, TResult> onRight,
            [NotNull] Func<TL, TResult> onLeft)
        {
            ThrowIfNull(onRight);
            ThrowIfNull(onLeft);

            return IsLeft ? onLeft(LeftItem!) : onRight(RightItem!);
        }

        public void Match(
            [NotNull] Action<TR> right,
            [NotNull] Action left)
        {
            ThrowIfNull(right);
            ThrowIfNull(left);

            if (IsLeft)
            {
                left.Invoke();
            }
            else
            {
                right.Invoke(RightItem!);
            }
        }

        public void Match(
            [NotNull] Action<TR> right,
            [NotNull] Action<TL> left)
        {
            ThrowIfNull(right);
            ThrowIfNull(left);

            if (IsLeft)
            {
                left.Invoke(LeftItem!);
            }
            else
            {
                right.Invoke(RightItem!);
            }
        }

        public async Task MatchAsync(
            [NotNull] Func<TR, Task> right,
            [NotNull] Action<TL> left)
        {
            ThrowIfNull(right);
            ThrowIfNull(left);

            if (IsLeft) left.Invoke(LeftItem!);
            else await right(RightItem!).ConfigureAwait(false);
        }

        public void Match(
            [NotNull] Action right,
            [NotNull] Action left)
        {
            ThrowIfNull(right);
            ThrowIfNull(left);

            if (IsLeft)
            {
                left.Invoke();
            }
            else
            {
                right.Invoke();
            }
        }

        [Pure]
        [NotNull]
        public TResult MatchResult<TResult>([NotNull] TResult right, [NotNull] TResult left)
        {
            ThrowIfNull(right);
            ThrowIfNull(left);

            return IsLeft ? left : right;
        }

        [Pure]
        [ItemNotNull]
        public async Task<TResult> MatchResultAsync<TResult>(
            [NotNull] Func<TR, Task<TResult>> right,
            [NotNull] Func<TL, Task<TResult>> left)
        {
            ThrowIfNull(right);
            ThrowIfNull(left);

            return IsLeft ? await left(LeftItem!).ConfigureAwait(false) :
                await right(RightItem!).ConfigureAwait(false);
        }

        [Pure]
        [NotNull]
        public Func<TResult> Bind<T, TResult>([NotNull] Func<T, TResult> func, [NotNull] T arg)
        {
            ThrowIfNull(func);
            ThrowIfNull(arg);

            return () => func(arg);
        }

    }
}
