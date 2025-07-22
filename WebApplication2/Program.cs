using WebApplication2.Hubs;
using WebApplication2.Data;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Pages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Add services to the container.
builder.Services.AddSession();
builder.Services.AddRazorPages();
builder.Services.AddSignalR();
builder.Services.AddHttpClient("UserApi", client =>
{
    client.BaseAddress = new Uri("https://localhost:7214/");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});
builder.Services.AddAuthentication("Cookies")
    .AddCookie("Cookies", options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/AccessDenied";
    });
builder.Services.AddControllers();
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(15);
    options.SlidingExpiration = false;
    options.LoginPath = "/Login";
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRazorPages();
app.MapHub<ChatHub>("/chatHub");

app.Run();
