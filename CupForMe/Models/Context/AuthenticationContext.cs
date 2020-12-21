using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CupForMe.Models.Context
{
    public class AuthenticationContext : IdentityDbContext<UserIdentity, ApplicationRole, int>
    {
        public AuthenticationContext(DbContextOptions<AuthenticationContext> options) : base(options)
        { }

        public DbSet<UserIdentity> ApplicationUsers { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserIdentity>(user =>
            {
                user.ToTable(name: "User.AspNetUsers").HasMany(e => e.UserRoles)
                .WithOne(e => e.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();
            });

            builder.Entity<ApplicationRole>(userRole =>
            {
                userRole.ToTable(name: "User.AspNetRoles").HasMany(e => e.UserRoles)
                .WithOne(e => e.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();
            });

            builder.Entity<IdentityUserToken<int>>(b =>
            {
                b.ToTable(name: "user.AspNetUserTokens");
                b.HasKey(k => k.UserId);
                b.Property(t => t.LoginProvider).HasMaxLength(128);
                b.Property(t => t.Name).HasMaxLength(128);
            });

            builder.Entity<IdentityUserLogin<int>>(b =>
            {
                b.ToTable(name: "User.AspNetUserLogins");
                b.HasKey(k => k.UserId);
            }
            );

            builder.Entity<IdentityUserClaim<int>>(b =>
            {
                b.ToTable(name: "User.AspNetUserClaims");
            }
            );

            builder.Entity<IdentityUserRole<int>>(b =>
            {
                b.ToTable(name: "User.AspNetUserRoles");
            }
            );

            builder.Entity<IdentityRoleClaim<int>>(b =>
            {
                b.ToTable(name: "User.AspNetRoleClaims");
            }
            );

        }
    }
}
