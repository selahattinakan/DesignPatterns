using BaseProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using WebApp.ChainOfResponsibilty.ChainOfResponsibility;
using WebApp.ChainOfResponsibilty.Models;

namespace BaseProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppIdentityDbContext _context;

        public HomeController(ILogger<HomeController> logger, AppIdentityDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        //halkalar birbirine baðlanýyor ve bir iþlem zinciri oluþuyor bu sayede yeni bir iþlem geleceði zaman hangi sýrada çalýþmasý gerekiyorsa ilgili aralýða ekleniyor
        public async Task<IActionResult> SendEmail()
        {
            var products = await _context.Products.ToListAsync();

            ExcelProcessHandler<Product> excelProcessHandler = new ExcelProcessHandler<Product>();

            ZipFileProccessHandler<Product> zipFileProccessHandler = new ZipFileProccessHandler<Product>();

            SendMailProcessHandler sendMailProcessHandler = new SendMailProcessHandler();

            excelProcessHandler.SetNext(zipFileProccessHandler).SetNext(sendMailProcessHandler);

            excelProcessHandler.handle(products);

            return Content("Ýþlemler zinciri çalýþtý");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
