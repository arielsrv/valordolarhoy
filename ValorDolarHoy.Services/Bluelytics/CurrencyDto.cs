namespace ValorDolarHoy.Services
{
    public class CurrencyDto
    {
        public OficialDto Official { get; init; }
        public BlueDto Blue { get; init; }

        public class OficialDto
        {
            public decimal Sell { get; init; }
            public decimal Buy { get; init; }
        }
        public class BlueDto
        {
            public decimal Sell { get; init; }
            public decimal Buy { get; init; }
        }
    }
}