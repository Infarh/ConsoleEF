using ConsoleEF.Entities;
using Microsoft.EntityFrameworkCore;

namespace ConsoleEF.Context
{
    public class StudentsDB : DbContext
    {
        public DbSet<Student> Students { get; set; }

        public DbSet<Group> Groups { get; set; }

        public StudentsDB(DbContextOptions<StudentsDB> opt) : base(opt)
        {
            
        }
    }
}
