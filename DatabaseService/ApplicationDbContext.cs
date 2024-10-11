using Microsoft.EntityFrameworkCore;
using Shared.Models;

namespace DatabaseService
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<TaskModel> Tasks { get; set; } // Таблица задач
    }
}