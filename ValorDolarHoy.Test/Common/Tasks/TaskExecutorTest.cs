using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ValorDolarHoy.Controllers;
using Xunit;

namespace ValorDolarHoy.Test.Common.Tasks;

public class TaskExecutorTest
{
    [Fact]
    public async Task ExecuteAsync_From_ObservableAsync()
    {
        IActionResult actionResult = await TaskExecutor.ExecuteAsync(Observable.Return("hello_world!"));
        
        Assert.NotNull(actionResult);
        Assert.IsType<OkObjectResult>(actionResult);
        
        OkObjectResult okObjectResult = (OkObjectResult)actionResult;
        Assert.Equal(200, okObjectResult.StatusCode);
        Assert.Equal("hello_world!", okObjectResult.Value);
    }
}