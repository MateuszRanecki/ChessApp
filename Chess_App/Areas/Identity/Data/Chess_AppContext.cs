using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Chess_App.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Chess_App.Data
{
    public class Chess_AppContext : IdentityDbContext<Chess_AppUser>
    {
        public Chess_AppContext(DbContextOptions<Chess_AppContext> options)
            : base(options)
        {
        }

        public DbSet<Chess_GameHistory> GameHistory { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
    }
}
