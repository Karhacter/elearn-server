namespace elearn_server.Domain.Enums;

public enum OrderStatus
{
    Pending = 0,    // Chờ xử lý
    Processing = 1, // Đang xử lý
    Completed = 2,  // Đã hoàn thành
    Cancelled = 3   // Đã hủy
}