﻿using ASP.NETTokenBasedAuthetication.Data.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ASP.NETTokenBasedAuthetication.Data
{
    public class AppDbContext : IdentityDbContext<ApplicationUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) :  base(options)
        {

        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
