using DAL.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace DAL
{
    public class OnBoardingSkdDbContext : DbContext
    {
        public OnBoardingSkdDbContext(DbContextOptions<OnBoardingSkdDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DressModel>().HasIndex(t => t.Name).IsUnique();
        }

        public DbSet<DressModel> Dresses { get; set; }
        public DbSet<DesignerModel> Designers { get; set; }

    }
}
