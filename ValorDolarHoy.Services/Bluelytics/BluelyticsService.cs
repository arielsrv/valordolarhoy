using System.Threading.Tasks;
using ValorDolarHoy.Services.Clients;

namespace ValorDolarHoy.Services
{
    public interface IBluelyticsService
    {
        Task<BluelyticsDto> GetLatest();
    }

    public class BluelyticsService : IBluelyticsService
    {
        private readonly IBluelyticsClient bluelyticsClient;

        public BluelyticsService(IBluelyticsClient bluelyticsClient)
        {
            this.bluelyticsClient = bluelyticsClient;
        }

        public async Task<BluelyticsDto> GetLatest()
        {
            BluelyticsResponse bluelyticsResponse = await this.bluelyticsClient.GetLatest();

            BluelyticsDto bluelyticsDto = new()
            {
                BlueDto = new BlueDto
                {
                    Buy = bluelyticsResponse.Blue.ValueBuy,
                    Sell = bluelyticsResponse.Blue.ValueSell
                }
            };

            return bluelyticsDto;
        }
    }
}