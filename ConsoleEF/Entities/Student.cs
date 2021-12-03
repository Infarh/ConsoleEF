using System;

namespace ConsoleEF.Entities
{
    public class Student
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string LastName { get; set; }

        public string Patronymic { get; set; }

        public DateTime Birthday { get; set; }
    }
}
