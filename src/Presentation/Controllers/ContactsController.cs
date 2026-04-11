using elearn_server.Application.Interfaces;
using elearn_server.Application.Requests;
using Microsoft.AspNetCore.Mvc;

namespace elearn_server.Presentation.Controllers;

[Route("api/contacts")]
public class ContactsController : ApiControllerBase
{
    private readonly IContactService _contactService;

    public ContactsController(IContactService contactService)
    {
        _contactService = contactService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllContacts()
    {
        return FromResult(await _contactService.GetAllContactsAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetContactById(int id)
    {
        return FromResult(await _contactService.GetContactByIdAsync(id));
    }

    [HttpPost]
    public async Task<IActionResult> CreateContact([FromBody] CreateContactRequest request)
    {
        return FromResult(await _contactService.CreateContactAsync(request));
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateContact(int id, [FromBody] UpdateContactRequest request)
    {
        return FromResult(await _contactService.UpdateContactAsync(id, request));
    }

    [HttpPatch("{id}/toggle-soft-delete")]
    public async Task<IActionResult> ToggleSoftDelete(int id)
    {
        return FromResult(await _contactService.ToggleSoftDeleteAsync(id));
    }
}
