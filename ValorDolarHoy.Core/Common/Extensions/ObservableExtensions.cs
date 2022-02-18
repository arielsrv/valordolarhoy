namespace System.Reactive.Linq;

public static class ObservableExtensions
{
    public static TSource ToBlocking<TSource>(this IObservable<TSource> source)
    {
        return source.Wait();
    }
}