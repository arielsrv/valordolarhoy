namespace ValorDolarHoy.Services
{
    public class BluelyticsDto
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