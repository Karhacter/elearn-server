namespace elearn_server.Domain.Enums;

public enum PaymentStatus
{
    Unpaid = 0,     // Chưa thanh toán
    Paid = 1,       // Đã thanh toán
    Failed = 2,     // Thanh toán thất bại
    Refunded = 3    // Đã hoàn tiền
}