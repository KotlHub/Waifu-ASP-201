using ASP_201.Data;
using ASP_201.Data.Entity;
using ASP_201.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Diagnostics;
using System.Security.Claims;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ASP_201.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly DataContext _dataContext;
        private readonly Models.Home.PassDataModelWaifu _waifuModel;

        public HomeController(ILogger<HomeController> logger,
            IConfiguration configuration,
            DataContext dataContext)
        {
            _logger = logger;
            _configuration = configuration;
            _dataContext = dataContext;
            _waifuModel = new()
            {
                Header = "Waifu&Husbandu",
                Title = "Waifu&Husbandu",
                WaifuHusbanduTable = _dataContext.WaifuHusbanduTable.ToList()
            };
        }

        public IActionResult Index()
        {
            return View(_waifuModel);
        }
        public IActionResult Search()
        {
            return View(_waifuModel);
        }
        [HttpPost]
        public IActionResult Search([FromRoute] string name, int? age, string character, string type)
        {
            var filteredData = _waifuModel.WaifuHusbanduTable
        .Where(w =>
            (string.IsNullOrEmpty(name) || w.Name.Contains(name)) &&
            (age == 0 || w.Age == age) &&
            (string.IsNullOrEmpty(character) || w.Characteristic.Contains(character)) &&
            (type == "value0" || (type == "value1" && w.Type == "Waifu") || (type == "value2" && w.Type == "Husbandu"))
        )
        .ToList();

            return View(_waifuModel);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public ViewResult Page404() {
            return View();
        }
       /*------------------------------------*/
       /* public ViewResult Sessions([FromQuery(Name = "session-attr")]String? sessionAttr)
        {
            if (sessionAttr is not null)
            {
                HttpContext.Session.SetString("session-attribute", sessionAttr);
            }
            return View();
        }
        public ViewResult EmailConfirmation()
        {
            ViewData["config"] = _configuration["Smtp:Gmail:Host"];
            return View();
        }*/

    }
}
