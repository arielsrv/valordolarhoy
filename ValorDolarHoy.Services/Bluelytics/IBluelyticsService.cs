using System.Threading.Tasks;

namespace ValorDolarHoy.Services
{
    public interface IBluelyticsService
    {
        Task<BluelyticsDto> GetLatest();
    }
}