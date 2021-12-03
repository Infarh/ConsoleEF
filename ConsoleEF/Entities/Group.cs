using System.Collections.Generic;

namespace ConsoleEF.Entities
{
    public class Group
    {
        public int Id { get; set; }

        public string Name { get; set; }

        // Устанавливаем связь между таблицами 1-ко-многим
        public ICollection<Student> Students { get; set; } = new HashSet<Student>();
    }
}
