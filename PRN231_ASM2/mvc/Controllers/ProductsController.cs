using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using api.Models;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Diagnostics.Metrics;

namespace mvc.Controllers
{
    public class ProductsController : Controller
    {
        private readonly HttpClient client;
        private string productURL = "https://localhost:7294/odata/Products";
        private string categoryURL = "https://localhost:7294/odata/Categories";
        public ProductsController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            HttpResponseMessage res = await client.GetAsync(productURL);
            string strData = await res.Content.ReadAsStringAsync();

            var data = JObject.Parse(strData);
            List<Product> products = JsonConvert.DeserializeObject<List<Product>>(data["value"].ToString());

            return View(products);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            HttpResponseMessage res = await client.GetAsync($"{productURL}/{id}");
            string strData = await res.Content.ReadAsStringAsync();

            Product member = JsonConvert.DeserializeObject<Product>(strData);
            return View(member);
        }

        // GET: Products/Create
        public async Task<IActionResult> Create()
        {
            HttpResponseMessage response = await client.GetAsync(categoryURL);
            string strCate = await response.Content.ReadAsStringAsync();
            var data = JObject.Parse(strCate);
            List<Category> categories = JsonConvert.DeserializeObject<List<Category>>(data["value"].ToString());
            var categoryList = new SelectList(categories, "CategoryId", "CategoryName");
            ViewBag.Category = categoryList;
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CategoryId,ProductName,Weight,UnitPrice,UnitInStock")] Product product)
        {
            try
            {
                string strData = JsonConvert.SerializeObject(product);
                var content = new StringContent(strData, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage res = await client.PostAsync(productURL, content);
                res.EnsureSuccessStatusCode();
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            HttpResponseMessage res = await client.GetAsync($"{productURL}/{id}");
            string strData = await res.Content.ReadAsStringAsync();

            Product product = JsonConvert.DeserializeObject<Product>(strData);
            ViewData["CategoryID"] = product.CategoryId;
            ViewData["ProductID"] = product.ProductId;
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,CategoryId,ProductName,Weight,UnitPrice,UnitInStock")] Product product)
        {
            try
            {
                string strData = JsonConvert.SerializeObject(product);
                var content = new StringContent(strData, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage res = await client.PutAsync($"{productURL}/{id}", content);
                res.EnsureSuccessStatusCode();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            HttpResponseMessage res = await client.GetAsync($"{productURL}/{id}");
            string strData = await res.Content.ReadAsStringAsync();

            Product product = JsonConvert.DeserializeObject<Product>(strData);
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id,Product product)
        {
            try
            {
                HttpResponseMessage res = await client.DeleteAsync($"{productURL}/{id}");
                res.EnsureSuccessStatusCode();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(product);
            }
        }
    }
}
