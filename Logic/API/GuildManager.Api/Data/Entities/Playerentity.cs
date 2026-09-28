using System;
using System.Collections.Generic;

namespace GuildManager.Api.Data.Entities;

public class PlayerEntity
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }

    public List<GuildMembershipEntity> Memberships { get; set; } = new();
}