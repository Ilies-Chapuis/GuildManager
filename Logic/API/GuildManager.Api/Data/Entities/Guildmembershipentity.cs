using System;

namespace GuildManager.Api.Data.Entities;

// Composite key (PlayerId + GuildId) is configured in the DbContext,
// not here -- EF Core needs Fluent API for composite keys.
public class GuildMembershipEntity
{
    public int PlayerId { get; set; }
    public int GuildId { get; set; }
    public DateTime JoinedAt { get; set; }

    public PlayerEntity? Player { get; set; }
    public GuildEntity? Guild { get; set; }
}