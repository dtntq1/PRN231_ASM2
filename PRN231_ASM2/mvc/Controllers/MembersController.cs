using api.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Net.Http.Headers;

namespace mvc.Controllers
{
    public class MembersController : Controller
    {
        private readonly HttpClient client;
        private string memberURL = "https://localhost:7294/odata/Members";
        public MembersController()
        {
            client = new HttpClient();
            var contentType = new MediaTypeWithQualityHeaderValue("application/json");
            client.DefaultRequestHeaders.Accept.Add(contentType);
        }
        // GET: MembersController
        public async Task<ActionResult> Index(string? email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                ViewData["emailName"] = email;
                HttpResponseMessage res = await client.GetAsync($"{memberURL}?$filter=contains(Email,'{email}')");
                string strData = await res.Content.ReadAsStringAsync();

                var data = JObject.Parse(strData);
                List<Member> members = JsonConvert.DeserializeObject<List<Member>>(data["value"].ToString());

                return View(members);
            }
            else
            {
                HttpResponseMessage res = await client.GetAsync(memberURL);
                string strData = await res.Content.ReadAsStringAsync();

                var data = JObject.Parse(strData);
                List<Member> members = JsonConvert.DeserializeObject<List<Member>>(data["value"].ToString());

                return View(members);
            }
        }

        // GET: MembersController/Details/5
        public async Task<ActionResult> Details(int id)
        {
            HttpResponseMessage res = await client.GetAsync($"{memberURL}/{id}");
            string strData = await res.Content.ReadAsStringAsync();

            Member member = JsonConvert.DeserializeObject<Member>(strData);
            return View(member);
        }

        // GET: MembersController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: MembersController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Member member)
        {
            try
            {
                string strData = JsonConvert.SerializeObject(member);
                var content = new StringContent(strData, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage res = await client.PostAsync(memberURL, content);
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
            HttpResponseMessage res = await client.GetAsync($"{memberURL}/{id}");
            string strData = await res.Content.ReadAsStringAsync();

            Member member = JsonConvert.DeserializeObject<Member>(strData);
            return View(member);
        }

        // POST: MembersController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit(int id, [Bind("MemberId", "Email", "CompanyName", "City", "Country", "Password")] Member member)
        {
            try
            {
                Member newMem = new Member
                {
                    Password = member.Password,
                    City = member.City,
                    CompanyName = member.CompanyName,
                    Country = member.Country,
                    Email = member.Email,
                    MemberId = member.MemberId,
                };
                string strData = JsonConvert.SerializeObject(newMem);
                var content = new StringContent(strData, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage res = await client.PutAsync($"{memberURL}/{id}", content);
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
            HttpResponseMessage res = await client.GetAsync($"{memberURL}/{id}");
            string strData = await res.Content.ReadAsStringAsync();

            Member member = JsonConvert.DeserializeObject<Member>(strData);
            return View(member);
        }

        // POST: MembersController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int id, Member member)
        {
            try
            {
                HttpResponseMessage res = await client.DeleteAsync($"{memberURL}/{id}");
                res.EnsureSuccessStatusCode();

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View(member);
            }
        }
    }
}
