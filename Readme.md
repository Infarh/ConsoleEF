# Проект-пример использования БД в консольном приложении

Проект предназначен для того, что бы показать как можно за минимальное число шагов добавить себе возможность работать с любой БД

## Необходимые шаги для создания инфраструктуры

1. Добавить в проект NuGet-пакет с требуемым поставщиком БД

К примеру, для добавления подключения к MS SQL-Server требуется

```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="6.0.0" />
```

2. Добавить в проект описание таблиц БД в виде классов-сущностей

Каждая сущность должна иметь первичный ключ: свойство типа `int`, `long`, `uint`, `ulong`..., `string`, `Guid`

```C#
public class Student
{
    public int Id { get; set; }

    // ...
}
```

Сущности определяют структуру таблиц: каждое свойство - столбец таблицы. Имя сущности - имя таблицы (во множественном числе), имя свойства - имя столбца. Имена можно изменить с помощью атрибутов, либо в настройках модели БД в классе контекста БД.

3. Добавить класс контекста БД

Через объект этого класса будет в дальнейшем осуществляться взаимодействие с БД.

```C#
public class StudentsDB : DbContext
{
    public StudentsDB(DbContextOptions<StudentsDB> opt) : base(opt)
    {
        
    }
}
```

У класса-контекста БД должен быть конструктор, передающий конструктору базового класса набор параметров.

4. Добавить в класс контекста БД набор свойств типа `DbSet<T>`, определяющих возможности доступа к нужным таблицам БД

```C#
public class StudentsDB : DbContext
{
    public DbSet<Student> Students { get; set; }

    public DbSet<Group> Groups { get; set; }

    public StudentsDB(DbContextOptions<StudentsDB> opt) : base(opt)
    {
        
    }
}
```

Инфраструктура доступа к БД готова.

## Работа с контекстом БД и его конфигурирование

1. Для соединения с БД необходимо создать объект контекста БД

Для этого требуется выполнить его конфигурирование (как минимум, указать строку подключения).

Пусть строка подключения будет 

```
Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Students.Test.db
```

Нужно создать объект класса `DbContextOptions<StudentsDB>` и указать ему строку подключения. Делается это с помощью билдера.

```C#
var connection_options = new DbContextOptionsBuilder<StudentsDB>()
   .UseSqlServer(sql_server_connection_string)
   .Options;
```

Полученный объект настроек подключения можно использовать для создания экземпляров контекста БД.

```C#
using (var db = new StudentsDB(connection_options))
{
    // ...
}

using (var db = new StudentsDB(new DbContextOptionsBuilder<StudentsDB>().UseSqlServer(sql_server_connection_string).Options))
{
    // ...
}
```

Первый вариант удобнее.

2. Инициализация БД исходными данными

При первом обращении к серверу БД база на сервере может отсутствовать. Её можно создать программно.

```C#
using (var db = new StudentsDB(connection_options))
{
    db.Database.EnsureCreated();
}
```

3. При обращении к существующей БД можно выполнить проверку наличия в ней необходимых исходных данных, и в случае их отсусттвия добавить их

```C#
using (var db = new StudentsDB(connection_options))
{
    db.Database.EnsureCreated();

    if (!db.Students.Any())
    {
        var ivanov = new Student
        {
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

        db.SaveChanges();
    }
}
```

Важно не забыть вызвать метод `SaveChanges()`. И также важно не хранить долго экземпляр контекста БД дабы он не забивался кешируемыми данными и не замедлял работу. Контекст БД должен создаваться на время выполнения одной (возможно комплексной) операции взаимодействия с БД.