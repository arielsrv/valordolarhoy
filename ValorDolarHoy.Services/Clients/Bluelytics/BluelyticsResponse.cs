namespace ValorDolarHoy.Services
{
    public class BluelyticsResponse
    {
        public Blue blue;
        public Oficial oficial;

        public class Oficial
        {
            public decimal ValueSell { get; init; }
            public decimal ValueBuy { get; init; }
        }

        public class Blue
        {
            public decimal ValueSell { get; init; }
            public decimal ValueBuy { get; init; }
        }
    }       
}