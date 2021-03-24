using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Issue24216Repro.Model
{
    public class TestContext : DbContext
    {
        public DbSet<Gender> Gender { get; set; }

        public DbSet<Message> Message { get; set; }

        public IQueryable<PersonStatus> GetPersonStatusAsOf(long personId, DateTime asOf)
            => FromExpression(() => GetPersonStatusAsOf(personId, asOf));

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("server=tcp:localhost,1433;database=test;user id=testadmin;password=asdf");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDbFunction(typeof(TestContext).GetMethod(nameof(GetPersonStatusAsOf),
                new[] { typeof(long), typeof(DateTime) }));
        }
    }
}
