using System.Collections.Concurrent;

namespace WorldCup.Shared.Services;

/// <summary>
/// Simple in-memory rate limiter for email sending
/// Prevents users from spamming email verification requests
/// </summary>
public class EmailRateLimiter
{
    private readonly ConcurrentDictionary<string, DateTime> _lastEmailSent = new();
    private readonly TimeSpan _minimumInterval;

    public EmailRateLimiter(TimeSpan? minimumInterval = null)
    {
        _minimumInterval = minimumInterval ?? TimeSpan.FromMinutes(2);
    }

    /// <summary>
    /// Check if user can send another email
    /// </summary>
    public bool CanSendEmail(string userId, out TimeSpan? remainingTime)
    {
        if (_lastEmailSent.TryGetValue(userId, out var lastSent))
        {
            var elapsed = DateTime.UtcNow - lastSent;
            if (elapsed < _minimumInterval)
            {
                remainingTime = _minimumInterval - elapsed;
                return false;
            }
        }

        remainingTime = null;
        return true;
    }

    /// <summary>
    /// Record that an email was sent to this user
    /// </summary>
    public void RecordEmailSent(string userId)
    {
        _lastEmailSent[userId] = DateTime.UtcNow;
    }

    /// <summary>
    /// Clean up old entries (should be called periodically)
    /// </summary>
    public void Cleanup()
    {
        var cutoff = DateTime.UtcNow.Subtract(_minimumInterval.Add(TimeSpan.FromHours(1)));
        var keysToRemove = _lastEmailSent
            .Where(kvp => kvp.Value < cutoff)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            _lastEmailSent.TryRemove(key, out _);
        }
    }
}
