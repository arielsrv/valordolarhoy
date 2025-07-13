namespace System.Reactive.Linq;
#pragma warning disable CS1591

public static class ObservableExtensions
{
    public static TSource ToBlocking<TSource>(this IObservable<TSource> source)
    {
        return source.Wait();
    }
}