namespace OrderManagement.Api.Constants;

public static class OrderStatus
{
    public const string Pending = "Pending";
    public const string Processing = "Processing";
    public const string Completed = "Completed";
    public const string Cancelled = "Cancelled";

    public static readonly string[] All =
    [
        Pending,
        Processing,
        Completed,
        Cancelled
    ];

    public static bool IsValid(string? status) =>
        !string.IsNullOrWhiteSpace(status) && All.Contains(status);
}
