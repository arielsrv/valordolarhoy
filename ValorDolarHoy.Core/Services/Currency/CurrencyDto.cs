namespace ValorDolarHoy.Core.Services.Currency;

/// <summary>
///     Currency
/// </summary>
public class CurrencyDto
{
    /// <summary>
    ///     Official
    /// </summary>
    public OficialDto? Official { get; init; }

    /// <summary>
    ///     Blue
    /// </summary>
    public BlueDto? Blue { get; init; }
}