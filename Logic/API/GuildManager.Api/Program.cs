using Microsoft.EntityFrameworkCore;
using GuildManager.Api.Data.Entities;
using GuildManager.Api.Data;
using GuildManager.Api.Dtos;
using BCrypt.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<GuildManagerDBContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("GuildManagerDb"))
           .UseSnakeCaseNamingConvention());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/auth/register", async (RegisterRequestDto request, GuildManagerDBContext db) =>
{
    bool alreadyExists = await db.Players.AnyAsync(p =>
        p.Username == request.Username || p.Email == request.Email);

    if (alreadyExists)
        return Results.Conflict("Username or email already taken.");

    var player = new PlayerEntity
    {
        Username = request.Username,
        Email = request.Email,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
        CreatedAt = DateTime.UtcNow
    };

    db.Players.Add(player);
    await db.SaveChangesAsync();

    return Results.Ok(new { player.Id, player.Username, player.Email });
})
.WithName("Register");

app.MapPost("/auth/login", async (LoginRequestDto request, GuildManagerDBContext db) =>
{
    var player = await db.Players.FirstOrDefaultAsync(p => p.Username == request.Username);

    if (player is null || !BCrypt.Net.BCrypt.Verify(request.Password, player.PasswordHash))
        return Results.Unauthorized();

    return Results.Ok(new { player.Id, player.Username, player.Email });
})
.WithName("Login");

app.MapPost("/guilds", async (CreateGuildRequestDto request, GuildManagerDBContext db) =>
{
    var creator = await db.Players.FindAsync(request.PlayerId);
    if (creator is null)
        return Results.NotFound("Player not found.");

    var guild = new GuildEntity
    {
        Name = request.Name,
        CurrentDay = 1,
        RemainingHours = 16,
        Gold = 0,
        Food = 0,
        HealthPotions = 0,
        Ending = "None",
        CreatedAt = DateTime.UtcNow
    };

    guild.Memberships.Add(new GuildMembershipEntity
    {
        PlayerId = creator.Id,
        JoinedAt = DateTime.UtcNow
    });

    db.Guilds.Add(guild);
    await db.SaveChangesAsync();

    return Results.Ok(new GuildStateDto
    {
        Id = guild.Id,
        Name = guild.Name,
        CurrentDay = guild.CurrentDay,
        RemainingHours = guild.RemainingHours,
        Gold = guild.Gold,
        Food = guild.Food,
        HealthPotions = guild.HealthPotions,
        Ending = guild.Ending,
        MemberUsernames = new List<string> { creator.Username }
    });
})
.WithName("CreateGuild");

app.MapPost("/guilds/{guildId:int}/join", async (int guildId, JoinGuildRequestDto request, GuildManagerDBContext db) =>
{
    var guild = await db.Guilds
        .Include(g => g.Memberships).ThenInclude(m => m.Player)
        .FirstOrDefaultAsync(g => g.Id == guildId);
    if (guild is null)
        return Results.NotFound("Guild not found.");

    var player = await db.Players.FindAsync(request.PlayerId);
    if (player is null)
        return Results.NotFound("Player not found.");

    bool alreadyMember = guild.Memberships.Any(m => m.PlayerId == player.Id);
    if (alreadyMember)
        return Results.Conflict("Player is already a member of this guild.");

    var existingUsernames = guild.Memberships.Select(m => m.Player!.Username).ToList();

    guild.Memberships.Add(new GuildMembershipEntity
    {
        PlayerId = player.Id,
        JoinedAt = DateTime.UtcNow
    });
    await db.SaveChangesAsync();

    existingUsernames.Add(player.Username);

    return Results.Ok(new GuildStateDto
    {
        Id = guild.Id,
        Name = guild.Name,
        CurrentDay = guild.CurrentDay,
        RemainingHours = guild.RemainingHours,
        Gold = guild.Gold,
        Food = guild.Food,
        HealthPotions = guild.HealthPotions,
        Ending = guild.Ending,
        MemberUsernames = existingUsernames
    });
})
.WithName("JoinGuild");

