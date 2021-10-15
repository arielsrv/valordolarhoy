namespace ValorDolarHoy.Services
{
    public class BluelyticsDto
    {
        public OficialDto oficial { get; init; }
        public BlueDto blue { get; init; }

        public class OficialDto
        {
            public decimal sell { get; init; }
            public decimal buy { get; init; }
        }
        public class BlueDto
        {
            public decimal sell { get; init; }
            public decimal buy { get; init; }
        }
    }
}