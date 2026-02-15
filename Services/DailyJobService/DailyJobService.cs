//using Microsoft.Extensions.DependencyInjection;
//using SmileProject.Services;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace SmileProject.Services.DailyJobService
//{
//    public class DailyJobService
//    {
//        private Timer _timer;
//        private bool _hasRunToday = false;
//        private readonly IServiceScopeFactory _scopeFactory;

//        public DailyJobService(IServiceScopeFactory scopeFactory)
//        {
//            _scopeFactory = scopeFactory;

//            // Timer هر 30 ثانیه اجرا میشه تا ساعت 17 رو بررسی کنه
//            _timer = new Timer(TimerElapsed, null,
//                TimeSpan.Zero,
//                TimeSpan.FromSeconds(30));
//        }

//        private void TimerElapsed(object state)
//        {
//            var now = DateTime.Now;

//            // اجرا دقیقاً ساعت 18 و اگر امروز هنوز اجرا نشده
//            if (now.Hour == 18 && !_hasRunToday)
//            {
//                _hasRunToday = true;
//                _ = RunJobAsync();
//            }

//            // ریست flag هر روز در نیمه شب
//            if (now.Hour == 0 && now.Minute == 0 && now.Second < 30)
//            {
//                _hasRunToday = false;
//            }
//        }

//        private async Task RunJobAsync()
//        {
//            using var scope = _scopeFactory.CreateScope();
//            var service = scope.ServiceProvider.GetRequiredService<SentencesService>();

//            await service.SendSentences();

//            Console.WriteLine("Daily job finished at " + DateTime.Now);
//        }
//    }
//}


////namespace SmileProject.Services.DailyJobService
////{
////    public class DailyJobService
////    {
////        private Timer _timer;
////        private bool _hasRunToday = false;
////        private readonly IServiceScopeFactory _scopeFactory;

////        public DailyJobService(IServiceScopeFactory scopeFactory)
////        {
////            _scopeFactory = scopeFactory;

////            _timer = new Timer(TimerElapsed, null,
////                TimeSpan.Zero,
////                TimeSpan.FromSeconds(1));
////        }

////        private void TimerElapsed(object state)
////        {
////            var now = DateTime.Now;

////            if ((now.Hour == 16 || now.Hour == 17) && !_hasRunToday)
////            {
////                _ = RunJobAsync();
////                _hasRunToday = true;
////            }
////            else if (now.Hour != 8)
////            {
////                _hasRunToday = false;
////            }
////        }

////        private async Task RunJobAsync()
////        {
////            using var scope = _scopeFactory.CreateScope();
////            var service = scope.ServiceProvider.GetRequiredService<SentencesService>();

////            await service.SendSentences();

////            Console.WriteLine("Daily job finished at " + DateTime.Now);
////        }
////    }
////}
