using elearn_server.Application.Common;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;

namespace elearn_server.Application.Interfaces;

public interface IContactService
{
    Task<ServiceResult<List<ContactResponse>>> GetAllContactsAsync();
    Task<ServiceResult<ContactResponse>> GetContactByIdAsync(int id);
    Task<ServiceResult<ContactResponse>> CreateContactAsync(CreateContactRequest request);
    Task<ServiceResult<ContactResponse>> UpdateContactAsync(int id, UpdateContactRequest request);
    Task<ServiceResult<bool>> ToggleSoftDeleteAsync(int id);
}
