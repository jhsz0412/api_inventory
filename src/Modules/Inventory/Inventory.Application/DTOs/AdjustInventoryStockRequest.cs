
namespace Inventory.Application.DTOs;

public sealed record AdjustInventoryStockRequest(
    decimal NewQuantity,
    string Remarks);
