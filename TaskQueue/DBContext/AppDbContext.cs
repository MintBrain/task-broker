using Microsoft.EntityFrameworkCore;
using Shared.Models;
using TaskQueue.Models;

namespace TaskQueue.Database
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Таблица задач
        public DbSet<TaskItem> Tasks { get; set; }

        // Таблица результатов задач
        public DbSet<TaskResult> TaskResults { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Настройка таблицы для TaskItem
            modelBuilder.Entity<TaskItem>()
                .HasKey(t => t.Id); // Задание первичного ключа

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Data)
                .IsRequired(); // Поле Data должно быть обязательным

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Ttl)
                .HasDefaultValue(60000); // Значение TTL по умолчанию (60 секунд)

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Status)
                .HasConversion<int>(); // Преобразование Enum в int для хранения в БД

            // Настройка таблицы для TaskResult
            modelBuilder.Entity<TaskResult>()
                .HasKey(r => r.Id); // Задание первичного ключа

            modelBuilder.Entity<TaskResult>()
                .Property(r => r.Status)
                .HasConversion<int>(); // Преобразование Enum в int для хранения в БД

            base.OnModelCreating(modelBuilder);
        }
    }
}