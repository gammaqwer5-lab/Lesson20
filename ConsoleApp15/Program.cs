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
    int count = await db.Students.CountAsync();
    double averageAge = await db.Students.AverageAsync(s => s.Age);
    int minAge = await db.Students.MinAsync(s => s.Age);
    int maxAge = await db.Students.MaxAsync(s => s.Age);

    bool allAdults = await db.Students
    .AllAsync(s => s.Age >= 18);

    var statistics = await db.Students
    .GroupBy(s => s.Age)
    .Select(g => new
    {
        Age = g.Key,
        Count = g.Count()
    })
    .OrderBy(x => x.Age)
    .ToListAsync();

    Console.WriteLine($"\nВсего студентов: {count}");
    Console.WriteLine($"Средний возраст: {averageAge}");
    Console.WriteLine($"Минимальный возраст: {minAge}");
    Console.WriteLine($"Максимальный возраст: {maxAge}");
    Console.WriteLine("Распределение по возрасту:");
    foreach (var stat in statistics)
    {
        Console.WriteLine($"{stat.Age} лет - {stat.Count}");
    }

    Console.WriteLine($"Все студенты совершеннолетние: {allAdults}");
}
catch (Exception ex)
{
    Console.WriteLine($"Ошибка: {ex.Message}");
}