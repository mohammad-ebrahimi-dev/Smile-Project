using System.Collections.Concurrent;
public class RateLimitation
{
    private static readonly ConcurrentDictionary<string, DateTime> _requests = new();

    private readonly TimeSpan _limit = TimeSpan.FromSeconds(60);

    public (bool allowed, int retryAfterSeconds) Check(string key)
    {
        var now = DateTime.UtcNow;

        if (_requests.TryGetValue(key, out var lastTime))
        {
            var diff = now - lastTime;

            if (diff < _limit)
            {
                var remaining = (int)(_limit - diff).TotalSeconds;
                return (false, remaining);
            }

            _requests[key] = now;
            return (true, 0);
        }

        _requests[key] = now;
        return (true, 0);
    }
}