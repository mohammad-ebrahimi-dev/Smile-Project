namespace SmileProject.Services
{
    public class Logs
    {
        public static void LogToFile(string message)
        {
            var logFilePath = "sendSentences_log.txt";
            var logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - {message}{Environment.NewLine}";

            System.IO.File.AppendAllText(logFilePath, logMessage);
        }
    }
}
