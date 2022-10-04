using Fanex.Data.Repository;

namespace Fanex.Logging.Distributor.Commands;

public class InsertCategoryCommand : NonQueryCommand
{
    public string CategoryName { get; init; }

    public long LogId { get; init; }

    public override string GetSettingKey() => "Acc_AppLog_CategoryIns";

    public override bool IsValid() => !string.IsNullOrWhiteSpace(CategoryName);
}