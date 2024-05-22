using BaseProject.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Observer.Events;
using WebApp.Observer.Models;
using WebApp.Observer.Observer;

namespace BaseProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserObserverSubject _userObserverSubject;
        private readonly IMediator _mediator;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, UserObserverSubject userObserverSubject, IMediator mediator)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userObserverSubject = userObserverSubject;
            _mediator = mediator;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var hasUser = await _userManager.FindByEmailAsync(email);

            if (hasUser == null) return View();

            var signInResult = await _signInManager.PasswordSignInAsync(hasUser, password, true, false);

            if (!signInResult.Succeeded) return View();


            return RedirectToAction(nameof(HomeController.Index), "Home");
        }


        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(AccountController.Login), "Account");
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> SignUp(UserCreateViewModel userCreateViewModel)
        {
            var appUser = new AppUser { UserName = userCreateViewModel.UserName, Email = userCreateViewModel.Email };
            var identityResult = await _userManager.CreateAsync(appUser, userCreateViewModel.Password);
            if (identityResult.Succeeded)
            {
                //manuel observer design pattern
                //_userObserverSubject.NotifyObservers(appUser);

                //mediatR kütüphanesi ile observer design pattern
                await _mediator.Publish(new UserCreatedEvent() { AppUser = appUser });

                ViewBag.message = "üyelik başarılı";
            }
            else
            {
                ViewBag.message = $"üyelik başarısız : {identityResult.Errors.ToList().First().Description}";
            }
            return View();
        }
    }
}
