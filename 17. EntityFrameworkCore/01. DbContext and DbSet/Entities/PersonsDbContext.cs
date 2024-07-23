using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
  public class PersonsDbContext : DbContext
  {
    public DbSet<Country> Countries { get; set; }
    public DbSet<Person> Persons { get; set; }

    public PersonsDbContext(DbContextOptions<PersonsDbContext> dbContextOptions) : base(dbContextOptions)
    {
      
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      

      modelBuilder.Entity<Country>().ToTable("Countries");
      modelBuilder.Entity<Person>().ToTable("Persons");
    }
  }
}
