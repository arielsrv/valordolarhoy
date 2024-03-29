This project was bootstrapped with [Rx.Net](https://github.com/dotnet/reactive).

[![.NET](https://github.com/arielsrv/valordolarhoy/actions/workflows/dotnet.yml/badge.svg)](https://github.com/arielsrv/valordolarhoy/actions/workflows/dotnet.yml)
![badge](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/arielsrv/85040afe50e9da55b30ca5e32a437743/raw/code-coverage.json)

[Demo](https://valordolarhoy.herokuapp.com/)

## Emitting objects and fire and forget action

This is an simple example where there are two observables and fire a forget in another thread.

### Controller

```csharp
[HttpGet]
public async Task<IActionResult> GetLatestAsync()
{
    return await TaskExecutor.ExecuteAsync(this.currencyService.GetLatest());
}
```

### Service

```csharp
public IObservable<CurrencyDto> GetFallback()
{
    string cacheKey = GetCacheKey();
    
    return this.keyValueStore.Get<CurrencyDto>(cacheKey).FlatMap(currencyDto =>
    {
        return currencyDto != null
            ? Observable.Return(currencyDto)
            : this.GetFromApi().Map(response =>
            {
                this.executorService.Run(() =>
                    this.keyValueStore.Put(cacheKey, response, 60 * 10).ToBlocking()); // mm * ss
                return response;
            });
    });
}
```

### Clients (beta)

```csharp
services.AddHttpClient<ICurrencyClient, CurrencyClient>()
    .SetTimeout(TimeSpan.FromMilliseconds(1500))
    .SetMaxConnectionsPerServer(20)
    .SetMaxParallelization(20);
```

### Warmup

#### Enabler
```csharp
public static void UseWarmUp(this IApplicationBuilder applicationBuilder)
    {
        Task.Run(() =>
            {
                IApplicationWarmUpper? applicationWarmUpper =
                    applicationBuilder.ApplicationServices.GetService<IApplicationWarmUpper>();

                if (applicationWarmUpper == null) return;

                WarmupExecutor.Warmup(applicationWarmUpper);
            }
        ).Forget();
    }
```

#### Custom func
```csharp 
public bool Warmup()
    {
        Thread.Sleep(1000);

        for (int i = 0; i < 3; i++)
        {
            try
            {
                if (string.IsNullOrEmpty(this.baseUrl)) return true;

                string requestUri = $"{this.baseUrl}/Currency";
                HttpRequestMessage httpRequestMessage = new(HttpMethod.Get, requestUri);
                HttpResponseMessage httpResponseMessage = this.httpClient.Send(httpRequestMessage);

                if (httpResponseMessage.StatusCode == HttpStatusCode.OK) return true;
            }
            catch (Exception)
            {
                // ignored
            }

            Thread.Sleep(1000);
        }

        return false;
    }
```
#### Ping

```csharp
public ActionResult<string> Pong()
    {
        return WarmupExecutor.Initialized
            ? this.StatusCode(StatusCodes.Status200OK, "pong")
            : this.StatusCode(StatusCodes.Status503ServiceUnavailable, "offline");
    }
```

### Unit Test

```csharp
[Fact]
public void Get_Latest_Ok_Fallback_FromApi()
{
    this.keyValueStore.Setup(store => store.Get<CurrencyDto>("bluelytics:v1"))
        .Returns(Observable.Return(default(CurrencyDto)));
    this.currencyClient.Setup(client => client.Get()).Returns(GetLatest());
    
    CurrencyService currencyService = new(this.currencyClient.Object, this.keyValueStore.Object);
    
    CurrencyDto currencyDto = currencyService.GetFallback().ToBlocking();
    
    Assert.NotNull(currencyDto);
    Assert.Equal(10.0M, currencyDto.Official!.Buy);
    Assert.Equal(11.0M, currencyDto.Official.Sell);
    Assert.Equal(12.0M, currencyDto.Blue!.Buy);
    Assert.Equal(13.0M, currencyDto.Blue.Sell);
}
```

### Integration Test

```csharp
[Fact]
public async Task Basic_Integration_Test_InternalServerErrorAsync()
{
    this.currencyService.Setup(service => service.GetLatest())
        .Returns(Observable.Throw<CurrencyDto>(new ApiException()));

    HttpResponseMessage httpResponseMessage = await this.httpClient.GetAsync("/Currency");
    string responseString = await httpResponseMessage.Content.ReadAsStringAsync();
    Assert.NotNull(responseString);

    ErrorHandlerMiddleware.ErrorModel? errorModel = JsonConvert
        .DeserializeObject<ErrorHandlerMiddleware.ErrorModel>(responseString);

    Assert.NotNull(errorModel);
    Assert.Equal(500, errorModel.Code);
    Assert.Equal(nameof(ApiException), errorModel.Type);
    Assert.NotNull(errorModel.Detail);
}
```

## Request

    curl 'https://valordolarhoy.herokuapp.com/fallback'

## Responses

### 200

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

### 404

```json
{
  "code": 404,
  "type": "ApiNotFoundException",
  "message": "{\"message\":\"Not Found\"}",
  "detail": "   at ValorDolarHoy.Core.Common..."
}
```

### 500

```json
{
  "code": 500,
  "type": "ApiException",
  "message": "An internal server error has occurred. ",
  "detail": "Please try again later"
}
```
