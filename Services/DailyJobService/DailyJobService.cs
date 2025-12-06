using Microsoft.Extensions.DependencyInjection;
using SmileProject.Services;

namespace SmileProject.Services.DailyJobService
{
    public class DailyJobService
    {
        private Timer _timer;
        private bool _hasRunToday = false;
        private readonly IServiceScopeFactory _scopeFactory;

        public DailyJobService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;

            // اجرای تایمر هر ساعت
            _timer = new Timer(TimerElapsed, null,
                TimeSpan.Zero,
                TimeSpan.FromHours(1));
        }

        private void TimerElapsed(object state)
        {
            var now = DateTime.Now;

            if (now.Hour == 8 && !_hasRunToday) // مثلا هر روز ساعت 8 صبح
            {
                _ = RunJobAsync();
                _hasRunToday = true;
            }
            else if (now.Hour != 8)
            {
                _hasRunToday = false;
            }
        }

        private async Task RunJobAsync()
        {
            using var scope = _scopeFactory.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<SentencesService>();

            await service.SendSentences();

            Console.WriteLine("Daily job finished at " + DateTime.Now);
        }
    }
}
