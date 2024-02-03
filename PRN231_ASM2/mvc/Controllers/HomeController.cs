using api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using mvc.Models;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.Net.Http.Headers;

namespace mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHttpContextAccessor contxt;
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient client;
        private string requestURL = "https://localhost:7294/odata/Login";
        public HomeController(ILogger<HomeController> logger, IHttpContextAccessor httpContextAccessor)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            _logger = logger;
            contxt = httpContextAccessor;
        }
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login([Bind("Email,Password")] RequestMember user)
        {
            if (ModelState.IsValid)
            {
                string strData = JsonConvert.SerializeObject(user);
                int role=0;
                var content = new StringContent(strData, System.Text.Encoding.UTF8, "application/json");
                if (user.Email == "admin@estore.com" && user.Password == "admin@@")
                {
                    role = 1;
                }
                else
                {
                    HttpResponseMessage response = await client.PostAsync(requestURL, content);
                    string memberData = await response.Content.ReadAsStringAsync();
                    Member member = JsonConvert.DeserializeObject<Member>(memberData);
                    if (response.IsSuccessStatusCode)
                    {
                        role = 2;
                        contxt.HttpContext.Session.SetInt32("userId", member.MemberId);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Thông tin đăng nhập không đúng");
                        return View(user);
                    }
                }
                contxt.HttpContext.Session.SetInt32("userRole",role);
                contxt.HttpContext.Session.SetString("userName", user.Email);
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Logout()
        {
            contxt.HttpContext.Session.Clear(); // Clear all session data
            return RedirectToAction(nameof(Index));
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
