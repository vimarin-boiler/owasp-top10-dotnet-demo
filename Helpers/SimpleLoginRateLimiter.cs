namespace OwaspTop10Demo.Helpers
{
    public class SimpleLoginRateLimiter
    {
        private class Entry
        {
            public int FailCount { get; set; }
            public DateTimeOffset? BlockedUntil { get; set; }
        }

        private readonly Dictionary<string, Entry> _entries = new();
        private readonly object _lock = new();

        public bool IsBlocked(string key)
        {
            lock (_lock)
            {
                if (!_entries.TryGetValue(key, out var entry))
                    return false;

                if (entry.BlockedUntil > DateTimeOffset.UtcNow)
                    return true;

                if (entry.BlockedUntil < DateTimeOffset.UtcNow)
                {
                    entry.FailCount = 0;
                    entry.BlockedUntil = null;
                }

                return false;
            }
        }

        public void RegisterFail(string key)
        {
            lock (_lock)
            {
                if (!_entries.TryGetValue(key, out var entry))
                    _entries[key] = entry = new Entry();

                entry.FailCount++;

                if (entry.FailCount >= 5)
                    entry.BlockedUntil = DateTimeOffset.UtcNow.AddMinutes(1);
            }
        }

        public void Reset(string key)
        {
            lock (_lock)
            {
                if (_entries.TryGetValue(key, out var entry))
                {
                    entry.FailCount = 0;
                    entry.BlockedUntil = null;
                }
            }
        }
    }
}