app.MapGet("/guilds/{guildId:int}", async (int guildId, GuildManagerDBContext db) =>
{
    var guild = await db.Guilds
        .Include(g => g.Memberships).ThenInclude(m => m.Player)
        .FirstOrDefaultAsync(g => g.Id == guildId);

    if (guild is null)
        return Results.NotFound("Guild not found.");

    return Results.Ok(new GuildStateDto
    {
        Id = guild.Id,
        Name = guild.Name,
        CurrentDay = guild.CurrentDay,
        RemainingHours = guild.RemainingHours,
        Gold = guild.Gold,
        Food = guild.Food,
        HealthPotions = guild.HealthPotions,
        Ending = guild.Ending,
        MemberUsernames = guild.Memberships.Select(m => m.Player!.Username).ToList()
    });
})
.WithName("GetGuild");

// --- Adventurers ---

app.MapPost("/guilds/{guildId:int}/adventurers", async (int guildId, RecruitAdventurerRequestDto request, GuildManagerDBContext db) =>
{
    var guild = await db.Guilds.FindAsync(guildId);
    if (guild is null) return Results.NotFound("Guild not found.");

    var player = await db.Players.FindAsync(request.PlayerId);
    if (player is null) return Results.NotFound("Player not found.");

    if (request.Category == "Special")
    {
        bool alreadyExists = await db.Adventurers.AnyAsync(a =>
            a.GuildId == guildId && a.SpecialKey == request.SpecialKey);
        if (alreadyExists)
            return Results.Conflict($"{request.SpecialKey} already exists in this guild.");
    }

    (int maxHp, _) = AdventurerStatsCatalog.GetBaseStats(request.Category, request.SpecialKey);

    var adventurer = new AdventurerEntity
    {
        GuildId = guildId,
        OwnerPlayerId = request.PlayerId,
        Name = request.Name,
        Category = request.Category,
        SpecialKey = request.SpecialKey,
        CurrentHealthPoints = maxHp,
        Level = 1,
        Experience = 0,
        UniqueEventTriggered = request.Category == "Special" ? false : null,
        UsedToday = false,
        Status = "Available",
        CreatedAt = DateTime.UtcNow
    };

    db.Adventurers.Add(adventurer);
    await db.SaveChangesAsync();

    return Results.Ok(new AdventurerStateDto
    {
        Id = adventurer.Id,
        Name = adventurer.Name,
        Category = adventurer.Category,
        SpecialKey = adventurer.SpecialKey,
        CurrentHealthPoints = adventurer.CurrentHealthPoints,
        Level = adventurer.Level,
        Experience = adventurer.Experience,
        Status = adventurer.Status,
        UsedToday = adventurer.UsedToday,
        OwnerUsername = player.Username
    });
})
.WithName("RecruitAdventurer");

app.MapGet("/guilds/{guildId:int}/adventurers", async (int guildId, int? playerId, GuildManagerDBContext db) =>
{
    var query = db.Adventurers.Include(a => a.OwnerPlayer).Where(a => a.GuildId == guildId);
    if (playerId.HasValue)
        query = query.Where(a => a.OwnerPlayerId == playerId.Value);

    var adventurers = await query.ToListAsync();

    return Results.Ok(adventurers.Select(a => new AdventurerStateDto
    {
        Id = a.Id,
        Name = a.Name,
        Category = a.Category,
        SpecialKey = a.SpecialKey,
        CurrentHealthPoints = a.CurrentHealthPoints,
        Level = a.Level,
        Experience = a.Experience,
        Status = a.Status,
        UsedToday = a.UsedToday,
        OwnerUsername = a.OwnerPlayer!.Username
    }));
})
.WithName("ListAdventurers");

// --- Quests ---

app.MapPost("/guilds/{guildId:int}/quests/generate", async (int guildId, GuildManagerDBContext db) =>
{
    var guild = await db.Guilds.Include(g => g.Memberships).FirstOrDefaultAsync(g => g.Id == guildId);
    if (guild is null) return Results.NotFound("Guild not found.");

    var playerIds = guild.Memberships.Select(m => m.PlayerId).ToList();
    var quests = QuestGenerationService.GenerateForDay(guild, playerIds);

    db.Quests.AddRange(quests);
    await db.SaveChangesAsync();

    return Results.Ok(quests.Select(q => new QuestStateDto
    {
        Id = q.Id,
        Name = q.Name,
        Type = q.Type,
        Difficulty = q.Difficulty,
        DurationHours = q.DurationHours,
        GoldReward = q.GoldReward,
        MinimumTeamSize = q.MinimumTeamSize,
        MaximumTeamSize = q.MaximumTeamSize,
        RequiresAllRecruitCategories = q.RequiresAllRecruitCategories,
        RequiredClassName = q.RequiredClassName,
        DayAvailable = q.DayAvailable,
        IsResolved = q.IsResolved,
        WasSuccessful = q.WasSuccessful,
        OwnerPlayerId = q.OwnerPlayerId
    }));
})
.WithName("GenerateDailyQuests");

