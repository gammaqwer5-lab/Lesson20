using ConsoleApp15;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateApplicationBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("Default")
    ?? throw new InvalidOperationException("Не найден Default");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

using var host = builder.Build();
using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

try
{
    Console.Write("Минимальный возраст: ");
    int minAge = int.Parse(Console.ReadLine()!);

    Console.Write("Максимальный возраст: ");
    int maxAge = int.Parse(Console.ReadLine()!);

    Console.Write("Номер страницы: ");
    int page = int.Parse(Console.ReadLine()!);

    const int pageSize = 3;

    var students = await db.Students
    .Select(s => new {
        s.FirstName,
        s.LastName,
        s.Age
    })
    .Where(s => s.Age >= minAge && s.Age <= maxAge)
    .OrderBy(s => s.LastName)
    .ThenBy(s => s.FirstName)
    .Skip((page - 1) * pageSize)
    .Take(pageSize)
    .ToListAsync();

    Console.WriteLine("\nрезультат поиска\n");

    foreach (var student in students)
    {
        Console.WriteLine($"{student.FirstName} {student.LastName} {student.Age}");
    }

    var hasMinors = await db.Students.AnyAsync(s => s.Age < 18);

    Console.WriteLine($"\nесть несовершеннолетние студенты: {hasMinors}\n");
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка: {ex.Message}");
}