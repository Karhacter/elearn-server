using System;
using System.ComponentModel.DataAnnotations;

namespace elearn_server.Domain.Entities
{
    public class BlacklistedToken
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }
    }
}
