# Проект-пример использования БД в консольном приложении

Проект предназначен для того, что бы показать как можно за минимальное число шагов добавить себе возможность работать с любой БД

## Необходимые шаги

1. Добавить в проект NuGet-пакет с требуемым поставщиком БД.
К примеру, для добавления подключения к MS SQL-Server требуется
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="6.0.0" />
```
