using elearn_server.Domain.Entities;

namespace Elearn_server.src.Domain.Entities;

public class Event : BaseEntity
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string ImageUrl { get; set; }
    public string Url { get; set; }
    public string Status { get; set; }
}

