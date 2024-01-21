using Microsoft.EntityFrameworkCore;
namespace HiveSync.Data;

public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
}
