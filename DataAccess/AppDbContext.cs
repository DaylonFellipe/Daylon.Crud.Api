using Microsoft.EntityFrameworkCore;

namespace Daylon.Crud.Api.DataAccess
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Model.PersonModel> People { get; set; }



    }
}
