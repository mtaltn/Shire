using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Shire.WebMvc.Contexts;
using Shire.WebMvc.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
//builder.Services.AddIdentity<User, UserRole>();
builder.Services.AddIdentity<User, UserRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();
builder.Services.AddScoped<UserManager<User>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
    name: "login",
    pattern: "Account/Login",
    defaults: new { controller = "Account", action = "Login" });
    endpoints.MapDefaultControllerRoute();
});

app.Run();
