using Microsoft.EntityFrameworkCore;
using FlightBookingSystem.Models;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Admin> Admins { get; set; }
    public DbSet<RewardAccount> RewardAccounts { get; set; }
    public DbSet<PointsTransaction> PointsTransactions { get; set; }
    public DbSet<Airport> Airports { get; set; }
    public DbSet<Flight> Flights { get; set; }
    public DbSet<SeatClass> SeatClasses { get; set; }
    public DbSet<FlightSeatClass> FlightSeatClasses { get; set; }
    public DbSet<PriceRule> PriceRules { get; set; }
    public DbSet<FlightRule> FlightRules { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Passenger> Passengers { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.Entity<FlightSeatClass>()
            .HasKey(f => new { f.FlightId, f.ClassId });

        modelBuilder.Entity<FlightRule>()
            .HasKey(f => new { f.FlightId, f.RuleId });

        
        modelBuilder.Entity<Flight>()
            .HasOne(f => f.DepartureAirport)
            .WithMany(a => a.DepartureFlights)
            .HasForeignKey(f => f.DepartureAirportId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Flight>()
            .HasOne(f => f.ArrivalAirport)
            .WithMany(a => a.ArrivalFlights)
            .HasForeignKey(f => f.ArrivalAirportId)
            .OnDelete(DeleteBehavior.Restrict);

        
        modelBuilder.Entity<Passenger>()
            .HasOne(p => p.FlightSeatClass)
            .WithMany(f => f.Passengers)
            .HasForeignKey(p => new { p.FlightId, p.ClassId });

       
        modelBuilder.Entity<Admin>()
            .HasOne(a => a.User)
            .WithOne(u => u.Admin)
            .HasForeignKey<Admin>(a => a.UserId);

       
        modelBuilder.Entity<RewardAccount>()
            .HasOne(r => r.User)
            .WithOne(u => u.RewardAccount)
            .HasForeignKey<RewardAccount>(r => r.UserId);

        
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Booking)
            .WithMany(b => b.Tickets)
            .HasForeignKey(t => t.BookingId)
            .OnDelete(DeleteBehavior.Restrict);

        
        modelBuilder.Entity<Ticket>()
            .HasOne(t => t.Passenger)
            .WithOne(p => p.Ticket)
            .HasForeignKey<Ticket>(t => t.PassengerId)
            .OnDelete(DeleteBehavior.Restrict);

        
        modelBuilder.Entity<Flight>()
            .Property(f => f.BasePrice).HasPrecision(10, 2);
        modelBuilder.Entity<FlightSeatClass>()
            .Property(f => f.FinalPrice).HasPrecision(10, 2);
        modelBuilder.Entity<PriceRule>()
            .Property(p => p.Multiplier).HasPrecision(5, 2);
        modelBuilder.Entity<Booking>()
            .Property(b => b.TotalPrice).HasPrecision(10, 2);
        modelBuilder.Entity<Payment>()
            .Property(p => p.Amount).HasPrecision(10, 2);
        modelBuilder.Entity<SeatClass>()
            .Property(s => s.ClassMultiplier).HasPrecision(5, 2);
    }
}