app.MapGet("/guilds/{guildId:int}/quests", async (int guildId, int? day, int? playerId, GuildManagerDBContext db) =>
{
    var query = db.Quests.Where(q => q.GuildId == guildId);
    if (day.HasValue) query = query.Where(q => q.DayAvailable == day.Value);
    if (playerId.HasValue) query = query.Where(q => q.OwnerPlayerId == playerId.Value || q.OwnerPlayerId == null);

    var quests = await query.ToListAsync();

    return Results.Ok(quests.Select(q => new QuestStateDto
    {
        Id = q.Id,
        Name = q.Name,
        Type = q.Type,
        Difficulty = q.Difficulty,
        DurationHours = q.DurationHours,
        GoldReward = q.GoldReward,
        MinimumTeamSize = q.MinimumTeamSize,
        MaximumTeamSize = q.MaximumTeamSize,
        RequiresAllRecruitCategories = q.RequiresAllRecruitCategories,
        RequiredClassName = q.RequiredClassName,
        DayAvailable = q.DayAvailable,
        IsResolved = q.IsResolved,
        WasSuccessful = q.WasSuccessful,
        OwnerPlayerId = q.OwnerPlayerId
    }));
})
.WithName("ListQuests");

app.MapPost("/quests/{questId:int}/attempt", async (int questId, AttemptQuestRequestDto request, GuildManagerDBContext db) =>
{
    var quest = await db.Quests.FindAsync(questId);
    if (quest is null) return Results.NotFound("Quest not found.");
    if (quest.IsResolved) return Results.Conflict("This quest has already been resolved.");

    var guild = await db.Guilds.FindAsync(quest.GuildId);
    if (guild is null) return Results.NotFound("Guild not found.");

    var team = await db.Adventurers
        .Where(a => request.AdventurerIds.Contains(a.Id) && a.GuildId == quest.GuildId)
        .ToListAsync();

    if (team.Count != request.AdventurerIds.Count)
        return Results.BadRequest("One or more adventurers were not found in this guild.");

    var result = QuestResolutionService.Attempt(guild, quest, team);
    await db.SaveChangesAsync();

    return Results.Ok(result);
})
.WithName("AttemptQuest");

// --- Day progression ---

app.MapPost("/guilds/{guildId:int}/advance-day", async (int guildId, GuildManagerDBContext db) =>
{
    var guild = await db.Guilds.Include(g => g.Adventurers).FirstOrDefaultAsync(g => g.Id == guildId);
    if (guild is null) return Results.NotFound("Guild not found.");

    if (guild.CurrentDay >= 10)
        return Results.BadRequest("The guild has already reached the final day.");

    guild.CurrentDay++;
    guild.RemainingHours = 16;

    foreach (var adventurer in guild.Adventurers.Where(a => a.Status != "Dead"))
        adventurer.UsedToday = false;

    await db.SaveChangesAsync();

    return Results.Ok(new { guild.Id, guild.CurrentDay, guild.RemainingHours });
})
.WithName("AdvanceDay");

app.MapPost("/guilds/{guildId:int}/evaluate-ending", async (int guildId, GuildManagerDBContext db) =>
{
    var guild = await db.Guilds.FindAsync(guildId);
    if (guild is null) return Results.NotFound("Guild not found.");

    if (guild.CurrentDay < 10)
    {
        guild.Ending = "None";
    }
    else
    {
        guild.Ending = guild.Gold >= 5000 ? "Good" : "Bad";
    }

    await db.SaveChangesAsync();

    return Results.Ok(new { guild.Id, guild.Ending, guild.Gold });
})
.WithName("EvaluateEnding");

app.Run();