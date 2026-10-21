using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace ConsoleApp15
{
    internal class AppDbContext : DbContext
    {
        public DbSet<Student> Students => Set<Student>();
        public AppDbContext(DbContextOptions opt) : base(opt)
        {

        }
    }
}
