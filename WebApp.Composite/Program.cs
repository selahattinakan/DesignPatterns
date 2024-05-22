using BaseProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApp.Composite.Models;

var builder = WebApplication.CreateBuilder(args);

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
        var newUser1 = new AppUser() { UserName = "user1", Email = "user1@outlook.com" };
        userManager.CreateAsync(newUser1, "Password12*").Wait();
        userManager.CreateAsync(new AppUser() { UserName = "user2", Email = "user2@outlook.com" }, "Password12*").Wait();
        userManager.CreateAsync(new AppUser() { UserName = "user3", Email = "user3@outlook.com" }, "Password12*").Wait();
        userManager.CreateAsync(new AppUser() { UserName = "user4", Email = "user4@outlook.com" }, "Password12*").Wait();
        userManager.CreateAsync(new AppUser() { UserName = "user5", Email = "user5@outlook.com" }, "Password12*").Wait();




        var newCategory1 = new Category { Name = "Suç Romanlarý", ReferenceId = 0, UserId = newUser1.Id };
        var newCategory2 = new Category { Name = "Cinayet Romanlarý", ReferenceId = 0, UserId = newUser1.Id };
        var newCategory3 = new Category { Name = "Polisiye Romanlarý", ReferenceId = 0, UserId = newUser1.Id };
        identituDbContext.Categories.AddRange(newCategory1, newCategory2, newCategory3);
        identituDbContext.SaveChanges();

        var subCategory1 = new Category { Name = "Suç Romanlarý 1", ReferenceId = newCategory1.Id, UserId = newUser1.Id };
        var subCategory2 = new Category { Name = "Cinayet Romanlarý 1", ReferenceId = newCategory2.Id, UserId = newUser1.Id };
        var subCategory3 = new Category { Name = "Polisiye Romanlarý 1", ReferenceId = newCategory3.Id, UserId = newUser1.Id };
        identituDbContext.Categories.AddRange(subCategory1, subCategory2, subCategory3);
        identituDbContext.SaveChanges();

        var subCategory4 = new Category { Name = "Cinayet Romanlarý 1.1", ReferenceId = subCategory2.Id, UserId = newUser1.Id };
        identituDbContext.Categories.AddRange(subCategory4);
        identituDbContext.SaveChanges();

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
