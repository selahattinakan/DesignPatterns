using BaseProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Strategy.Models;
using WebApp.Strategy.Repositories;

var builder = WebApplication.CreateBuilder(args);

//dinamik dbType
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IProductRepository>(sp =>
{
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var claim = httpContextAccessor.HttpContext.User.Claims.Where(x => x.Type == Settings.claimDatabaseType).FirstOrDefault();
    var context = sp.GetRequiredService<AppIdentityDbContext>();
    if (claim == null) return new ProductRepositoryFromSqlServer(context);

    var databaseType = (EDatabaseType)int.Parse(claim.Value);

    return databaseType switch
    {
        EDatabaseType.SqlServer => new ProductRepositoryFromSqlServer(context),
        EDatabaseType.MongoDb => new ProductRepositoryFromMongoDb(builder.Configuration),
        _ => throw new NotImplementedException()
    };
});
//dinamik dbType

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
