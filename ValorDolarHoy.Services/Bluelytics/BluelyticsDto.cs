namespace ValorDolarHoy.Services
{
    public class BluelyticsDto
    {
        public BlueDto blue { get; init; }

        public class BlueDto
        {
            public decimal sell { get; init; }
            public decimal buy { get; init; }
        }
    }
}