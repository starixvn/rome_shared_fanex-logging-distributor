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
    private readonly ILogger logger;

    public LogService(IDynamicRepository dynamicRepository, ILogger logger)
    {
        this.dynamicRepository = dynamicRepository;
        this.logger = logger;
    }

    public async Task WriteLog(LogEntry logEntry)
    {
        try
        {
            var command = new WriteLogCommand
            {
                AppDomainName = logEntry.AppDomainName,
                EventId = logEntry.EventId,
                MachineName = logEntry.MachineName,
                Message = logEntry.Message,
                Priority = logEntry.Priority,
                ProcessId = logEntry.ProcessId,
                ProcessName = logEntry.ProcessName,
                ThreadName = logEntry.ManagedThreadName ?? string.Empty,
                Severity = logEntry.Severity.ToString(),
                TimeStamp = logEntry.TimeStamp,
                Title = logEntry.Title,
                Win32ThreadId = logEntry.Win32ThreadId,
                FormattedMessage = string.Join("\r\n",
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
            };

            await dynamicRepository.ExecuteAsync(command);

            await dynamicRepository.ExecuteAsync(new InsertCategoryCommand { CategoryName = logEntry.Categories.FirstOrDefault(), LogId = command.LogId });
        }
        catch (Exception ex)
        {
            await logger.ErrorAsync(ex.Message, ex);
        }
    }
}