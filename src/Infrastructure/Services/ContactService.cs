using elearn_server.Application.Common;
using elearn_server.Application.Interfaces;
using elearn_server.Application.Mappings;
using elearn_server.Application.Requests;
using elearn_server.Application.Responses;
using elearn_server.Domain.Entities;
using elearn_server.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace elearn_server.Infrastructure.Services;

public class ContactService : IContactService
{
    private readonly AppDbContext _context;

    public ContactService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<ServiceResult<List<ContactResponse>>> GetAllContactsAsync()
    {
        var contacts = await _context.Set<Contact>()
            .Where(c => !c.IsDeleted)
            .ToListAsync();
        return ServiceResult<List<ContactResponse>>.Ok(contacts.Select(c => c.ToResponse()).ToList());
    }

    public async Task<ServiceResult<ContactResponse>> GetContactByIdAsync(int id)
    {
        var contact = await _context.Set<Contact>().FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (contact == null) return ServiceResult<ContactResponse>.Fail(404, "Contact not found.");
        return ServiceResult<ContactResponse>.Ok(contact.ToResponse());
    }

    public async Task<ServiceResult<ContactResponse>> CreateContactAsync(CreateContactRequest request)
    {
        var contact = new Contact
        {
            FullName = request.FullName,
            Email = request.Email,
            Message = request.Message,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow
        };
        _context.Set<Contact>().Add(contact);
        await _context.SaveChangesAsync();
        return ServiceResult<ContactResponse>.Created(contact.ToResponse());
    }

    public async Task<ServiceResult<ContactResponse>> UpdateContactAsync(int id, UpdateContactRequest request)
    {
        var contact = await _context.Set<Contact>().FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
        if (contact == null) return ServiceResult<ContactResponse>.Fail(404, "Contact not found.");

        contact.Status = request.Status;
        contact.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();
        return ServiceResult<ContactResponse>.Ok(contact.ToResponse());
    }

    public async Task<ServiceResult<bool>> ToggleSoftDeleteAsync(int id)
    {
        var contact = await _context.Set<Contact>().FirstOrDefaultAsync(c => c.Id == id);
        if (contact == null) return ServiceResult<bool>.Fail(404, "Contact not found.");

        contact.IsDeleted = !contact.IsDeleted;
        contact.DeletedAt = contact.IsDeleted ? DateTime.UtcNow : null;
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true, "Contact soft delete toggled.");
    }
}
