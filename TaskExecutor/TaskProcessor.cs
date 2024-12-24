using System;
using System.Threading.Tasks;

namespace TaskExecutor
{
    public class TaskProcessor
    {
        public async Task<string> ProcessAsync(string taskMessage)
        {
            try
            {
                // Парсим задачу
                var result = await Task.Run(() => Calculate(taskMessage));
                return $"Result of '{taskMessage}' is {result}";
            }
            catch (Exception ex)
            {
                return $"Error processing task '{taskMessage}': {ex.Message}";
            }
        }

        private double Calculate(string task)
        {
            // Удаляем пробелы
            task = task.Replace(" ", "");

            // Определяем операцию
            char operation = ' ';
            if (task.Contains("+")) operation = '+';
            else if (task.Contains("-")) operation = '-';
            else if (task.Contains("*")) operation = '*';
            else if (task.Contains("/")) operation = '/';
            else throw new InvalidOperationException("Invalid operation");

            // Разделяем операнды
            var parts = task.Split(operation);
            if (parts.Length != 2)
                throw new InvalidOperationException("Invalid task format");

            // Преобразуем операнды в числа
            double leftOperand = double.Parse(parts[0]);
            double rightOperand = double.Parse(parts[1]);

            // Выполняем вычисление
            return operation switch
            {
                '+' => leftOperand + rightOperand,
                '-' => leftOperand - rightOperand,
                '*' => leftOperand * rightOperand,
                '/' when rightOperand != 0 => leftOperand / rightOperand,
                '/' => throw new DivideByZeroException(),
                _ => throw new InvalidOperationException("Unsupported operation"),
            };
        }
    }
}
