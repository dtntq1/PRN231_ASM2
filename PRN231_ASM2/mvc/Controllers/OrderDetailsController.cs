using api.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace mvc.Controllers
{
    public class OrderDetailsController : Controller
    {
        private readonly IHttpContextAccessor contxt;
        private readonly HttpClient client;
        private string orderDetailsURL = "https://localhost:7294/odata/OrderDetails";
        public OrderDetailsController(IHttpContextAccessor httpContextAccessor)
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
            contxt = httpContextAccessor;
        }
        public async Task<ActionResult> Index()
        {
            HttpResponseMessage res = await client.GetAsync(orderDetailsURL);
            string strData = await res.Content.ReadAsStringAsync();

            var data = JObject.Parse(strData);
            List<OrderDetail> orderDetails = JsonConvert.DeserializeObject<List<OrderDetail>>(data["value"].ToString());
            return View(orderDetails);
        }
        // GET: MembersController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            HttpResponseMessage res = await client.GetAsync($"{orderDetailsURL}/{id}");
            string strData = await res.Content.ReadAsStringAsync();

            OrderDetail order = JsonConvert.DeserializeObject<OrderDetail>(strData);
            return View(order);
        }

        // GET: MembersController/Create
        public async Task<ActionResult> Create(int id)
        {
            var memberId = contxt.HttpContext.Session.GetInt32("userId");
            string productURL = "https://localhost:7294/odata/Products";
            HttpResponseMessage res = await client.GetAsync(productURL);
            string strData = await res.Content.ReadAsStringAsync();
            var data = JObject.Parse(strData);
            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(data["value"].ToString());
            var productsList = new SelectList(products, "ProductId", "ProductName");
            ViewBag.Products = productsList;
            ViewData["OrderId"] = id;
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProductId,OrderId,UnitPrice,Quantity,Discount")] OrderDetail orderDetail)
        {
            try
            {
                string strData = JsonConvert.SerializeObject(orderDetail);
                var content = new StringContent(strData, System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage res = await client.PostAsync(orderDetailsURL, content);
                res.EnsureSuccessStatusCode();
                return RedirectToAction("Index", "Orders");
            }
            catch
            {
                return View();
            }
        }
        // GET: MembersController/Delete/5
        public async Task<ActionResult> Delete(int id)
        {
            HttpResponseMessage res = await client.GetAsync($"{orderDetailsURL}/{id}");
            string strData = await res.Content.ReadAsStringAsync();

            OrderDetail order = JsonConvert.DeserializeObject<OrderDetail>(strData);
            return View(order);
        }

        // POST: MembersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Order order)
        {
            try
            {
                HttpResponseMessage res = await client.DeleteAsync($"{orderDetailsURL}/{id}");
                res.EnsureSuccessStatusCode();

                return RedirectToAction("Index", "Orders");
            }
            catch
            {
                return View(order);
            }
        }

        // GET: MembersController/Edit/5
        public async Task<ActionResult> Edit(int id)
        {
            string productURL = "https://localhost:7294/odata/Orders";
            HttpResponseMessage res = await client.GetAsync(productURL);
            string strData1 = await res.Content.ReadAsStringAsync();
            var data = JObject.Parse(strData1);
            List<Order> products = JsonConvert.DeserializeObject<List<Order>>(data["value"].ToString());
            var productsList = new SelectList(products, "ProductId", "ProductName");
            ViewBag.Orders = productsList;

            HttpResponseMessage response = await client.GetAsync($"{orderDetailsURL}/{id}");
            string strData = await res.Content.ReadAsStringAsync();

            OrderDetail order = JsonConvert.DeserializeObject<OrderDetail>(strData);
            ViewData["orderId"] = order.OrderId;
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
                HttpResponseMessage res = await client.PutAsync($"{orderDetailsURL}/{id}", content);
                res.EnsureSuccessStatusCode();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
