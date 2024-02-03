using api.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace mvc.Controllers
{
    public class OrdersController : Controller
    {
        private readonly IHttpContextAccessor contxt;
        private readonly HttpClient client;
        private string orderURL = "https://localhost:7294/odata/Orders";
        public OrdersController(IHttpContextAccessor httpContextAccessor)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            contxt = httpContextAccessor;
        }
        public async Task<ActionResult> Index()
        {
            var memberId = contxt.HttpContext.Session.GetInt32("userId");

            HttpResponseMessage res = await client.GetAsync($"{orderURL}?$filter=MemberId eq {memberId}");
            string strData = await res.Content.ReadAsStringAsync();

            var data = JObject.Parse(strData);
            List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(data["value"].ToString());

            return View(orders);
        }

        // GET: MembersController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            HttpResponseMessage res = await client.GetAsync($"{orderURL}/{id}");
            string strData = await res.Content.ReadAsStringAsync();

            Order order = JsonConvert.DeserializeObject<Order>(strData);
            return View(order);
        }

        // GET: MembersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MembersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Order order)
        {
            try
            {
                var memberId = contxt.HttpContext.Session.GetInt32("userId");
                RequestOrder newOrder = new RequestOrder
                {
                    MemberId = (int)memberId,
                    OrderDate = order.OrderDate,
                    RequiredDate = order.RequiredDate,
                    ShippedDate = order.ShippedDate,
                    Freight = order.Freight,
                };
                string strData = JsonConvert.SerializeObject(newOrder);
                var content = new StringContent(strData, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage res = await client.PostAsync(orderURL, content);
                res.EnsureSuccessStatusCode();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MembersController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            HttpResponseMessage res = await client.GetAsync($"{orderURL}/{id}");
            string strData = await res.Content.ReadAsStringAsync();

            Order order = JsonConvert.DeserializeObject<Order>(strData);
            ViewData["OrderDetailId"] = id;
            ViewData["memberId"] = contxt.HttpContext.Session.GetInt32("userId");
            return View(order);
        }

        // POST: MembersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId", "MemberId", "OrderDate", "RequiredDate", "ShippedDate", "Freight")] Order order)
        {
            try
            {
                //nice
                var newOrder = new RequestOrder
                {
                    OrderId = order.OrderId,
                    MemberId = order.MemberId,
                    OrderDate = order.OrderDate,
                    RequiredDate = order.RequiredDate,
                    ShippedDate = order.ShippedDate,
                    Freight = order.Freight,
                };
                string strData = JsonConvert.SerializeObject(newOrder);
                var content = new StringContent(strData, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage res = await client.PutAsync($"{orderURL}/{id}", content);
                res.EnsureSuccessStatusCode();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: MembersController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            HttpResponseMessage res = await client.GetAsync($"{orderURL}/{id}");
            string strData = await res.Content.ReadAsStringAsync();

            Order order = JsonConvert.DeserializeObject<Order>(strData);
            return View(order);
        }

        // POST: MembersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Order order)
        {
            try
            {
                HttpResponseMessage res = await client.DeleteAsync($"{orderURL}/{id}");
                res.EnsureSuccessStatusCode();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(order);
            }
        }
    }
}
