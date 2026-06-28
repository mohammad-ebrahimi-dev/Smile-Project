using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using SmileProject.Services;
using SmileProject.Services.Dashboard;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Services
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Database
        builder.Services.AddDbContext<SmileProject.Databes.MainDbContext.MainDbContext>(options =>
            options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection")));

        // DI
        builder.Services.AddScoped<SentencesService>();
        builder.Services.AddScoped<Authentication>();
        builder.Services.AddScoped<ShowUsers>();
        builder.Services.AddSingleton<RateLimitation>();
        builder.Services.AddScoped<BoardService>();
        builder.Services.AddScoped<OTPService>();
        builder.Services.AddScoped<SaveCategoryService>();
        builder.Services.AddScoped<GetCategoryService>();
        builder.Services.AddScoped<IResultService, ResultService>();

        builder.Services.AddHttpContextAccessor();

        // Cookie Authentication
        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Login.html";
                options.AccessDeniedPath = "/Login.html";
            });

        builder.Services.AddAuthorization();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseAuthorization();

        // Protect html pages
        app.Use(async (context, next) =>
        {
            Console.WriteLine($"Path : {context.Request.Path}");
            Console.WriteLine($"Auth : {context.User.Identity?.IsAuthenticated}");

            if (context.Request.Path.Equals("/Dashboard.html", StringComparison.OrdinalIgnoreCase))
            {
                if (!(context.User.Identity?.IsAuthenticated ?? false))
                {
                    context.Response.Redirect("/Login.html");
                    return;
                }
            }

            await next();
        });

        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.MapControllers();

        app.Run();
    }
}