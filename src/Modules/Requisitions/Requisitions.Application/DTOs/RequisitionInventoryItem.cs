namespace Requisitions.Application.DTOs;

public sealed record RequisitionInventoryItem(
    string ItemCode,
    string Name,
    bool IsActive);
