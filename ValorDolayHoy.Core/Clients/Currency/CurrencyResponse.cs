namespace ValorDolayHoy.Core.Clients.Currency;

public class CurrencyResponse
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