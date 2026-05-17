# Valor Dolar Hoy 💰

[![.NET](https://github.com/arielsrv/valordolarhoy/actions/workflows/dotnet.yml/badge.svg)](https://github.com/arielsrv/valordolarhoy/actions/workflows/dotnet.yml)
![Code Coverage](https://img.shields.io/endpoint?url=https://gist.githubusercontent.com/arielsrv/85040afe50e9da55b30ca5e32a437743/raw/code-coverage.json)
![License](https://img.shields.io/badge/license-MIT-blue.svg)
![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)

> REST API to get the current dollar value in Argentina (official and blue) with reactive architecture using Rx.NET

## 🌟 Demo

**Live Demo:** [https://valordolarhoy.herokuapp.com/](https://valordolarhoy.herokuapp.com/)

## 📋 Features

- **🔄 Reactive Architecture**: Uses Rx.NET for asynchronous and reactive handling
- **💾 Smart Caching**: In-memory caching system for performance optimization
- **🛡️ Resilience**: Implements Polly for retry policies and circuit breakers
- **📊 Monitoring**: Health checks and automatic warmup
- **🧪 Complete Testing**: Unit tests, integration tests and code coverage
- **🚀 Performance**: Optimized for high concurrency
- **📱 React Frontend**: Modern interface with React and TypeScript

## 🏗️ Architecture

### Technology Stack

- **Backend**: ASP.NET Core 9.0, C# 13
- **Frontend**: React 18, TypeScript
- **Reactive Programming**: Rx.NET
- **Caching**: Memory Cache
- **Testing**: xUnit, Moq
- **Deployment**: Docker, Heroku

### Design Patterns

- **Reactive Programming**: Observable patterns with Rx.NET
- **Repository Pattern**: For data access
- **Dependency Injection**: Native IoC container
- **Circuit Breaker**: For resilience in external calls
- **Caching Strategy**: Multi-layer caching

## 🚀 Installation

### Prerequisites

- .NET 9.0 SDK
- Node.js 18+ (for frontend)

### Local Development

1. **Clone the repository**
```bash
git clone https://github.com/arielsrv/valordolarhoy.git
cd valordolarhoy
```

2. **Restore dependencies**
```bash
dotnet restore
```

3. **Configure environment variables**
```bash
# Create appsettings.Development.json or use environment variables
export ASPNETCORE_ENVIRONMENT=Development
```

4. **Run the application**
```bash
dotnet run --project ValorDolarHoy
```

5. **Frontend (optional)**
```bash
cd ValorDolarHoy/ClientApp
npm install
npm start
```

### Docker

```bash
docker build -t valordolarhoy .
docker run -p 5000:5000 valordolarhoy
```

## 📚 Usage

### API Endpoints

#### Get Current Exchange Rate
```bash
curl https://valordolarhoy.herokuapp.com/Currency
```


#### Health Check
```bash
curl https://valordolarhoy.herokuapp.com/Ping
```

### Responses

#### ✅ 200 - Successful Exchange Rate
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

#### ❌ 404 - Resource Not Found
```json
{
  "code": 404,
  "type": "ApiNotFoundException",
  "message": "Not Found",
  "detail": "The requested resource does not exist"
}
```

#### ❌ 500 - Internal Error
```json
{
  "code": 500,
  "type": "ApiException",
  "message": "An internal server error has occurred",
  "detail": "Please try again later"
}
```

## 🔧 Development

### Project Structure

```
ValorDolarHoy/
├── ValorDolarHoy.Core/          # Main backend
│   ├── Controllers/             # API Controllers
│   ├── Services/                # Business Logic
│   ├── Clients/                 # External API clients
│   ├── Common/                  # Shared utilities
│   └── Middlewares/             # Custom middlewares
├── ValorDolarHoy/              # Web application
│   └── ClientApp/              # React frontend
└── ValorDolarHoy.Test/         # Test projects
    ├── Unit/                   # Unit tests
    └── Integration/            # Integration tests
```

### Code Examples

#### Reactive Controller
```csharp
[HttpGet]
public async Task<IActionResult> GetLatestAsync()
{
    return await TaskExecutor.ExecuteAsync(this.currencyService.GetLatest());
}
```

#### Service with In-Memory Cache
```csharp
public IObservable<CurrencyDto> GetLatest()
{
    var cacheKey = GetCacheKey();

    CurrencyDto? currencyDto = this.AppCache.GetIfPresent(cacheKey);

    return currencyDto != null
        ? Observable.Return(currencyDto)
        : this.GetFromApi().Map(response =>
        {
            this._executorService.Run(() => this.AppCache.Put(cacheKey, response));
            return response;
        });
}
```

#### Configured HTTP Client
```csharp
services.AddHttpClient<ICurrencyClient, CurrencyClient>()
    .SetTimeout(TimeSpan.FromMilliseconds(1500))
    .SetMaxConnectionsPerServer(20)
    .SetMaxParallelization(20);
```

### Testing

#### Unit Tests
```bash
dotnet test ValorDolarHoy.Test/Unit
```

#### Integration Tests
```bash
dotnet test ValorDolarHoy.Test/Integration
```

#### Code Coverage
```bash
./coverage.sh
```

## 🧪 Testing

### Unit Test Example
```csharp
[Fact]
public void Get_Latest_Ok_From_Cache()
{
    this._appCache.Setup(client => client.GetIfPresent("bluelytics:v1")).Returns(GetFromCache());

    CurrencyService currencyService = new(this._currencyClient.Object, this._mapper)
    {
        AppCache = this._appCache.Object
    };

    CurrencyDto currencyDto = currencyService.GetLatest().ToBlocking();

    Assert.NotNull(currencyDto);
    Assert.Equal(10.0M, currencyDto.Official!.Buy);
    Assert.Equal(11.0M, currencyDto.Official.Sell);
    Assert.Equal(12.0M, currencyDto.Blue!.Buy);
    Assert.Equal(13.0M, currencyDto.Blue.Sell);
}
```

### Integration Test Example
```csharp
[Fact]
public async Task Basic_Integration_Test_InternalServerErrorAsync()
{
    this.currencyService.Setup(service => service.GetLatest())
        .Returns(Observable.Throw<CurrencyDto>(new ApiException()));

    HttpResponseMessage httpResponseMessage = await this.httpClient.GetAsync("/Currency");
    string responseString = await httpResponseMessage.Content.ReadAsStringAsync();
    
    ErrorHandlerMiddleware.ErrorModel? errorModel = JsonConvert
        .DeserializeObject<ErrorHandlerMiddleware.ErrorModel>(responseString);

    Assert.NotNull(errorModel);
    Assert.Equal(500, errorModel.Code);
    Assert.Equal(nameof(ApiException), errorModel.Type);
}
```

## 🔄 CI/CD

The project includes GitHub Actions for:

- ✅ Automatic build
- 🧪 Test execution
- 📊 Code coverage reporting
- 🚀 Automatic deployment to Heroku

## 🤝 Contributing

1. Fork the project
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Contribution Guidelines

- Follow C# and .NET conventions
- Add tests for new features
- Maintain high code coverage
- Document important changes

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🙏 Acknowledgments

- [Rx.NET](https://github.com/dotnet/reactive) - Reactive Extensions for .NET
- [AutoMapper](https://automapper.org/) - Object mapping
- [Polly](https://github.com/App-vNext/Polly) - Resilience and transient-fault-handling

## 📞 Contact

- **Author**: Ariel Servin
- **GitHub**: [@arielsrv](https://github.com/arielsrv)
- **Demo**: [https://valordolarhoy.herokuapp.com/](https://valordolarhoy.herokuapp.com/)

---

⭐ If this project helps you, give it a star!
