using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SmileProject.Services;
using System.Text;
using Swashbuckle;
internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();
        //Database Configuration
        builder.Services.AddDbContext<SmileProject.Databes.MainDbContext.MainDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        //mN%5313do
        builder.Services.AddScoped<SentencesService>();
        builder.Services.AddScoped<Authentication>();
        builder.Services.AddScoped<ShowUsers>();
        builder.Services.AddSingleton<RateLimitation>();
        builder.Services.AddScoped<BoardService>();
        builder.Services.AddScoped<OTPService>();
        builder.Services.AddScoped<GetCategoryService>();
        builder.Services.AddScoped<IResultService, ResultService>();
        builder.Services.AddHttpContextAccessor();
        // Authentication
        // coockie
        // اضافه کردن سرویس احراز هویت با استفاده از کوکی
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultSignInScheme = "Cookies"; // طرح پیش‌فرض برای ورود
            options.DefaultAuthenticateScheme = "Cookies";
            options.DefaultChallengeScheme = "Cookies";
        })
        .AddCookie("Cookies", options =>
        {
            options.LoginPath = "/Account/Login"; // آدرس صفحه لاگین (در صورت نیاز)
            options.AccessDeniedPath = "/Account/AccessDenied";
        });
        //end coockie
        var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]);
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ClockSkew = TimeSpan.Zero
            };
        });

        builder.Services.AddAuthorization();
        var app = builder.Build();
        if (app.Environment.IsDevelopment())
        {

        }
        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseDefaultFiles();
        app.UseStaticFiles();
        app.MapControllers();
        app.Run();
    }
}


