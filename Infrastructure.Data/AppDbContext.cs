using Domain.Entities;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Asset> Assets { get; set; }
    public DbSet<Trade> Trades { get; set; }
    public DbSet<Quote> Quotes { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Dividend> Dividends { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().Property(x => x.FeePercentage).HasPrecision(5, 2);
        modelBuilder.Entity<Trade>().Property(x => x.UnitPrice).HasPrecision(15, 4);
        modelBuilder.Entity<Trade>().Property(x => x.Fee).HasPrecision(15, 4);
        modelBuilder.Entity<Quote>().Property(x => x.UnitPrice).HasPrecision(15, 4);
        modelBuilder.Entity<Position>().Property(x => x.AvgPrice).HasPrecision(15, 4);
        modelBuilder.Entity<Position>().Property(x => x.PnL).HasPrecision(15, 4);
        modelBuilder.Entity<Dividend>().Property(x => x.ValuePerShare).HasPrecision(15, 4);

        modelBuilder.Entity<User>().Property(u => u.Name).IsRequired();
        modelBuilder.Entity<User>().Property(u => u.Email).IsRequired();
        modelBuilder.Entity<Asset>().Property(a => a.Code).IsRequired();
        modelBuilder.Entity<Asset>().Property(a => a.Name).IsRequired();
        modelBuilder.Entity<Trade>().Property(t => t.TradeType).IsRequired();

        base.OnModelCreating(modelBuilder);

        // Configuração de chaves e relacionamentos
        modelBuilder.Entity<User>().HasMany(u => u.Trades).WithOne(t => t.User).HasForeignKey(t => t.UserId);
        modelBuilder.Entity<User>().HasMany(u => u.Positions).WithOne(p => p.User).HasForeignKey(p => p.UserId);

        modelBuilder.Entity<Asset>().HasMany(a => a.Trades).WithOne(t => t.Asset).HasForeignKey(t => t.AssetId);
        modelBuilder.Entity<Asset>().HasMany(a => a.Quotes).WithOne(q => q.Asset).HasForeignKey(q => q.AssetId);
        modelBuilder.Entity<Asset>().HasMany(a => a.Positions).WithOne(p => p.Asset).HasForeignKey(p => p.AssetId);
        modelBuilder.Entity<Asset>().HasMany(a => a.Dividends).WithOne(d => d.Asset).HasForeignKey(d => d.AssetId);

        // Índices sugeridos
        modelBuilder.Entity<Trade>().HasIndex(t => new { t.UserId, t.AssetId, t.TradeTime });
        modelBuilder.Entity<Quote>().HasIndex(q => new { q.AssetId, q.QuoteTime });
        modelBuilder.Entity<Trade>().HasIndex(t => t.UserId); // Para total de corretagem
    }
}