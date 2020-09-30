using System;

namespace Domain.Models
{
    public class Entity
    {
        public long Id { get; protected set; }
        public DateTimeOffset CreatedDate { get; protected set; } = DateTimeOffset.UtcNow;
    }
}
