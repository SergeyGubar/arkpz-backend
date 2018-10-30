
using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StatosphericBackend.Entities;

namespace StatosphericBackend.Context
{
    public class ApplicationContext: IdentityDbContext<User>
    {

        public DbSet<Launch> Launches { get; set; }
//        public DbSet<User> Users { get; set; }
        
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

    }
}