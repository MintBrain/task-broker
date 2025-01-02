using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Shared.Enums;
using Shared.Models;

namespace TaskExecutor
{
    public class TaskProcessor
    {
        private readonly ILogger<TaskProcessor> _logger;

        public TaskProcessor(ILogger<TaskProcessor> logger)
        {
            _logger = logger;
        }

        public async Task<double> ProcessAsync(TaskItem task, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Processing Task {TaskId} of type {TaskType}...", task.Id, task.Type);

            try
            {
                // Выполнение вычислений
                var result = Calculate(task.Type, task.Data);

                // Имитация задержки выполнения
                var random = new Random();
                var minDelay = Math.Max(10, task.Ttl * 0.1); // Минимальная задержка
                var maxDelay = task.Ttl * 1.1; // Максимальная задержка

                var simulatedDelay = random.NextDouble() * (maxDelay - minDelay) + minDelay;
                var totalDelay = TimeSpan.FromMilliseconds(simulatedDelay);

                _logger.LogDebug("Task {TaskId} will simulate workload for {Delay} ms", task.Id, totalDelay.TotalMilliseconds);

                var startTime = DateTime.UtcNow;

                while (DateTime.UtcNow - startTime < totalDelay)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    await Task.Delay(10, cancellationToken); // Малые задержки для проверки отмены
                }

                _logger.LogInformation("Task {TaskId} completed successfully with result: {Result}", task.Id, result);
                return result;
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning("Task {TaskId} was canceled.", task.Id);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing Task {TaskId}: {ErrorMessage}", task.Id, ex.Message);
                throw;
            }
        }

        private static double Calculate(TaskType taskType, string data)
        {
            var parts = data.Split(',');
            if (parts.Length != 2)
                throw new InvalidOperationException("Invalid task format");

            // Преобразуем операнды в числа
            var leftOperand = double.Parse(parts[0]);
            var rightOperand = double.Parse(parts[1]);

            return taskType switch
            {
                TaskType.Addition => leftOperand + rightOperand,
                TaskType.Subtraction => leftOperand - rightOperand,
                TaskType.Multiplication => leftOperand * rightOperand,
                TaskType.Division when rightOperand != 0 => leftOperand / rightOperand,
                TaskType.Division => throw new DivideByZeroException("Division by zero"),
                _ => throw new InvalidOperationException($"Unknown task type: {taskType}")
            };
        }
    }
}
