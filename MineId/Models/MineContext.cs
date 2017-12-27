using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MineId.Models {
    class MineContext : IdentityDbContext<MineUser> {
        public MineContext (DbContextOptions<MineContext> options) : base (options) {

        }

        DbSet<MineApplication> MineApplications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<MineUserApplication>()
                .HasKey(mua => new { mua.MineApplicationId, mua.MineUserId });

            builder.Entity<MineUser>()
                .HasMany(mu => mu.ApplicationRoot)
                .WithOne(ma => ma.RootUser)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}