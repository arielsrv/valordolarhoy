namespace ValorDolarHoy.Services
{
    public class BluelyticsResponse
    {
        public Blue blue;

        public class Blue
        {
            public decimal valueSell { get; init; }
            public decimal valueBuy { get; init; }
        }
    }
}