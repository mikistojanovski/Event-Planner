using Diplomska.Areas.Identity.Data;
using Diplomska.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Diplomska.Data;

public class DiplomskaContext : IdentityDbContext<DiplomskaUser>
{
    public DiplomskaContext(DbContextOptions<DiplomskaContext> options)
        : base(options)
    {
    }


    public DbSet<Diplomska.Models.Guest> Guest { get; set; } = default!;

    public DbSet<Diplomska.Models.EventGuest>? EventGuest { get; set; }

    public DbSet<Diplomska.Models.Event>? Event { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);


        builder.Entity<Guest>().ToTable("Guest");
        builder.Entity<EventGuest>().ToTable("EventGuest");
        builder.Entity<Event>().ToTable("Event");


        builder.Entity<EventGuest>()
        .HasOne<Guest>(p => p.Guest)
        .WithMany(p => p.Events)
        .HasForeignKey(p => p.GuestId);
        //.HasPrincipalKey(p => p.Id);
        builder.Entity<EventGuest>()
        .HasOne<Event>(p => p.Event)
        .WithMany(p => p.Guests)
        .HasForeignKey(p => p.EventId);
        //.HasPrincipalKey(p => p.Id);
        builder.Entity<Event>()
        .HasOne<Guest>(p => p.Hoster)
        .WithMany(p => p.Hosting)
        .HasForeignKey(p => p.HostName);
        //.HasPrincipalKey(p => p.Id);
    }
}
