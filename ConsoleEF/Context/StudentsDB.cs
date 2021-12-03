using Microsoft.EntityFrameworkCore;

namespace ConsoleEF.Context
{
    public class StudentsDB : DbContext
    {
        public StudentsDB(DbContextOptions<StudentsDB> opt) : base(opt)
        {
            
        }
    }
}
