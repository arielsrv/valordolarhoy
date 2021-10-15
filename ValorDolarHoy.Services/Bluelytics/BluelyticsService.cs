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
                official = new BluelyticsDto.OficialDto
                {
                    buy = bluelyticsResponse.oficial.valueBuy,
                    sell = bluelyticsResponse.oficial.valueSell
                },
                blue = new BluelyticsDto.BlueDto
                {
                    buy = bluelyticsResponse.blue.valueBuy,
                    sell = bluelyticsResponse.blue.valueSell
                }
            };

            return bluelyticsDto;
        }
    }
}