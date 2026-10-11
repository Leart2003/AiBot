using Infrastructure.Persistence;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpClient<ChatAiService>(client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Llm:BaseUrl"]
        ?? throw new InvalidOperationException("Llm:BaseUrl is missing in appsettings.json."));
    client.Timeout = TimeSpan.FromSeconds(60);
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(
        "Bearer",
        builder.Configuration["Llm:ApiKey"]
        ?? throw new InvalidOperationException("Llm:ApiKey is missing. Set it with dotnet user-secrets."));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();