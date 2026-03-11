using Microsoft.Extensions.Logging;

namespace eventoweb_secretaria_back.Logging;

public sealed class FileLoggerProvider(string filePath) : ILoggerProvider
{
    private readonly object m_Lock = new();
    private readonly string m_FilePath = filePath;

    public ILogger CreateLogger(string categoryName)
    {
        return new FileLogger(categoryName, m_FilePath, m_Lock);
    }

    public void Dispose()
    {
    }
}

internal sealed class FileLogger(string categoryName, string filePath, object fileLock) : ILogger
{
    private readonly string m_CategoryName = categoryName;
    private readonly string m_FilePath = filePath;
    private readonly object m_FileLock = fileLock;

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        if (!IsEnabled(logLevel))
            return;

        var message = formatter(state, exception);
        if (string.IsNullOrWhiteSpace(message))
            return;

        var directory = Path.GetDirectoryName(m_FilePath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
            Directory.CreateDirectory(directory);

        var line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss.fff} [{logLevel}] {m_CategoryName} - {message}";
        if (exception != null)
            line += $" {exception}";

        lock (m_FileLock)
        {
            File.AppendAllText(m_FilePath, line + Environment.NewLine);
        }
    }
}
