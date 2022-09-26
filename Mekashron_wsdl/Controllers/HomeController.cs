using Mekashron_wsdl.Models;
using Mekashron_wsdl.Models.BindingsModels;
using Mekashron_wsdl.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Xml;

namespace Mekashron_wsdl.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISendRequest _sendRequest;

        public HomeController(ILogger<HomeController> logger, ISendRequest sendRequest)
        {
            _logger = logger;
            _sendRequest = sendRequest;
        }

        public IActionResult Index()
        {
            var model = new IndexBinding();
            return View(model);
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
        [HttpPost]
        public IActionResult Login(User user)
        {
            if(!ModelState.IsValid)
            {
                var userData = _sendRequest.Send(user);
                if (userData.EntityId == "-1")
                {
                    var model = new IndexBinding() { ErrorMessage = "User not found, try again. Probably an incorrect email or password", User = user };
                    return View("Index", model);
                }
                else
                {
                    var model = new IndexBinding() { SuccessMessage = "User successfully found", UserData = userData };
                    return View("Index", model);
                }
            }
            else
            {
                return BadRequest();
            }
        }
    }
}