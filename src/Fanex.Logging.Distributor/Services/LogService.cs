using Fanex.Data.Repository;
using Fanex.Logging.Distributor.Commands;

namespace Fanex.Logging.Distributor.Services;

public interface ILogService
{
    Task WriteLog(LogEntry logEntry);
}

public class LogService : ILogService
{
    private readonly IDynamicRepository dynamicRepository;

    public LogService(IDynamicRepository dynamicRepository)
    {
        this.dynamicRepository = dynamicRepository;
    }

    public async Task WriteLog(LogEntry logEntry)
    {
        var command = logEntry.Categories != null && logEntry.Categories.Any()
        ? new WriteLogCommand
        {
            AppDomainName = logEntry.AppDomainName,
            EventId = logEntry.EventId,
            MachineName = logEntry.MachineName,
            Message = logEntry.Message,
            Priority = logEntry.Priority,
            ProcessId = logEntry.ProcessId,
            ProcessName = logEntry.ProcessName,
            ThreadName = logEntry.ManagedThreadName,
            Severity = logEntry.Severity.ToString(),
            TimeStamp = logEntry.TimeStamp,
            Title = logEntry.Title,
            Win32ThreadId = logEntry.Win32ThreadId,
            FormattedMessage = string.Join(Environment.NewLine,
                $"Timestamp: {logEntry.TimeStamp}",
                $"Message: {logEntry.Message}",
                $"Category: {logEntry.Categories.FirstOrDefault()}",
                $"Priority: {logEntry.Priority}",
                $"EventId: {logEntry.EventId}",
                $"Severity: {logEntry.Severity}",
                $"Title: {logEntry.Title}",
                $"Machine: {logEntry.MachineName}",
                $"App Domain: {logEntry.AppDomainName}",
                $"ProcessId: {logEntry.ProcessId}",
                $"Process Name: {logEntry.ProcessName}",
                $"Thread Name: {logEntry.ManagedThreadName}",
                $"Win32 ThreadId: {logEntry.Win32ThreadId}",
                $"Extended Properties: {string.Join(",", logEntry.ExtendedProperties.Select(p => $"{p.Key} - {p.Value}").ToArray())}")
        }
        : new WriteLogCommand { Message = logEntry.Message };

        await dynamicRepository.ExecuteAsync(command);

        await dynamicRepository.ExecuteAsync(new InsertCategoryCommand { CategoryName = logEntry.Categories.FirstOrDefault(), LogId = command.LogId });
    }
}