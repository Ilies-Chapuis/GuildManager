using Microsoft.EntityFrameworkCore;
using GuildManager.Api.Data.Entities;

namespace GuildManager.Api.Data;

public class GuildManagerDBContext : DbContext
{
    public GuildManagerDBContext(DbContextOptions<GuildManagerDBContext> options)
        : base(options)
    {
    }

    public DbSet<PlayerEntity> Players => Set<PlayerEntity>();
    public DbSet<GuildEntity> Guilds => Set<GuildEntity>();
    public DbSet<GuildMembershipEntity> GuildMemberships => Set<GuildMembershipEntity>();
    public DbSet<AdventurerEntity> Adventurers => Set<AdventurerEntity>();
    public DbSet<QuestEntity> Quests => Set<QuestEntity>();
    public DbSet<QuestAssignmentEntity> QuestAssignments => Set<QuestAssignmentEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Composite primary keys (EF Core can't guess these on its own)
        modelBuilder.Entity<GuildMembershipEntity>()
            .HasKey(gm => new { gm.PlayerId, gm.GuildId });

        modelBuilder.Entity<QuestAssignmentEntity>()
            .HasKey(qa => new { qa.QuestId, qa.AdventurerId });

        base.OnModelCreating(modelBuilder);
    }
}