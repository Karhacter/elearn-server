namespace elearn_server.Application.Responses;

public class OrderDetailResponse
{
    public int Id { get; set; }
    public int OrderId { get; set; }
    public int CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
}

public class OrderResponse
{
    public int OrderId { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public int UserId { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public IReadOnlyCollection<OrderDetailResponse> Items { get; set; } = Array.Empty<OrderDetailResponse>();
}

public class WishlistResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public DateTime AddedDate { get; set; }
}

public class PaymentResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int? OrderId { get; set; }
    public string? OrderCode { get; set; }
    public int? CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Gateway { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? GatewayTransactionNo { get; set; }
    public DateTime PaymentDate { get; set; }
    public DateTime? PaidAt { get; set; }
}

public class CheckoutResponse
{
    public int OrderId { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public int PaymentId { get; set; }
    public string PaymentUrl { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class PaymentHistoryResponse
{
    public int PaymentId { get; set; }
    public int OrderId { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string Gateway { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime PaymentDate { get; set; }
    public DateTime? PaidAt { get; set; }
    public IReadOnlyCollection<OrderDetailResponse> Items { get; set; } = Array.Empty<OrderDetailResponse>();
}

public class InvoiceResponse
{
    public int Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public int OrderId { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime IssuedAt { get; set; }
    public decimal TotalAmount { get; set; }
    public string DownloadUrl { get; set; } = string.Empty;
}

public class VnPayIpnResponse
{
    public string RspCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}

public class PaymentReturnResponse
{
    public string OrderCode { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ResponseCode { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class EnrollmentResponse
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int CourseId { get; set; }
    public string? CourseTitle { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public bool IsActive { get; set; }
}
