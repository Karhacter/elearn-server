using elearn_server.Application.Common;
using elearn_server.Application.Responses;

namespace elearn_server.Application.Interfaces;

public interface IInvoiceService
{
    Task<ServiceResult<IReadOnlyCollection<InvoiceResponse>>> GetMyInvoicesAsync(int userId);
    Task<ServiceResult<InvoiceResponse>> GetInvoiceAsync(int userId, int invoiceId);
    Task<ServiceResult<string>> DownloadInvoiceHtmlAsync(int userId, int invoiceId);
}
