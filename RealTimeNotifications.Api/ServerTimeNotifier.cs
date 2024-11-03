using Microsoft.AspNetCore.SignalR;

namespace RealTimeNotifications.Api;

public class ServerTimeNotifier(ILogger<ServerTimeNotifier> logger, IHubContext<NotificationsHub, INotificationClient> context) : BackgroundService
{
    private static readonly TimeSpan Period = TimeSpan.FromSeconds(5);
    private readonly ILogger<ServerTimeNotifier> _logger = logger;
    private readonly IHubContext<NotificationsHub, INotificationClient> _context = context;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(Period);

        while (!stoppingToken.IsCancellationRequested && await timer.WaitForNextTickAsync(stoppingToken))
        {
            var dateTime = DateTime.Now;

            _logger.LogInformation("Executing {Service} {Time}", nameof(ServerTimeNotifier), dateTime);

            await _context.Clients
                .User("f45fe475-8466-484f-af69-a2658a8ee915")
                .ReceiveNotification($"Server time: {dateTime}");
        }
    }
}