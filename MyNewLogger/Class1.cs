using System.Threading.Channels;

namespace MyNewLogger
{
    public interface ILogger
    {
        public void LogInformation(string message);
        public void LogError(Exception exeption, string? additionalMessage = null);
    }

    public class ConsoleLogger : ILogger
    {
        public void LogInformation(string message)
        {
            Console.WriteLine($"LogInformation: {message}");
        }

        public void LogError(Exception exeption, string? additionalMessage = null)
        {
            Console.WriteLine($"LogError:{exeption}");
            if (additionalMessage != null)
            {
                Console.WriteLine(additionalMessage);
            }
        }
    }

    public class FileLogger : ILogger
    {
        
        public void LogInformation(string message)
        {
            using (var sw = new StreamWriter("log.txt"))
            {
                sw.WriteLine($"LogInformation: {message}");
            }  
        }

        public void LogError(Exception exeption, string? additionalMessage = null)
        {
            using (var sw = new StreamWriter("log.txt"))
            {
                sw.WriteLine($"LogError:{exeption}");
                if (additionalMessage != null)
                {
                    sw.WriteLine(additionalMessage);
                }
            }
                
        }   
    }

    public class CompositeLogger : ILogger
    {
        private readonly ILogger _ConsoleLogger;
        private readonly ILogger _FileLogger;
        public CompositeLogger(ILogger ConsoleLogger, ILogger FileLogger)
        {
            _ConsoleLogger = ConsoleLogger;
            _FileLogger = FileLogger;
        }

        public void LogError(Exception exeption, string? additionalMessage = null)
        {
            using (var sw = new StreamWriter("log.txt"))
            {
                sw.WriteLine($"LogError:{exeption}");
                Console.WriteLine($"LogError:{exeption}");
                if (additionalMessage != null)
                {
                    Console.WriteLine(additionalMessage);
                    sw.WriteLine(additionalMessage);
                }
            }
        }

        public void LogInformation(string message)
        {
            using (var sw = new StreamWriter("log.txt"))
            {
                sw.WriteLine($"LogInformation: {message}");
                Console.WriteLine($"LogInformation: {message}");
            }
        }
    }
}
