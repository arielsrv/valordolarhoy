namespace ValorDolarHoy.Services
{
    public class BluelyticsResponse
    {
        public Oficial oficial;
        public Blue blue;

        public class Oficial
        {
            public decimal valueSell { get; init; }
            public decimal valueBuy { get; init; }
        }

        public class Blue
        {
            public decimal valueSell { get; init; }
            public decimal valueBuy { get; init; }
        }
    }
}