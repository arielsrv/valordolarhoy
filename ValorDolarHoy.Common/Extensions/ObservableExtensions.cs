namespace System.Reactive.Linq
{
    public static class ObservableExtensions
    {
        public static TSource ToBlockingFirst<TSource>(this IObservable<TSource> source)
        {
            return source.Wait();
        }
    }
}