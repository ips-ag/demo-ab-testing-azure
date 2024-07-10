using IPSAG.AbTesting.Monads;
using JetBrains.Annotations;
using static System.ArgumentNullException;

namespace IPSAG.AbTesting.Extensions;

[PublicAPI]
public static class EitherExtensions
{
    [Pure]
    [NotNull]
    public static Maybe<TR> ToMaybe<TL, TR>([NotNull] this Either<TL, TR> either)
    {
        ThrowIfNull(either, nameof(either));
        return either.Match(Maybe<TR>.Some, left => Maybe<TR>.None);
    }

    [Pure]
    [NotNull]
    public static Either<TLeft, TResult> SelectMany<TLeft, TRight, TResult, TArgs>(
        [NotNull] this Either<TLeft, TRight> source,
        [NotNull] Func<TArgs, Either<TLeft, TRight>> func,
        [NotNull] Func<TRight, TRight, Either<TLeft, TResult>> selector,
        [NotNull] TArgs arg)
    {
        ThrowIfNull(source, nameof(source));
        ThrowIfNull(func, nameof(func));
        ThrowIfNull(selector, nameof(selector));
        ThrowIfNull(arg, nameof(arg));

        var result = source.Match(
            right =>
            {
                return func.Invoke(arg).Match(
                    nestedRight => selector(right, nestedRight),
                    Either<TLeft, TResult>.Left);
            },
            Either<TLeft, TResult>.Left);

        return result;
    }
}
