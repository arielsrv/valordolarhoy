This project was bootstrapped with [Rx.Net](https://github.com/dotnet/reactive).

[![.NET](https://github.com/arielsrv/valordolarhoy/actions/workflows/dotnet.yml/badge.svg)](https://github.com/arielsrv/valordolarhoy/actions/workflows/dotnet.yml)
[![CodeQL](https://github.com/arielsrv/valordolarhoy/actions/workflows/codeql-analysis.yml/badge.svg)](https://github.com/arielsrv/valordolarhoy/actions/workflows/codeql-analysis.yml)
![badge](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/arielsrv/85040afe50e9da55b30ca5e32a437743/raw/code-coverage.json)

[Demo](https://valordolarhoy.herokuapp.com/)

## Emitting objects and fire and forget action

This is an simple example where there are two observables and fire a forget in another thread.

Service
```csharp
public IObservable<CurrencyDto> GetFallback()
{
    string cacheKey = GetCacheKey
    return this.keyValueStore.Get<CurrencyDto>(cacheKey).FlatMap(currencyDto =>
    {
        return currencyDto != null
            ? Observable.Return(currencyDto)
            : this.GetFromApi().Map(response =>
            {
                this.executorService.Run(() =>
                    this.keyValueStore.Put(cacheKey, response, 60 * 10).Wait()); // mm * ss
                return response;
            });
    });
}
```

Controller
```csharp
[HttpGet]
public async Task<IActionResult> GetLatestAsync()
{
    return await this.QueryAsync(this.currencyService.GetFallback());
}
```

## Request

    curl 'https://valordolarhoy.herokuapp.com/fallback'
```json
{
  "official": {
    "sell": 107.57,
    "buy": 101.57
  },
  "blue": {
    "sell": 200,
    "buy": 196
  }
}
```
