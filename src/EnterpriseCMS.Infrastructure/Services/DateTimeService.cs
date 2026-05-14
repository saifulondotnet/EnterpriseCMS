using EnterpriseCMS.Core.Interfaces;

namespace EnterpriseCMS.Infrastructure.Services;

public class DateTimeService : IDateTimeService
{
    public DateTime UtcNow => DateTime.UtcNow;
}
