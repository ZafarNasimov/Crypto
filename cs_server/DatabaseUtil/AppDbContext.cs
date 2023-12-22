using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Cryptocurrency> ServerAssets { get; set; }
    public DbSet<UserPortfolio> AccPortfolio { get; set; }
    public DbSet<UserOffer> UserOffers { get; set; }
    public DbSet<Miner> Miners { get; set; }
    public DbSet<Block> Blocks { get; set; }
    public DbSet<Admin> Admins { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql("Server=localhost;Port=3306;Database=Crypto;User Id=root;Password=TAE_spring_60;", ServerVersion.AutoDetect("Server=localhost;Port=3306;Database=Crypto;User Id=root;Password=TAE_spring_60;"));
    }
}
