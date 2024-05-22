using BaseProject.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using WebApp.Observer.Observer;

var builder = WebApplication.CreateBuilder(args);


/*
 * burada yapýlmak istenen iþlemleri temisl eden sýnýflar UserObserverSubject sýnýfýna kayýt ediliyor, 
 * aþaðýdaki sýnýflar(console,discount,mail) IUserObserver interface'inden miras aldýðý için ve 
 * UserObserverSubject bu interface'i miras alanlarýn listesini tuttuðu için hepsine tek bir sýnýf üzerinden eriþebiliyoruz.
 * böylece bu interface'i miras alan tüm sýnýflar burada kayýt edildiði zaman otomatik olarka ilgili metodu çalýþtýrmýþ olacaklar.
 */

//--manuel observer design pattern--
builder.Services.AddSingleton<UserObserverSubject>(sp =>
{
    UserObserverSubject userObserverSubject = new();
    userObserverSubject.RegisterObserver(new UserObserverWriteToConsole(sp));
    userObserverSubject.RegisterObserver(new UserObserverCreateDiscount(sp));
    userObserverSubject.RegisterObserver(new UserObserverSendMail(sp));
    return userObserverSubject;
});

/*MediatR kütüphanesi ile ayný iþleri yapabiliriz Event ve EventHandlers klasörlerindeki dosyalar incelenebilir*/
builder.Services.AddMediatR(Assembly.GetExecutingAssembly());


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
