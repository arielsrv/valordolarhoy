using System.Threading.Tasks;

namespace ValorDolarHoy.Services.Clients
{
    public interface IBluelyticsClient
    {
        Task<BluelyticsResponse> GetLatest();
    }
}