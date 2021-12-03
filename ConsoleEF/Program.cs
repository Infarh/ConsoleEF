using System;
using System.Linq;
using ConsoleEF.Context;
using ConsoleEF.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ConsoleEF
{
    class Program
    {
        public static void Main(string[] args)
        {
            const string sql_server_connection_string = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Students.Test.db";

            var connection_options = new DbContextOptionsBuilder<StudentsDB>()
               .UseSqlServer(sql_server_connection_string)
               .Options;

            using (var db = new StudentsDB(connection_options))
            {
                db.Database.EnsureCreated();

                if (!db.Students.Any())
                {
                    var ivanov = new Student
                    {
                        //Id = ???, - идентификатор никогда вручную не меняем!
                        LastName = "Иванов",
                        Name = "Иван",
                        Patronymic = "Иванович",
                        Birthday = DateTime.Now.AddYears(-21),
                    };

                    var petrov = new Student
                    {
                        LastName = "Петров",
                        Name = "Пётр",
                        Patronymic = "Петрович",
                        Birthday = DateTime.Now.AddYears(-24),
                    };

                    var sidorov = new Student
                    {
                        LastName = "Сидоров",
                        Name = "Сидор",
                        Patronymic = "Сидорович",
                        Birthday = DateTime.Now.AddYears(-22),
                    };

                    var group1 = new Group { Name = "Группа 1" };
                    var group2 = new Group { Name = "Группа 2" };

                    group1.Students.Add(ivanov);
                    group1.Students.Add(petrov);

                    group2.Students.Add(sidorov);

                    db.Groups.Add(group1);
                    db.Groups.Add(group2);

                    // Студентов в контекст можно добавлять, а можно не добавлять
                    // Контекст увидит, что в добавленных в него группах есть ссылка на студентов и студенты тоже будут переданы в БД
                    //db.Students.Add(ivanov);
                    //db.Students.Add(petrov);
                    //db.Students.Add(sidorov);

                    db.SaveChanges(); // здесь будет выполнена передача запроса в БД с целью записи данных
                }
            }

            var configuration = new ConfigurationBuilder()
               .AddJsonFile("appsettings.json")
               .Build();

            var db_options = new DbContextOptionsBuilder<StudentsDB>()
               .UseSqlServer(configuration.GetConnectionString("Default"))
               .Options;

            using (var db = new StudentsDB(db_options))
            {
                var grous_with_max_students = db.Groups
                   .Include(g => g.Students)
                   .OrderByDescending(g => g.Students.Count())
                   .First();

                Console.WriteLine(grous_with_max_students.Name);
                foreach (var student in grous_with_max_students.Students)
                    Console.WriteLine("{0} {1} {2}", student.LastName, student.Name, student.Patronymic);
            }
        }
    }
}
