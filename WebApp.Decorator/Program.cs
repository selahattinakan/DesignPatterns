using BaseProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using WebApp.Decorator.Repositories;
using WebApp.Decorator.Repositories.Decorator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMemoryCache();
builder.Services.AddHttpContextAccessor();

//2. Yol - Scrutor kütüphanesi kullanýlarak yapýldý
//builder.Services.AddScoped<IProductRepository, ProductRepository>().Decorate<IProductRepository, ProductRepositoryCacheDecorator>().Decorate<IProductRepository, ProductRepositoryLoggingDecorator>();

//1. Yol - manuel olarak yapýldý
//builder.Services.AddScoped<IProductRepository>(sp =>
//{
//    var context = sp.GetRequiredService<AppIdentityDbContext>();
//    var memoryCache = sp.GetRequiredService<IMemoryCache>();
//    var productRepository = new ProductRepository(context);
//    var logService = sp.GetRequiredService<ILogger<ProductRepositoryLoggingDecorator>>();


//    var cacheDecorator = new ProductRepositoryCacheDecorator(productRepository, memoryCache);
//    var logDecorator = new ProductRepositoryLoggingDecorator(cacheDecorator, logService);

//    //yeni bir decorator eklenmek istediðinde onun product repository parametresine de logDecorator'u vericez ve onu return yapýcaz

//    return logDecorator;
//});

// 3. Yol - Runtime senaryosu : user1 için cachleme yapsýn, diðerleri için loglama
builder.Services.AddScoped<IProductRepository>(sp =>
{
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();

    var context = sp.GetRequiredService<AppIdentityDbContext>();
    var memoryCache = sp.GetRequiredService<IMemoryCache>();
    var productRepository = new ProductRepository(context);
    var logService = sp.GetRequiredService<ILogger<ProductRepositoryLoggingDecorator>>();

    if (httpContextAccessor.HttpContext.User.Identity.Name == "user1")
    {
        var cacheDecorator = new ProductRepositoryCacheDecorator(productRepository, memoryCache);
        return cacheDecorator;
    }

    var logDecorator = new ProductRepositoryLoggingDecorator(productRepository, logService);
    return logDecorator;

    //yeni bir decorator eklenmek istediðinde onun product repository parametresine de logDecorator'u vericez ve onu return yapýcaz


});


builder.Services.AddDbContext<AppIdentityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"));
});

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.User.RequireUniqueEmail = true;
}).AddEntityFrameworkStores<AppIdentityDbContext>();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var identituDbContext = scope.ServiceProvider.GetRequiredService<AppIdentityDbContext>();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();

    identituDbContext.Database.Migrate();//migration sonrasý update-database kýsmýný otomatize ettik

    if (!userManager.Users.Any())
    {
        userManager.CreateAsync(new AppUser() { UserName = "user1", Email = "user1@outlook.com" }, "Password12*").Wait();
        userManager.CreateAsync(new AppUser() { UserName = "user2", Email = "user2@outlook.com" }, "Password12*").Wait();
        userManager.CreateAsync(new AppUser() { UserName = "user3", Email = "user3@outlook.com" }, "Password12*").Wait();
        userManager.CreateAsync(new AppUser() { UserName = "user4", Email = "user4@outlook.com" }, "Password12*").Wait();
        userManager.CreateAsync(new AppUser() { UserName = "user5", Email = "user5@outlook.com" }, "Password12*").Wait();
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
