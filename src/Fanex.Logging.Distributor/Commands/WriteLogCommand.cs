using Fanex.Data.Repository;

namespace Fanex.Logging.Distributor.Commands;

public class WriteLogCommand : NonQueryCommand
{
    public int EventId { get; init; }

    public int Priority { get; init; }

    public string Severity { get; init; }

    public string Title { get; init; }

    public DateTime TimeStamp { get; init; }

    public string MachineName { get; init; }

    public string AppDomainName { get; init; }

    public string ProcessId { get; init; }

    public string ProcessName { get; init; }

    public string ThreadName { get; init; }

    public string Win32ThreadId { get; init; }

    public string Message { get; init; }

    public string FormattedMessage { get; init; }

    public int LogId { get; set; }

    public override string GetSettingKey() => "WriteLog_AppLog";

    public override bool IsValid() => true;
}