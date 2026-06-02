using Microsoft.AspNetCore.Mvc;
using IPE.SmsIrClient;
using Microsoft.EntityFrameworkCore;
using SmileProject.Databes.Entities;
using SmileProject.Databes.MainDbContext;
namespace SmileProject.Services
{
    public class SmsService : Controller
    {
        public async Task<bool> SendWelcomeSmsAsync(string mobile, string firstName, string lastName)
        {
            try
            {
                SmsIr smsIr = new SmsIr("NXqgkyS7aW23D98kgjqukfbbGw9rSjGQVSK6mVOLXF8eP28d");
                var message = $"{firstName} {lastName} welcome to smile";
                var mobileNumber = mobile;
                var bulkSendResult = await smsIr.BulkSendAsync(
                 50003181890144,  
                 message,          
                 new string[] { mobileNumber } 
                 );
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
