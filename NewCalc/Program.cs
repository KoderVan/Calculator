using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using MyNewLogger;

namespace calc
{
    public class Program
    {
        static void Main(string[] args)
        {
            string? mathOperation;
            var ConsoleLogger = new ConsoleLogger();
            var FileLogger = new FileLogger();
            var CompositeLogger = new CompositeLogger(ConsoleLogger, FileLogger);
            Calc2 calc = new Calc2(ConsoleLogger);
            var dict = new Dictionary<string, Func<double, double, double?>>
            {
                {"+", calc.Sum},
                {"-", calc.Substr},
                {"*", calc.Multiply},
                {"/", calc.Divide},
                {"%", calc.Percent},
                {"^", calc.Exponent}
            };
            string[] resultString;
            double? result = 0.0;

            Console.WriteLine("Введите своё выражение:");
            
            while (true)
            {
                mathOperation = Console.ReadLine().Replace(" ", "");

                if (mathOperation.ToLower() == "exit")
                {
                    break;
                }
                else if (mathOperation.ToLower() == "help")
                {
                    Console.WriteLine("Основные команды:\n" +
                    "Help - справка\n" +
                    "Exit - выход из приложения\n" +
                    "\n" +
                    "Поддерживаемы операции: +, -, *, /, %, ^");
                }
                else
                {
                    try
                    {
                        resultString = ParseString(mathOperation);
                    }
                    catch (Exception ex)
                    {
                        CompositeLogger.LogError(ex, "Не удалось распарсить строку");
                        break;
                    }
                    try
                    {
                        result = calc.CountResult(resultString, dict);
                        Console.WriteLine(result);
                    }
                    catch (Exception ex)
                    {
                        CompositeLogger.LogError(ex, "Не удалось распарсить строку");
                    }
                }
            }
        }

        static string[] ParseString(string mathOperation)
        {
            string operand1 = "";
            string operand2 = "";
            string operation = "";
            bool seekOperand1 = true;
            
            for (int i = 0; i < mathOperation.Length; i++)
            {
                if (seekOperand1)
                {
                    if (char.IsDigit(mathOperation[i]))
                    {
                        operand1 += mathOperation[i];
                    }
                    else
                    {
                        operation = mathOperation[i].ToString();
                        seekOperand1 = false;
                    }
                }
                else
                {
                    operand2 += mathOperation[i];
                }
            }
            string[] mathOperationList = [operand1, operand2, operation];
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

        public double? Sum(double operand1, double operand2)
        {
            _logger.LogInformation("Сложение");
            return operand1 + operand2;
        }
        public double? Substr(double operand1, double operand2)
        {
            _logger.LogInformation("Вычитание");
            return operand1 - operand2;
        }
        public double? Multiply(double operand1, double operand2)
        {
            _logger.LogInformation("Умножение");
            return operand1 * operand2;
        }
        public double? Divide(double operand1, double operand2)
        {
            if (operand2 == 0)
            {
                _logger.LogError(new DivideByZeroException(), "Деление на ноль");
                
                return null;
            }
            else return operand1 / operand2;
        }
        
        public double? Percent(double operand1, double operand2) 
        {
            _logger.LogInformation("Остаток");
           return operand1 % operand2;
        }
        public double? Exponent(double operand1, double operand2)
        {
            _logger.LogInformation("Возведение в степень");
            return Math.Pow(operand1, operand2);
        }
        public double? CountResult(string[] mathOperationList, Dictionary<string, Func<double, double, double?>> dict)
        {
            double operand1 = double.Parse(mathOperationList[0]);
            double operand2 = double.Parse(mathOperationList[1]);
            string operation = mathOperationList[2];

            return dict[operation](operand1, operand2);
        }
    }

}
