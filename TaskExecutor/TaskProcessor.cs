using System;
using System.Threading.Tasks;
using Shared.Enums;
using Shared.Models;

namespace TaskExecutor
{
    public class TaskProcessor
    {
        public async Task<double> ProcessAsync(TaskItem task, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Processing Task {task.Id}...");

            try
            {
                // Perform calculation
                var result = Calculate(task.Type, task.Data);

                // Simulate workload with a random delay based on TTL
                var random = new Random();
                var minDelay = Math.Max(10, task.Ttl * 0.1); // Minimum delay of 10ms or 10% of TTL
                var maxDelay = task.Ttl * 1.1; // Maximum delay of 110% of TTL

                // Generate a random delay within the range [minDelay, maxDelay]
                var simulatedDelay = random.NextDouble() * (maxDelay - minDelay) + minDelay;

                var totalDelay = TimeSpan.FromMilliseconds(simulatedDelay);

                var startTime = DateTime.UtcNow;

                while (DateTime.UtcNow - startTime < totalDelay)
                {
                    cancellationToken.ThrowIfCancellationRequested(); // Check for cancellation

                    // Simulate a small workload during the delay (this can be adjusted for granularity)
                    await Task.Delay(10, cancellationToken); // Delay for 10ms, adjust as needed
                }

                Console.WriteLine($"Task {task.Id} completed with result: {result}");
                return result;
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine($"Task {task.Id} was canceled.");
                throw; // Allow the caller to handle cancellation
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing Task {task.Id}: {ex.Message}");
                throw; // Rethrow the exception for higher-level handling
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