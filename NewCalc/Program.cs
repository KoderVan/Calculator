using MyNewLogger;

namespace calc
{
    public class Program
    {
        static void Main(string[] args)
        {
            string? mathOperation;
            var consoleLogger = new ConsoleLogger();
            var fileLogger = new FileLogger("log.txt");
            var compositeLogger = new CompositeLogger(consoleLogger, fileLogger);
            Calc2 calc = new Calc2(compositeLogger);
            
            string[] resultString;
            double result = 0.0;

            PrintInstruction();
            Console.WriteLine("Введите своё выражение:");
            
            
            while (true)
            {
                mathOperation = Console.ReadLine();

                if (mathOperation.ToLower() == "exit")
                {
                    break;
                }
                else if (mathOperation.ToLower() == "help")
                {
                    PrintInstruction();
                }
                else
                {
                    try
                    {
                        resultString = ParseString(mathOperation);
                    }
                    catch (Exception ex)
                    {
                        compositeLogger.LogError(ex, "Не удалось распарсить строку");
                        Console.WriteLine("Введите выражение ещё раз:");
                        continue;
                    }
                    try
                    {
                        result = calc.CountResult(resultString);
                        Console.WriteLine(result);
                    }
                    catch (Exception ex)
                    {
                        compositeLogger.LogError(ex, "Не удалось вычислить");
                    }
                }
            }
        }
        static void PrintInstruction()
        {
            Console.WriteLine("Основные команды:\n" +
                    "Help - справка\n" +
                    "Exit - выход из приложения\n" +
                    "\n" +
                    "Поддерживаемы операции: +, -, *, /, %, ^");
        }
        static string[] ParseString(string mathOperation)
        {         
            string[] mathOperationList = mathOperation.Split(' ');
            return mathOperationList;
        }

    }

    class Calc2
    {
        private readonly ILogger _logger;
        public Calc2(ILogger logger)
        {
            _logger = logger;
        }

        public double Sum(double operand1, double operand2)
        {
            _logger.LogInformation("Сложение");
            return operand1 + operand2;
        }
        public double Substr(double operand1, double operand2)
        {
            _logger.LogInformation("Вычитание");
            return operand1 - operand2;
        }
        public double Multiply(double operand1, double operand2)
        {
            _logger.LogInformation("Умножение");
            return operand1 * operand2;
        }
        public double Divide(double operand1, double operand2)
        {
            if (operand2 == 0)
            {
                var ex = new DivideByZeroException("Попытка деления на ноль");
                _logger.LogError(ex, "Деление на ноль");
                throw ex;
            }
            return operand1 / operand2;
        }
        
        public double Percent(double operand1, double operand2) 
        {
            _logger.LogInformation("Остаток");
           return operand1 % operand2;
        }
        public double Exponent(double operand1, double operand2)
        {
            _logger.LogInformation("Возведение в степень");
            return Math.Pow(operand1, operand2);
        }
        public double CountResult(string[] mathOperationList)
        {
            var dict = new Dictionary<string, Func<double, double, double>>
            {
                {"+", Sum},
                {"-", Substr},
                {"*", Multiply},
                {"/", Divide},
                {"%", Percent},
                {"^", Exponent}
            };
            double operand1 = double.Parse(mathOperationList[0], System.Globalization.CultureInfo.InvariantCulture);
            double operand2 = double.Parse(mathOperationList[2], System.Globalization.CultureInfo.InvariantCulture);
            string operation = mathOperationList[1];

            return dict[operation](operand1, operand2);
        }
    }

}
