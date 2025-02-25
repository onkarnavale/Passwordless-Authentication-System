using Fido2Apis.Domain.Fido2Auth;
using Fido2Apis.Domain.User;
using Microsoft.EntityFrameworkCore;

namespace Fido2Apis.Infra.Db
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Fido2Credential> fido2_credentials { get; set; }
        public DbSet<User> users { get; set; }
    }
}
