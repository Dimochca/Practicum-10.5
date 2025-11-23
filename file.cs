using System;

public interface ICalculator
{
    double Add(double a, double b);
}

public interface ILogger
{
    void LogEvent(string message);
    void LogError(string message);
}

public class Calculator : ICalculator
{
    private readonly ILogger _logger;

    public Calculator(ILogger logger)
    {
        _logger = logger;
    }

    public double Add(double a, double b)
    {
        _logger.LogEvent($"Выполняется сложение: {a} + {b}");
        double result = a + b;
        _logger.LogEvent($"Результат сложения: {result}");
        return result;
    }
}

public class ColorConsoleLogger : ILogger
{
    public void LogEvent(string message)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"[СОБЫТИЕ] {DateTime.Now:HH:mm:ss}: {message}");
        Console.ResetColor();
    }

    public void LogError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[ОШИБКА] {DateTime.Now:HH:mm:ss}: {message}");
        Console.ResetColor();
    }
}

public static class SimpleDIContainer
{
    public static ILogger GetLogger()
    {
        return new ColorConsoleLogger();
    }

    public static ICalculator GetCalculator()
    {
        return new Calculator(GetLogger());
    }
}

class Program
{
    static void Main(string[] args)
    {
        var logger = SimpleDIContainer.GetLogger();
        var calculator = SimpleDIContainer.GetCalculator();

        logger.LogEvent("Мини-калькулятор запущен");

        try
        {
            Console.WriteLine("Введите первое число:");

            if (!double.TryParse(Console.ReadLine(), out double number1))
            {
                throw new FormatException("Некорректный формат первого числа");
            }

            Console.WriteLine("Введите второе число:");

            if (!double.TryParse(Console.ReadLine(), out double number2))
            {
                throw new FormatException("Некорректный формат второго числа");
            }

            double result = calculator.Add(number1, number2);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\nРезультат: {number1} + {number2} = {result}");
            Console.ResetColor();
        }
        catch (FormatException ex)
        {
            logger.LogError($"Ошибка ввода: {ex.Message}");
            Console.WriteLine("Пожалуйста, вводите числа в правильном формате");
        }
        catch (OverflowException ex)
        {
            logger.LogError($"Ошибка переполнения: {ex.Message}");
            Console.WriteLine("Введено слишком большое или слишком маленькое число");
        }
        catch (Exception ex)
        {
            logger.LogError($"Неожиданная ошибка: {ex.Message}");
            Console.WriteLine("Произошла непредвиденная ошибка");
        }
        finally
        {
            logger.LogEvent("Работа калькулятора завершена");
            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}
