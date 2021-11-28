namespace System.Reactive.Linq
{
    public static class ObservableExtensions
    {
        public static IObservable<TResult> Map<TSource, TResult>(this IObservable<TSource> source,
            Func<TSource, TResult> selector)
        {
            return source.Select(selector);
        }
    }
}