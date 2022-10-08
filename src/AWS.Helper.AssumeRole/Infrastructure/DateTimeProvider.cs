using System;

namespace AWS.Helper.AssumeRole
{
    public interface IDateTimeProvider
    {
        public DateTimeOffset Now { get; }
        public DateTimeOffset UtcNow { get; }
    }

    internal sealed class DateTimeProvider : IDateTimeProvider
    {
        public DateTimeOffset Now => DateTimeOffset.Now;
        public DateTimeOffset UtcNow => DateTimeOffset.UtcNow;
    }
}
