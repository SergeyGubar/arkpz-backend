
using System;
using Microsoft.EntityFrameworkCore;
using StatosphericBackend.Entities;

namespace StatosphericBackend.Context
{
    public class ApplicationContext: DbContext
    {

        public DbSet<Launch> Launches { get; set; }
        
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }
    }
}