using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace dotnet_rpg.Data
{
    public class DataContext : DbContext
    {
     public DataContext(DbContextOptions<DataContext> options) : base(options){

     }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Skill>().HasData(
                new Skill {Id = 1, Name = "Fireball", Damage = 30},
                new Skill {Id = 2, Name = "Frenzy", Damage = 45},
                new Skill {Id = 3, Name = "Bless", Damage = 40});
        }

        public DbSet<Character> Characters =>Set<Character>(); //corresponding db table
    public DbSet<User> Users =>Set<User>();
    public DbSet<Weapon> Weapons =>Set<Weapon>();
    public DbSet<Skill> Skills =>Set<Skill>();


     
    }
}