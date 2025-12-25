namespace Contracts.Entities;

public record Product
{
    public required string Id { get; init; }      // SKU or product identifier
    public required string Name { get; init; }    // For display/logging
    public required int Quantity { get; init; }          // How many units
    public required decimal UnitPrice { get; init; }     // Price per unit
}