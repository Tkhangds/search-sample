using Eme_Search.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Eme_Search.Database;

public class SSDatabaseContext: DbContext
{
    public SSDatabaseContext(DbContextOptions<SSDatabaseContext> options)
        : base(options) { }

    public DbSet<BlacklistCategory> BlacklistCategories { get; set; }
    public DbSet<BlacklistBusiness> BlacklistBusinesses { get; set; }
}